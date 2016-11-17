﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqQueryInjector.Builders
{
	public interface IEncountered<TEncounteredType>
	{
		IReplaceRule<TEncounteredType> ReplaceWith(Expression<Func<IQueryable<TEncounteredType>, IQueryable<TEncounteredType>>> expr);
	}
}
