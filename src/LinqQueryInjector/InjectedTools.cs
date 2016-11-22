namespace LinqQueryInjector
{
    public static class Inject<TValueType>
	{
	    public static TValueType Value<TKeyType>(TKeyType injectionKey)
	    {
		    return default(TValueType);
	    }
    }
}
