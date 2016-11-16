using System;

namespace LinqQueryInjector
{
	public interface IEncountered<out TEncounteredType>
	{
		void ReplaceWith<TReturnType>(Func<TEncounteredType, TReturnType> expr);
	}

	public class Encountered<TEncounteredType> : IEncountered<TEncounteredType>
	{
		public void ReplaceWith<TReturnType>(Func<TEncounteredType, TReturnType> expr)
		{
			
		}
	}
	public static class QueryInjector
    {
		public static IEncountered<T> WhenEncountering<T>()
		{
			return new Encountered<T>();
		}
    }
}
