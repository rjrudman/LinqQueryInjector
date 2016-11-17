using System.Linq;
using NUnit.Framework;

namespace LinqQueryInjector
{
    public class BasicQueryableTests
    {
	    [Test]
	    public void CanIterateEnumerable()
	    {
		    var query = new[] {1, 2, 3, 4, 5}
			    .Where(i => i % 2 == 0)
				.AsInjectableQueryable();
			
		    Assert.True(new[] {2, 4}.SequenceEqual(query.ToList()));
	    }

		[Test]
		public void CanIterateEnumerableQuery()
		{
			var query = new EnumerableQuery<int>(new[] { 1, 2, 3, 4, 5 })
				.Where(i => i % 2 == 0)
				.AsInjectableQueryable();
			
			Assert.True(new[] { 2, 4 }.SequenceEqual(query.ToList()));
		}
	}
}
