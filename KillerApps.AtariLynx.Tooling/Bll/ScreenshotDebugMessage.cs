using System;

namespace KillerApps.AtariLynx.Tooling.Bll
{
	public class ScreenshotDebugMessage : IBllDebugMessage
	{
		public byte[] ToBytes()
		{
			byte[] bytes = new byte[2] {
				(byte)DebugCommandBytes.Header,
				(byte)'S'
			};
			return bytes;
		}
	}
}

/*
****************
* send the current screen
* via comlynx
****************
IFD ScreenBase
	screen_to_comlynx::
		phy
		ldx #31
.loopc  lda $fda0, x
			jsr SendByte
			dex
		bpl.loopc  ; colors first
		ldy #102
	;-- send screen with Y lines
IF DBUFuser=1		; send current screen !!
		MOVE ScreenBase2, DebugPtr
ELSE
		MOVE ScreenBase,DebugPtr
ENDIF

.loopy  ldx #79
.loopx		lda (DebugPtr)
			inc DebugPtr
			bne .cont
			inc DebugPtr+1
.cont		jsr SendByte
			dex
		bpl.loopx
		dey
		bne.loopy
		jmp Start   ; reset
ENDIF
*/
