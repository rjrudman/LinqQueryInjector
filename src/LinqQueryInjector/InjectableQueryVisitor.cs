using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using InvokeInliner;
using LinqQueryInjector.Builders;

namespace LinqQueryInjector
{
    internal class InjectableQueryVisitor : ExpressionVisitor
	{
	    private readonly Stack<List<IReplaceRule>> _replaceRulesContext = new Stack<List<IReplaceRule>>();

	    internal InjectableQueryVisitor(IEnumerable<IReplaceRule> replaceRulesContext)
	    {
		    _replaceRulesContext.Push(replaceRulesContext.ToList());
	    }

		public override Expression Visit(Expression node)
		{
			if (node == null)
				return null;

			if (!IsExpressionInjectionCall(node) && _replaceRulesContext.Count > 0)
			{
				var replaceRules = _replaceRulesContext.Peek();
				var matches = replaceRules.Where(rr => rr.ReplaceType.GetTypeInfo().IsAssignableFrom(node.Type)).ToList();

				var currentExpr = base.Visit(node);

				foreach (var match in matches)
				{	
					var result = Expression.Invoke(match.ReplaceWithExpr, currentExpr);
					currentExpr = result.InlineInvokes();
				}
				return currentExpr;
			}
			
			return base.Visit(node);
		}

		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			//We don't want to visit the parameter definitions
			var bodyResult = Visit(node.Body);
			return Expression.Lambda(bodyResult, node.Parameters);
		}

		private static readonly HashSet<MethodInfo> _registerMethods =
			new HashSet<MethodInfo>(
				typeof(InjectableQueryExtensions)
					.GetRuntimeMethods()
					.Where(m => m.Name == "RegisterInject")
			);

		private static bool IsExpressionInjectionCall(Expression node)
		{
			var methodCallExpr = node as MethodCallExpression;
			if (methodCallExpr == null)
				return false;

			var baseMethod = methodCallExpr.Method.IsGenericMethod ? methodCallExpr.Method.GetGenericMethodDefinition() : methodCallExpr.Method;
			return _registerMethods.Contains(baseMethod)
			       && methodCallExpr.Arguments.Count == 2
			       & methodCallExpr.Arguments[1] is ConstantExpression;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (!IsExpressionInjectionCall(node))
				return base.VisitMethodCall(node);
			
			//First argument is the source
			var secondArgument = (ConstantExpression)node.Arguments[1];

			var currentRules = new List<IReplaceRule>(_replaceRulesContext.Count > 0 ? _replaceRulesContext.Peek() : Enumerable.Empty<IReplaceRule>());
			var replaceRules = secondArgument.Value as Func<IQueryInjectorBuilder, IReplaceRule>[];
			var builder = new QueryInjectorBuilder();
			var encounterObjs = replaceRules.Select(encounterFunc => encounterFunc(builder)).ToList();
			currentRules.AddRange(encounterObjs);
			_replaceRulesContext.Push(currentRules);
			var returnExpression = Visit(node.Arguments[0]);
			_replaceRulesContext.Pop();
			return returnExpression;
		}
	}
}
