using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	}
}
