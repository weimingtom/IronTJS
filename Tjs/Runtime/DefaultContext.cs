﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronTjs.Runtime
{
	public static class DefaultContext
	{
		static TjsContext _default;

		public static TjsContext DefaultTjsContext { get { return _default; } }

		internal static void InitializeDefaults(TjsContext context)
		{
			if (_default == null)
				System.Threading.Interlocked.CompareExchange(ref _default, context, null);
		}
	}
}
