using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqQueryInjector.Builders
{
	public class Encountered<TEncounteredType> : IEncountered<TEncounteredType>
	{
		public IReplaceRule<TEncounteredType> ReplaceWith(Expression<Func<IQueryable<TEncounteredType>, IQueryable<TEncounteredType>>> expr)
		{
			return new ReplaceRule<TEncounteredType>(expr);
		}
	}
}
