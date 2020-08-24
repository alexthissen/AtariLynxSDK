using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.Bll
{
    public enum DebugCommandBytes: byte
    {
        Header = 0x81,
        Continue = 0x82,
        SetRegisters = 0x83,
        WriteMemory = 0x84,
        ReadMemory = 0x85,
        SendRegisters = 0x86
    }

    /* From debug.inc:

    dc.w Continue		; $82
	dc.w BRKSetRegisters	; $83,A,X,Y,S,P,PC
	dc.w BRKWriteMem	; $84,addr,n,...
	dc.w BRKReadMem		; $85,addr,n
	dc.w BRKSndRegisters	; $86 
    */
}
