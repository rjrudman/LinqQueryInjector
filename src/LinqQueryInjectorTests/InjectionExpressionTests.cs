using System.Linq;
using ExpressionComparer;
using NUnit.Framework;

namespace LinqQueryInjector
{
	[TestFixture]
    public class InjectionExpressionTests
    {
		[SetUp]
		public void Setup()
		{
			QueryInjector.ClearGlobals();
		}

		[Test]
		public void TestInjectionScoped()
		{
			var initialCollection = new[] { 1, 2, 3 }.AsQueryable();
			var secondCollection = new[] { 5 };

			var actualQuery = initialCollection
				.AsInjectableQueryable()
				.RegisterInject(ib => ib.WhenEncountering<IQueryable<int>>().ReplaceWith(collection => collection.Where(i => i % 2 == 0)))
				.Concat(secondCollection) as IInjectableQueryable;
			
			var expected = initialCollection.Where(i => i % 2 == 0).Concat(secondCollection);

			Assert.NotNull(actualQuery);
			ExpressionEqualityComparer.AssertExpressionsEqual(expected.Expression, actualQuery.GetInjectedExpression());
		}

		[Test]
		public void TestInjectionScoped2()
		{
			var initialCollection = new[] { 1, 2, 3 }.AsQueryable();
			var secondCollection = new[] { 5 };

			var actualQuery = initialCollection
				.AsInjectableQueryable()
				.RegisterInject(ib => ib.WhenEncountering<IQueryable<int>>().ReplaceWith(collection => collection.Where(i => i % 2 == 0)))
				.Concat(secondCollection)
				.RegisterInject(ib => ib.WhenEncountering<IQueryable<int>>().ReplaceWith(collection => collection.Where(i => i % 2 == 0)))
				 as IInjectableQueryable;

			var expected = initialCollection.Where(i => i % 2 == 0).Concat(secondCollection).Where(i => i % 2 == 0);

			Assert.NotNull(actualQuery);
			ExpressionEqualityComparer.AssertExpressionsEqual(expected.Expression, actualQuery.GetInjectedExpression());
		}
	}
}
