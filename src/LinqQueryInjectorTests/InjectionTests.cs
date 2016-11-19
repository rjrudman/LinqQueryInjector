using System.Linq;
using NUnit.Framework;

namespace LinqQueryInjector
{
	[TestFixture]
    public class InjectionTests
    {
		[SetUp]
		public void Setup()
		{
			QueryInjector.ClearGlobals();
		}

		[Test]
	    public void TestBasicInjectionint()
		{
			QueryInjector.RegisterGlobal(
					ib => ib.WhenEncountering<IQueryable<int>>().ReplaceWith(q => q.Where(i => i % 2 == 0))
				);

			var query = new [] {1, 2, 3, 4, 5}.AsInjectableQueryable();

			Assert.True(new[] { 2, 4 }.SequenceEqual(query));
		}

		[Test]
		public void TestBasicInjectionString()
		{
			QueryInjector.RegisterGlobal(
					ib => ib.WhenEncountering<IQueryable<string>>().ReplaceWith(s => new [] {"replaced!"}.AsQueryable())
				);

			var query = new[] {"a", "b", "c"}.AsInjectableQueryable().ToList();

			Assert.True(new[] { "replaced!" }.SequenceEqual(query));
		}

		[Test]
		public void TestBasicInjectionStringLiteral()
		{
			QueryInjector.RegisterGlobal(
					ib => ib.WhenEncountering<string>().ReplaceWith(s => "rob")
				);

			var query = new[] { "a", "b", "c" }.AsQueryable().Where(s => s == "rob").AsInjectableQueryable().ToList();

			Assert.True(new[] { "a", "b", "c" }.SequenceEqual(query));
		}
		
		[Test]
		public void TestInjectionOnQueryable()
		{
			var query = new[] { "a", "b", "c" }
				.AsInjectableQueryable()
				.RegisterInject(
					ib => ib.WhenEncountering<IQueryable<string>>().ReplaceWith(strings => strings.Where(s => s == "a"))
				)
				.ToList();

			Assert.True(new[] { "a" }.SequenceEqual(query));
		}


	    [Test]
	    public void TestInjectionScoped()
	    {
		    var query = new[] {1, 2, 3}
			    .AsInjectableQueryable()
			    .RegisterInject(ib => ib.WhenEncountering<IQueryable<int>>().ReplaceWith(i => i.Where(ii => ii % 2 == 0)))
			    .Concat(new[] {5});

		    Assert.True(new[] {2, 5}.SequenceEqual(query));
	    }

		[Test]
		public void TestInjectionScoped2()
		{
			var query = new[] { 1, 2, 3 }
				.AsInjectableQueryable()
				.RegisterInject(ib => ib.WhenEncountering<IQueryable<int>>().ReplaceWith(i => i.Where(ii => ii % 2 == 0)))
				.Concat(new[] { 5 })
				.RegisterInject(ib => ib.WhenEncountering<IQueryable<int>>().ReplaceWith(i => i.Where(ii => ii % 2 == 0)));

			Assert.True(new[] { 2 }.SequenceEqual(query));
		}

		[Test]
		public void TestChainingLINQ()
		{
			QueryInjector.RegisterGlobal(
					ib => ib.WhenEncountering<string>().ReplaceWith(s => "rob")
				);

			var query = new[] { "a", "b", "c" }.AsInjectableQueryable().Where(s => s == "rob").ToList();

			Assert.True(new[] { "a", "b", "c" }.SequenceEqual(query));
		}
		
		private class Student
	    {
		    public string Name { get; set; }
		    public bool Deleted { get; set; }
	    }
		
	    [Test]
		public void TestObjectFilters()
		{
			QueryInjector.RegisterGlobal(
				ib => ib.WhenEncountering<IQueryable<Student>>().ReplaceWith(students => students.Where(student => !student.Deleted))
			);

			var myStudentSet = new[]
			{
				new Student {Name = "A", Deleted = false},
				new Student {Name = "B", Deleted = true},
				new Student {Name = "C", Deleted = false},
				new Student {Name = "D", Deleted = true},
				new Student {Name = "E", Deleted = false},
			}.AsInjectableQueryable();
			
			Assert.AreEqual(new [] { "A", "C", "E"}, myStudentSet.Select(s => s.Name));
		}
	}
}
