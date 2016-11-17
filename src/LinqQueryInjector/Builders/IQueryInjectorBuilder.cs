namespace LinqQueryInjector.Builders
{
	public interface IQueryInjectorBuilder
	{
		IEncountered<T> WhenEncountering<T>();
	}

}
