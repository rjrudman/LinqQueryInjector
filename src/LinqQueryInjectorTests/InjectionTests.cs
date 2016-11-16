using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace LinqQueryInjector
{
	[TestFixture]
    public class InjectionTests
    {
		[Test]
	    public void TestBasicInjection()
		{

			QueryInjector.WhenEncountering<IQueryable<int>>().ReplaceWith(coll => coll.Where(i => i % 2 == 0));
		}
    }
}
