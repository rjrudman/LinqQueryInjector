using System;
using System.Linq;
using System.Linq.Expressions;
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
		public void TestBasicInjectionInt()
		{
			QueryInjector.RegisterGlobal(
					ib => ib.WhenEncountering<IQueryable<int>>().ReplaceWith(q => q.Where(i => i % 2 == 0))
				);

			var collection = new[] { 1, 2, 3, 4, 5 }.AsQueryable();
			var actualQuery = collection.AsInjectableQueryable() as IInjectableQueryable;
			var expected = collection.Where(i => i % 2 == 0);

			Assert.NotNull(actualQuery);
			ExpressionEqualityComparer.AssertExpressionsEqual(expected.Expression, actualQuery.GetInjectedExpression());
		}
		
		[Test]
		public void TestBasicInjectionString()
		{
			Expression<Func<IQueryable<string>, IQueryable<string>>> replaceExpr = s => new[] { "replaced!" }.AsQueryable();
			QueryInjector.RegisterGlobal(
					ib => ib.WhenEncountering<IQueryable<string>>().ReplaceWith(replaceExpr)
				);
			
			var actualQuery = new[] { "a", "b", "c" }.AsInjectableQueryable() as IInjectableQueryable;
			
			Assert.NotNull(actualQuery);
			ExpressionEqualityComparer.AssertExpressionsEqual(replaceExpr.Body, actualQuery.GetInjectedExpression());
		}

		[Test]
		public void TestBasicInjectionStringLiteral()
		{
			Expression<Func<string, string>> replaceExpr = s => "rob";

			QueryInjector.RegisterGlobal(
					ib => ib.WhenEncountering<string>().ReplaceWith(replaceExpr)
				);

			var initialCollection = new[] { "a", "b", "c" }.AsQueryable();
			var actualQuery = initialCollection.Where(s => s == "rob").AsInjectableQueryable() as IInjectableQueryable;

			//We need to write it this way, because .Where(s => "rob" == "rob") is optimised into .Where(s => true)
			var expression = Expression.Lambda<Func<string, bool>>(Expression.Equal(Expression.Constant("rob"), Expression.Constant("rob")), Expression.Parameter(typeof(string), "s"));
			var expectedQuery = initialCollection.Where(expression);
			
			Assert.NotNull(actualQuery);
			ExpressionEqualityComparer.AssertExpressionsEqual(expectedQuery.Expression, actualQuery.GetInjectedExpression());
		}

		[Test]
		public void TestInjectionOnQueryable()
		{
			var collection = new[] { "a", "b", "c" }.AsQueryable();
			var actualQuery = collection
				.AsInjectableQueryable()
				.RegisterInject(
					ib => ib.WhenEncountering<IQueryable<string>>().ReplaceWith(strings => strings.Where(s => s == "a"))
				) as IInjectableQueryable;

			var expected = collection.Where(s => s == "a");

			Assert.NotNull(actualQuery);
			ExpressionEqualityComparer.AssertExpressionsEqual(expected.Expression, actualQuery.GetInjectedExpression());
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

			var expected = initialCollection.Where(i => i % 2 == 0).Where(i => i % 2 == 0).Concat(secondCollection).Where(i => i % 2 == 0);

			Assert.NotNull(actualQuery);
			ExpressionEqualityComparer.AssertExpressionsEqual(expected.Expression, actualQuery.GetInjectedExpression());
		}

		private class Student
		{
			public bool Deleted { get; set; }
		}

		[Test]
		public void TestObjectFilters()
		{
			QueryInjector.RegisterGlobal(
				ib => ib.WhenEncountering<IQueryable<Student>>().ReplaceWith(students => students.Where(student => !student.Deleted))
			);

			var collection = new[]
			{
				new Student {Deleted = false},
				new Student {Deleted = true},
				new Student {Deleted = false},
				new Student {Deleted = true},
				new Student {Deleted = false},
			}.AsQueryable();

			var actualQuery = collection.AsInjectableQueryable() as IInjectableQueryable;
			var expected = collection.Where(student => !student.Deleted);

			Assert.NotNull(actualQuery);
			ExpressionEqualityComparer.AssertExpressionsEqual(expected.Expression, actualQuery.GetInjectedExpression());
		}
	}
}
