using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillerApps.AtariLynx.Debugger
{
	public class SendRegistersRequest : DebugRequest
	{
		public SendRegistersRequest(): base(DebugCommand.SendRegisters){ }
		public override int ResponseLength { get { return 7; } }
	}
}
