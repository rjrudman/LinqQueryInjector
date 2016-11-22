using System;
using System.Linq.Expressions;

namespace LinqQueryInjector.Builders
{
	public class ReplaceRule : IReplaceRule
	{
		public Func<Expression, bool> AcceptExpression { get { return null; } }
		public ExpressionType[] ExpressionTypes
		{
			get { return new[] {ExpressionType.Constant, ExpressionType.Parameter, ExpressionType.Constant, ExpressionType.NewArrayInit}; }
		}

		public Type ReplaceType { get; protected set; }
		public bool IsRequired { get; protected set; }
		public Expression ReplaceWithExpr { get; protected set; }
	}

	public class ReplaceRule<TEncounteredType> : ReplaceRule, IReplaceRule<TEncounteredType>
	{
		public ReplaceRule(Expression<Func<TEncounteredType, TEncounteredType>> expr)
		{
			ReplaceType = typeof(TEncounteredType);
			ReplaceWithExpr = expr;
		}

		public IReplaceRule<TEncounteredType> Required(bool isRequired)
		{
			IsRequired = isRequired;
			return this;
		}
	}

}
