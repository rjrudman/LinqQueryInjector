using System;
using System.Linq.Expressions;

namespace LinqQueryInjector.Builders
{
	public interface IReplaceRule
	{
		Type ReplaceType { get; }
		ExpressionType[] ExpressionTypes { get; }
		Func<Expression, bool> AcceptExpression { get; }
		bool IsRequired { get; }
		Expression ReplaceWithExpr { get; }
	}
	public interface IReplaceRule<TEncounteredType> : IReplaceRule
	{
		IReplaceRule<TEncounteredType> Required(bool isRequired);
	}
}
