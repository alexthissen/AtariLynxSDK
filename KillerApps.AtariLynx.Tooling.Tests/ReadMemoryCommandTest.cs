using KillerApps.AtariLynx.Tooling.Bll;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.Tests
{
    [TestClass]
    public class ReadMemoryMessageTest
    {
        [TestMethod]
        public void CreateCommand()
        {
            ushort address = 0x1234;
            byte length = 0x67;
            ReadMemoryDebugMessage message = new ReadMemoryDebugMessage(address, length);
            byte[] messageInBytes = message.ToBytes();

            CollectionAssert.AreEqual(
                new byte[] { (byte)DebugCommandBytes.ReadMemory, 0x12, 0x34, length },
                messageInBytes);
        }
    }
}
