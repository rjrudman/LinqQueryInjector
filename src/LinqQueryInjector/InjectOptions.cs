using System;
using System.Linq.Expressions;

namespace LinqQueryInjector
{
    public class InjectOptions
    {
		public Type NodeType { get; set; }
		public Expression ReplaceWith { get; set; }
    }
}
