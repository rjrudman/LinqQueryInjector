using System.Linq;
using NUnit.Framework;

namespace LinqQueryInjector
{
    public class BasicQueryableTests
    {
	    [Test]
	    public void CanIterateEnumerable()
	    {
		    var expr = new[] {1, 2, 3, 4, 5}
			    .Where(i => i % 2 == 0)
				.AsQueryable();
			
			var query = new InjectableQueryable<int>(expr);

		    Assert.True(new[] {2, 4}.SequenceEqual(query.ToList()));
	    }

		[Test]
		public void CanIterateEnumerableQuery()
		{
			var expr = new EnumerableQuery<int>(new[] { 1, 2, 3, 4, 5 })
				.Where(i => i % 2 == 0)
				.AsQueryable();

			var query = new InjectableQueryable<int>(expr);

			Assert.True(new[] { 2, 4 }.SequenceEqual(query.ToList()));
		}
	}
}
