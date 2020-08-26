using System;

namespace KillerApps.AtariLynx.Tooling.Bll
{
    public class ReadMemoryDebugMessage : IBllDebugMessage
    {
        public byte ByteLength { get; private set; }
        public ushort Address { get; private set; }

        public ReadMemoryDebugMessage(ushort address, byte length) 
        { 
            if (length == 0) throw new ArgumentException("Data length cannot be zero");
            ByteLength = length;
            Address = address;
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)DebugCommandBytes.ReadMemory;
            bytes[1] = (byte)(Address >> 8);
            bytes[2] = (byte)(Address & 0xff);
            bytes[3] = ByteLength;
            return bytes;
        }
    }
}
