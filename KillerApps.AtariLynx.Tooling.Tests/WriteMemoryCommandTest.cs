using KillerApps.AtariLynx.Tooling.Bll;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.Tests
{
    [TestClass]
    public class WriteMemoryMessageTest
    {
        [TestMethod]
        public void CreateWriteSingleByteMessage()
        {
            byte data = 0x42;
            ushort address = 0x1234;
            WriteMemoryDebugMessage message = new WriteMemoryDebugMessage(address, data);
            byte[] messageInBytes = message.ToBytes();

            CollectionAssert.AreEqual(
                new byte[] { (byte)DebugCommandBytes.WriteMemory, 0x12, 0x34, 0x01, data },
                messageInBytes);
        }

        [TestMethod]
        public void CreateWriteWordMessage()
        {
            ushort data = 0x4243;
            ushort address = 0x1234;
            WriteMemoryDebugMessage message = new WriteMemoryDebugMessage(address, data);
            byte[] messageInBytes = message.ToBytes();

            CollectionAssert.AreEqual(
                new byte[] { (byte)DebugCommandBytes.WriteMemory, 0x12, 0x34, 0x02, 0x42, 0x43 },
                messageInBytes);
        }
    }
}
