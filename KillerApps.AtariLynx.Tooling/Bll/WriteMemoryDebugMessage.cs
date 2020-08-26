using System;

namespace KillerApps.AtariLynx.Tooling.Bll
{
    public class WriteMemoryDebugMessage : IBllDebugMessage
    {
        public byte ByteLength { get; private set; }
        public byte[] Data { get; private set; }
        public ushort Address { get; private set; }

        private void AllocateDataMemory(byte length)
        {
            ByteLength = length;
            Data = new byte[ByteLength];
        }

        public WriteMemoryDebugMessage(ushort address, byte data) : 
            this(address, new byte[] { data }, 0, 1) { }

        public WriteMemoryDebugMessage(ushort address, ushort data) :
            this(address, new byte[] { (byte)(data >> 0x8), (byte)(data & 0xff) }, 0, 2)
        { }

        public WriteMemoryDebugMessage(ushort address, byte[] data, int offset, int length) 
        {
            if (length == 0) length = data.Length - offset;
            if (length > 0xff)
                throw new ArgumentException("Data length cannot exceed 255");
            AllocateDataMemory((byte)length);
            Address = address;
            Array.Copy(data, offset, Data, 0, ByteLength);
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[ByteLength + 4];
            bytes[0] = (byte)DebugCommandBytes.WriteMemory;
            bytes[1] = (byte)(Address >> 8);
            bytes[2] = (byte)(Address & 0xff);
            bytes[3] = ByteLength;
            Array.Copy(Data, 0, bytes, 4, ByteLength);
            return bytes;
        }
    }
}
