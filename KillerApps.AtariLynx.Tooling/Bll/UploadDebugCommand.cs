using System;

namespace KillerApps.AtariLynx.Tooling.Bll
{
    public class UploadDebugCommand : IBllDebugCommand
    {
        public ushort LoadAddress { get; private set; }
        public ushort Length { get; private set; }

        public UploadDebugCommand(ushort loadAddress, ushort length)
        {
            this.LoadAddress = loadAddress;
            this.Length = length;
        }

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

            byte[] bytes = new byte[6] { 
                (byte)DebugCommandBytes.Header, 
                (byte)'P',
                (byte)(LoadAddress >> 8), (byte)(LoadAddress & 0xff), // First HI, then LO seems to work
                (byte)((Length >> 8) ^ 0xff),
                (byte)((Length & 0xff) ^ 0xff)
            };
            return bytes;
        }
    }
}
