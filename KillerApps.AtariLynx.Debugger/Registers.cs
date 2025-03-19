using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace KillerApps.AtariLynx.Debugger
{
    public struct Registers
    {
        public ushort PC;
        public byte SP;
        public byte PS, Y, X, A;

        public override string ToString()
        {
            return string.Format("A:{0:X2} X:{1:X2} Y:{2:X2} PS:{3:X2} PC:{4:X4} SP:{5:X2}", A, X, Y, PS, PC, SP);
        }

        public static Registers FromBytes(byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Registers registers = (Registers)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Registers));
            handle.Free();
            registers.PC = BinaryPrimitives.ReverseEndianness(registers.PC);
            return registers;
        }
    }
}
