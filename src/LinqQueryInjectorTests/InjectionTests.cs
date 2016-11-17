using System.Linq;
using NUnit.Framework;

namespace LinqQueryInjector
{
	[TestFixture]
    public class InjectionTests
    {
		[Test]
	    public void TestBasicInjectionint()
		{
			QueryInjector.RegisterGlobal(
					ib => ib.WhenEncountering<int>().ReplaceWith(q => q.Where(i => i % 2 == 0))
				);

			var query = new [] {1, 2, 3, 4, 5}.AsInjectableQueryable();

			Assert.True(new[] { 2, 4 }.SequenceEqual(query));
		}

		[Test]
		public void TestBasicInjectionString()
		{
			QueryInjector.RegisterGlobal(
					ib => ib.WhenEncountering<string>().ReplaceWith(s => new [] {"replaced!"}.AsQueryable())
				);

			var query = new[] {"a", "b", "c"}.AsInjectableQueryable().ToList();

			Assert.True(new[] { "replaced!" }.SequenceEqual(query));
		}
	}
}
