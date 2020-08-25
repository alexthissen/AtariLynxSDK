using System;

namespace KillerApps.AtariLynx.Tooling.Bll
{
    public class ScreenshotDebugCommand : IBllDebugCommand
    {
        public byte[] ToBytes()
        {
            /*
             *  Excerpt from the BLL documentation:-
             *  ------------------------------------
             *  start-sequence  : $81,"P"                       ; command : load program
             *  init-sequence   : LO(Start),HI(Start)           ;start address = dest.
             *  LO((Len-10) XOR $FFFF),HI((Len-10) XOR $FFFF)   ; and len of data
             *  Xmit-sequence : ....                            ; data
             *  checksum : none at all !! 
            */

            byte[] bytes = new byte[2] { 
                (byte)DebugCommandBytes.Header, 
                (byte)'S'
            };
            return bytes;
        }
    }
}
