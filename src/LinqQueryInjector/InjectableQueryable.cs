using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqQueryInjector
{
	public interface IInjectableQueryable
	{
		Expression GetInjectedExpression();
	}

	internal class InjectableQueryable<T> : InjectableQueryable, IQueryable<T>
	{
		private IEnumerable<T> WrappedTypeEnumerable => (IEnumerable<T>) WrappedQueryable;
		public InjectableQueryable(IQueryable<T> queryable) : base(queryable) { }
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			InjectQuery(Expression);
			return WrappedTypeEnumerable.GetEnumerator();
		}
	}
	internal class InjectableQueryable : IQueryable, IQueryProvider, IInjectableQueryable
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
			return (IQueryable<TElement>) CreateTypedInjectableQueryable(expression, true);
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

		Expression IInjectableQueryable.GetInjectedExpression() => InjectQuery(Expression);

		IEnumerator IEnumerable.GetEnumerator()
		{
			InjectQuery(Expression);
			return WrappedEnumerable.GetEnumerator();
		}
		
		internal Expression InjectQuery(Expression expr)
		{
			if (RequiresInjection)
			{
				var visitor = new InjectableQueryVisitor(QueryInjector.ReplaceRules);
				expr = visitor.Visit(expr);
				WrappedQueryable = CreateTypedInjectableQueryable(expr, false);
			}
				

			RequiresInjection = false;
			return expr;
		}

		private IQueryable CreateTypedInjectableQueryable(Expression expr, bool requiresInjection)
		{
			var wrappedQueryable = WrappedProvider.CreateQuery(expr);
			var result = (InjectableQueryable)Activator.CreateInstance(typeof(InjectableQueryable<>).MakeGenericType(wrappedQueryable.ElementType), wrappedQueryable);
			result.RequiresInjection = requiresInjection;
			return result;
		}
	}
}
