using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KillerApps.AtariLynx.Debugger
{
	public class DebugResponseReceivedEventArgs
	{
		public IDebugResponse Response { get; private set; }

		public DebugResponseReceivedEventArgs(IDebugResponse response)
		{
			this.Response = response;
		}
	}
}
