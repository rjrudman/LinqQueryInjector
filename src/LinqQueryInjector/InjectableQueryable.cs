using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqQueryInjector
{
	public class InjectableQueryable<T> : InjectableQueryable, IQueryable<T>
	{
		private IEnumerable<T> WrappedTypeEnumerable => (IEnumerable<T>) WrappedQueryable;
		public InjectableQueryable(IQueryable<T> queryable) : base(queryable) { }
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			InjectQuery(Expression);
			return WrappedTypeEnumerable.GetEnumerator();
		}
	}
	public class InjectableQueryable : IQueryable, IQueryProvider
	{
		protected IQueryable WrappedQueryable;
		private IEnumerable WrappedEnumerable => WrappedQueryable;
		private IQueryProvider WrappedProvider => WrappedQueryable.Provider;
		protected bool RequiresInjection = true;

		public InjectableQueryable(IQueryable queryable)
		{
			WrappedQueryable = queryable;
		}

		public Type ElementType => WrappedQueryable.ElementType;
		public Expression Expression => WrappedQueryable.Expression;
		public IQueryProvider Provider => this;

		public IQueryable CreateQuery(Expression expression)
		{
			var wrappedQueryable = WrappedProvider.CreateQuery(expression);
			return new InjectableQueryable(wrappedQueryable);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return (IQueryable<TElement>) CreateTypedInjectableQueryable(expression);
		}

		public object Execute(Expression expression)
		{
			expression = InjectQuery(expression);
			return WrappedProvider.Execute(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			expression = InjectQuery(expression);
			return WrappedProvider.Execute<TResult>(expression);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			InjectQuery(Expression);
			return WrappedEnumerable.GetEnumerator();
		}
		
		protected Expression InjectQuery(Expression expr)
		{
			if (RequiresInjection)
				WrappedQueryable = CreateTypedInjectableQueryable(expr);

			RequiresInjection = false;
			return expr;
		}

		private IQueryable CreateTypedInjectableQueryable(Expression expr)
		{
			var wrappedQueryable = WrappedProvider.CreateQuery(expr);
			var result = (InjectableQueryable)Activator.CreateInstance(typeof(InjectableQueryable<>).MakeGenericType(wrappedQueryable.ElementType), wrappedQueryable);
			result.RequiresInjection = false;
			return result;
		}
	}
}
