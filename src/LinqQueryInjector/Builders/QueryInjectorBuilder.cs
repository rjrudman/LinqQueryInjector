namespace LinqQueryInjector.Builders
{
	public class QueryInjectorBuilder : IQueryInjectorBuilder
	{
		public IEncountered<T> WhenEncountering<T>()
		{
			return new Encountered<T>();
		}
	}
}
