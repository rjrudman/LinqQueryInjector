using System;
using System.Linq.Expressions;

namespace LinqQueryInjector.Builders
{
	public interface IEncountered<TEncounteredType>
	{
		IReplaceRule<TEncounteredType> ReplaceWith<TReturnType>(Expression<Func<TEncounteredType, TReturnType>> expr);
	}
}
