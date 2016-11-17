namespace LinqQueryInjector.Builders
{
	public interface IReplaceRule { }
	public interface IReplaceRule<TEncounteredType> : IReplaceRule
	{
		IReplaceRule<TEncounteredType> Required(bool isRequired);
	}
}
