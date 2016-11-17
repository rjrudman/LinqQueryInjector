using System;
using System.Collections.Generic;
using System.Linq;
using LinqQueryInjector.Builders;

namespace LinqQueryInjector
{
	public static class QueryInjector
    {
		private static readonly List<IReplaceRule> _replaceRules = new List<IReplaceRule>();
	    public static IEnumerable<IReplaceRule> ReplaceRules => _replaceRules;

		public static void RegisterGlobal(params Func<IQueryInjectorBuilder, IReplaceRule>[] replaceRules)
	    {
			var builder = new QueryInjectorBuilder();
		    var encounterObjs = replaceRules.Select(encounterFunc => encounterFunc(builder)).ToList();
			lock(_replaceRules)
				_replaceRules.AddRange(encounterObjs);
	    }

	    public static void ClearGlobals()
	    {
		    lock(_replaceRules)
				_replaceRules.Clear();
	    }
    }
}
