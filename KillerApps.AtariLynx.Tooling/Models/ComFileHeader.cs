using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.Models
{
    public struct ComFileHeader
    {
        public const int HEADER_SIZE = 10; 
        public const string BS93_SIGNATURE = "BS93";

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] MagicBytes;
        private ushort LoadAddressRaw;
        private ushort ObjectSizeRaw;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Bs93Signature;        

        public ComFileHeader(ushort loadAddress, ushort objectSize)
        {
            MagicBytes = new byte[] { 0x80, 0x08 };
            LoadAddressRaw = BinaryPrimitives.ReverseEndianness(loadAddress);
            ObjectSizeRaw = BinaryPrimitives.ReverseEndianness(objectSize);
            Bs93Signature = Encoding.ASCII.GetBytes(BS93_SIGNATURE);
        }

        public ushort LoadAddress
        {
            get { return BinaryPrimitives.ReverseEndianness(LoadAddressRaw); }
            set { LoadAddressRaw = BinaryPrimitives.ReverseEndianness(value); }
        }
        public ushort ObjectSize
        {
            get { return BinaryPrimitives.ReverseEndianness(ObjectSizeRaw); }
            set { ObjectSizeRaw = BinaryPrimitives.ReverseEndianness(value); }
        }

        public static ComFileHeader FromBytes(byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            ComFileHeader header = (ComFileHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(ComFileHeader));
            handle.Free();
            return header;
        }

        public byte[] ToBytes()
        {
            int size = Marshal.SizeOf<ComFileHeader>();
            byte[] data = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);
            return data;
        }
    }
}
