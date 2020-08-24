using KillerApps.AtariLynx.Tooling.Bll;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.Tests
{
    [TestClass]
    public class WriteMemoryCommandTest
    {
        [TestMethod]
        public void CreateSingleByteCommand()
        {
            byte data = 0x42;
            ushort address = 0x1234;
            WriteMemoryDebugCommand command = new WriteMemoryDebugCommand(address, data);
            byte[] commandInBytes = command.ToBytes();

            CollectionAssert.AreEqual(
                new byte[] { (byte)DebugCommandBytes.WriteMemory, 0x12, 0x34, 0x01, data },
                commandInBytes);
        }

        [TestMethod]
        public void CreateWordCommand()
        {
            ushort data = 0x4243;
            ushort address = 0x1234;
            WriteMemoryDebugCommand command = new WriteMemoryDebugCommand(address, data);
            byte[] commandInBytes = command.ToBytes();

            CollectionAssert.AreEqual(
                new byte[] { (byte)DebugCommandBytes.WriteMemory, 0x12, 0x34, 0x02, 0x42, 0x43 },
                commandInBytes);
        }
    }
}
