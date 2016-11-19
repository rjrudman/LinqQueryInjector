using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqQueryInjector.Helpers
{
    public static class MethodHelper
    {
		public static MethodInfo GetMethodInfoOf<T>(Expression<Func<T>> expression)
		{
			var body = (MethodCallExpression)expression.Body;
			return body.Method;
		}
	}
}
