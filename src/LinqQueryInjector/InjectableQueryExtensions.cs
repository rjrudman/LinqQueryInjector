using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqQueryInjector.Builders;
using LinqQueryInjector.Helpers;

namespace LinqQueryInjector
{
    public static class InjectableQueryExtensions
    {
	    public static IQueryable<T> AsInjectableQueryable<T>(this IQueryable<T> source)
	    {
		    if (source is InjectableQueryable<T>)
			    return source;
			return new InjectableQueryable<T>(source);
	    }

		public static IQueryable AsInjectableQueryable(this IQueryable source)
		{
			if (source is InjectableQueryable)
				return source;
			return new InjectableQueryable(source);
		}

		public static IQueryable<T> AsInjectableQueryable<T>(this IEnumerable<T> source)
		{
			var injectableQueryable = source as InjectableQueryable<T>;
			if (injectableQueryable != null)
				return injectableQueryable;

			return source.AsQueryable().AsInjectableQueryable();
		}

		public static IQueryable AsInjectableQueryable(this IEnumerable source)
		{
			var injectableQueryable = source as InjectableQueryable;
			if (injectableQueryable != null)
				return injectableQueryable;

			return source.AsQueryable().AsInjectableQueryable();
		}
		
		public static IQueryable<T> RegisterInject<T>(this IQueryable<T> source, params Func<IQueryInjectorBuilder, IReplaceRule>[] replaceRules)
		{
			if (replaceRules == null)
				throw new ArgumentNullException(nameof(replaceRules));

			return source.Provider.CreateQuery<T>(Expression.Call(
				null,
				MethodHelper.GetMethodInfoOf(() => RegisterInject(
					default(IQueryable<T>),
					default(Func<IQueryInjectorBuilder, IReplaceRule>[]))),
					new[] { source.Expression, Expression.Constant(replaceRules) }
				));
		}

		public static IQueryable RegisterInject(this IQueryable source, params Func<IQueryInjectorBuilder, IReplaceRule>[] replaceRules)
		{
			if (replaceRules == null)
				throw new ArgumentNullException(nameof(replaceRules));

			return source.Provider.CreateQuery(Expression.Call(
				null,
				MethodHelper.GetMethodInfoOf(() => RegisterInject(
					default(IQueryable),
					default(Func<IQueryInjectorBuilder, IReplaceRule>[]))),
					new[] { source.Expression, Expression.Constant(replaceRules) }
				));
		}

	    public static IQueryable<T> InjectWith<T, TKeyType, TResultType>(this IQueryable<T> source, TKeyType key, TResultType value)
	    {
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (value == null)
				throw new ArgumentNullException(nameof(value));

		    return source.Provider.CreateQuery<T>(Expression.Call(
			    null,
			    MethodHelper.GetMethodInfoOf(() => InjectWith(
				    default(IQueryable<T>),
					default(TKeyType),
				    default(TResultType))),
			    new[] {source.Expression, Expression.Constant(key, typeof(TKeyType)), Expression.Constant(value, typeof(TResultType)) }
		    ));
	    }

		public static IQueryable InjectWith<TKeyType, TResultType>(this IQueryable source, TKeyType key, TResultType value)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			return source.Provider.CreateQuery(Expression.Call(
				null,
				MethodHelper.GetMethodInfoOf(() => InjectWith(
					default(IQueryable),
					default(TKeyType),
					default(TResultType))),
				new[] { source.Expression, Expression.Constant(key, typeof(TKeyType)), Expression.Constant(value, typeof(TResultType)) }
			));
		}

	}
}
