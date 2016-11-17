using System.Linq;
using NUnit.Framework;

namespace LinqQueryInjector
{
	[TestFixture]
    public class InjectionTests
    {
		[Test]
	    public void TestBasicInjection()
		{
			QueryInjector.RegisterGlobal(
					ib => ib.WhenEncountering<IQueryable<int>>().ReplaceWith(q => q.Where(i => i % 2 == 0)),
					ib => ib.WhenEncountering<IQueryable<string>>().ReplaceWith(q => q.Where(s => s == "Rob"))
				);
		}
    }
}
