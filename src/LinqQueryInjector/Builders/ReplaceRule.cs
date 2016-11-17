﻿using System;
using System.Linq.Expressions;

namespace LinqQueryInjector.Builders
{
	public class ReplaceRule : IReplaceRule
	{
		public Type ReplaceType { get; protected set; }
		public bool IsRequired { get; protected set; }
		public Expression ReplaceWithExpr { get; protected set; }
	}

	public class ReplaceRule<TEncounteredType, TReturnType> : ReplaceRule, IReplaceRule<TEncounteredType>
	{
		public ReplaceRule(Expression<Func<TEncounteredType, TReturnType>> expr)
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