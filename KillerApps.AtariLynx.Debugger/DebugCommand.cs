using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillerApps.AtariLynx.Debugger
{
	public enum DebugCommand : byte
	{
		Continue = 0x82,
		ReceiveRegisters = 0x83,
		WriteMemory = 0x84,
		ReadMemory = 0x85,
		SendRegisters = 0x86,
		Break = 0x8f
	}
}
