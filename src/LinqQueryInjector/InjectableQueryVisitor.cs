using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqQueryInjector.Builders;
using LinqQueryInjector.Visitors;

namespace LinqQueryInjector
{
    internal class InjectableQueryVisitor : ExpressionVisitor
	{
	    private readonly IEnumerable<IReplaceRule> _replaceRules;

	    internal InjectableQueryVisitor(IEnumerable<IReplaceRule> replaceRules)
	    {
		    _replaceRules = replaceRules;
	    }

		public override Expression Visit(Expression node)
		{
			if (node == null)
				return null;
			
			var match = _replaceRules.FirstOrDefault(rr => rr.ReplaceType.GetTypeInfo().IsAssignableFrom(node.Type));
			if (match != null)
			{
				var result = Expression.Invoke(match.ReplaceWithExpr, node);
				var secondResult = new InvokeInliner().Inline(result);
				return secondResult;
			}

			return base.Visit(node);
		}

		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			//We don't want to visit the parameter definitions
			var bodyResult = Visit(node.Body);
			return Expression.Lambda(bodyResult, node.Parameters);
		}
	}
}
