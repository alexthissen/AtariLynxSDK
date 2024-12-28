using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillerApps.AtariLynx.Debugger
{
	public class BreakRequest : DebugRequest
	{
		public BreakRequest() : base(DebugCommand.Break) { }
        public override int ResponseLength { get { return 8; } }
    }
}
