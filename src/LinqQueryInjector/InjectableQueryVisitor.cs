using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using InvokeInliner;
using LinqQueryInjector.Builders;

namespace LinqQueryInjector
{
    internal class InjectableQueryVisitor : ExpressionVisitor
	{
		class InjectedItem { public object Key; public object Value; }

	    private Stack<List<IReplaceRule>> _replaceRulesContext = new Stack<List<IReplaceRule>>();
		private readonly Stack<InjectedItem> _injectedItems = new Stack<InjectedItem>();

	    internal InjectableQueryVisitor(IEnumerable<IReplaceRule> replaceRulesContext)
	    {
		    _replaceRulesContext.Push(replaceRulesContext.ToList());
	    }

		public override Expression Visit(Expression node)
		{
			if (node == null)
				return null;

			var currentExpr = base.Visit(node);

			if (IsMetaCall(node))
				return currentExpr;

			if (_replaceRulesContext.Count > 0)
			{
				var matches = _replaceRulesContext.Peek()
					.Where(m => m.ReplaceType.GetTypeInfo().IsAssignableFrom(currentExpr.Type))
					.Where(m => m.ExpressionTypes.Contains(currentExpr.NodeType))
					.ToList();
				
				if (matches.Any())
				{	
					_replaceRulesContext.Push(new List<IReplaceRule>());
					foreach (var match in matches)
					{
						var replaceWithExpr = Visit(match.ReplaceWithExpr);

						var result = Expression.Invoke(replaceWithExpr, currentExpr);
						currentExpr = result.InlineInvokes();
					}
					_replaceRulesContext.Pop();
					return currentExpr;
				}
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

		private static readonly HashSet<MethodInfo> _injectWithMethods =
			new HashSet<MethodInfo>(
				typeof(InjectableQueryExtensions)
					.GetTypeInfo()
					.GetMethods()
					.Where(m => m.Name == "InjectWith")
			);

		private static bool IsInMethods(Expression node, HashSet<MethodInfo> methods)
		{
			var methodCallExpr = node as MethodCallExpression;
			if (methodCallExpr == null)
				return false;
			
			var baseMethod = methodCallExpr.Method.IsGenericMethod ? methodCallExpr.Method.GetGenericMethodDefinition() : methodCallExpr.Method;
			return methods.Contains(baseMethod);
		}

		private static bool IsMetaCall(Expression node)
		{
			return IsRegisterCall(node) || IsInjectWithCall(node) || IsInjectPlaceholderCall(node);
		}
		
		private static bool IsRegisterCall(Expression node)
		{
			return IsInMethods(node, _registerMethods);
		}

		private static bool IsInjectWithCall(Expression node)
		{
			return IsInMethods(node, _injectWithMethods);
		}

		private static bool IsInjectPlaceholderCall(Expression node)
		{
			var methodCallExpr = node as MethodCallExpression;
			if (methodCallExpr == null)
				return false;

			return
				methodCallExpr.Method.Name == "Value"
				&& methodCallExpr.Method.DeclaringType.IsConstructedGenericType
				&& methodCallExpr.Method.DeclaringType.GetGenericTypeDefinition() == typeof(Inject<>);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (IsRegisterCall(node))
				return TranslateRegisterCall(node);
			if (IsInjectWithCall(node))
				return TranslateInjectWithCall(node);
			if (IsInjectPlaceholderCall(node))
				return TranslateInjectPlaceholder(node);

			return base.VisitMethodCall(node);
		}

		private Expression TranslateRegisterCall(MethodCallExpression node)
		{
			//First argument is the source
			var secondArgument = (ConstantExpression) node.Arguments[1];
			var replaceRules = secondArgument.Value as Func<IQueryInjectorBuilder, IReplaceRule>[];

			var builder = new QueryInjectorBuilder();
			var encounterObjs = replaceRules.Select(encounterFunc => encounterFunc(builder)).Concat(_replaceRulesContext.Peek());

			_replaceRulesContext.Push(new List<IReplaceRule>(encounterObjs));
			var returnExpression = Visit(node.Arguments[0]);
			_replaceRulesContext.Pop();

			return returnExpression;
		}

		private Expression TranslateInjectWithCall(MethodCallExpression node)
		{
			var keyExpr = node.Arguments[1] as ConstantExpression;
			if (keyExpr == null)
				throw new InvalidOperationException($"Key must be a Constant Expression, but found ${node.Arguments[1].GetType()}");

			_injectedItems.Push(new InjectedItem { Key = keyExpr.Value, Value = node.Arguments[2] });
			return Visit(node.Arguments[0]);
		}

		private Expression TranslateInjectPlaceholder(MethodCallExpression node)
		{
			var keyExpr = node.Arguments[0] as ConstantExpression;
			if (keyExpr == null)
				throw new InvalidOperationException("Encountered an inject placeholder, but value was not a Constant Expression");

			var matchingInject = _injectedItems.FirstOrDefault(ii => ii.Key == keyExpr.Value);
			if (matchingInject == null)
				throw new InvalidOperationException($"Encountered an inject placeholder, but no value was provided for {keyExpr.Value}");

			var constExpr = matchingInject.Value as Expression;
			return constExpr;
		}
	}
}
