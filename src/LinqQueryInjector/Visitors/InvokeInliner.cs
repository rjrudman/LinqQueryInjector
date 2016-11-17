using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqQueryInjector.Visitors
{
	public class InvokeInliner : ExpressionVisitor
	{
		private readonly Stack<Dictionary<ParameterExpression, Expression>> _context = new Stack<Dictionary<ParameterExpression, Expression>>();
		public Expression Inline(Expression expression)
		{
			return Visit(expression);
		}

		protected override Expression VisitInvocation(InvocationExpression e)
		{
			var callingLambda = e.Expression as LambdaExpression;
			if (callingLambda == null)
				return base.VisitInvocation(e);
			var currentMapping = new Dictionary<ParameterExpression, Expression>();
			for (var i = 0; i < e.Arguments.Count; i++)
			{
				var argument = Visit(e.Arguments[i]);
				var parameter = callingLambda.Parameters[i];
				if (parameter != argument)
					currentMapping.Add(parameter, argument);
			}
			_context.Push(currentMapping);
			var result = Visit(callingLambda.Body);
			_context.Pop();
			return result;
		}

		protected override Expression VisitParameter(ParameterExpression e)
		{
			if (_context.Count > 0)
			{
				var currentMapping = _context.Peek();
				if (currentMapping.ContainsKey(e))
					return currentMapping[e];
			}
			return e;
		}
	}
}
