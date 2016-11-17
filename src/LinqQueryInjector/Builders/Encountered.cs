using System;
using System.Linq.Expressions;

namespace LinqQueryInjector.Builders
{
	public class Encountered<TEncounteredType> : IEncountered<TEncounteredType>
	{
		public IReplaceRule<TEncounteredType> ReplaceWith<TReturnType>(Expression<Func<TEncounteredType, TReturnType>> expr)
		{
			return new ReplaceRule<TEncounteredType, TReturnType>(expr);
		}
	}
}
