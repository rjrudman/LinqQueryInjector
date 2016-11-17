using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqQueryInjector.Builders
{
	public class ReplaceRule : IReplaceRule
	{
		public Type ReplaceType { get; protected set; }
		public bool IsRequired { get; protected set; }
		public Expression ReplaceWithExpr { get; protected set; }
	}

	public class ReplaceRule<TEncounteredType> : ReplaceRule, IReplaceRule<TEncounteredType>
	{
		public ReplaceRule(Expression<Func<IQueryable<TEncounteredType>, IQueryable<TEncounteredType>>> expr)
		{
			ReplaceType = typeof(IQueryable<TEncounteredType>);
			ReplaceWithExpr = expr;
		}

		public IReplaceRule<TEncounteredType> Required(bool isRequired)
		{
			IsRequired = isRequired;
			return this;
		}
	}

}
