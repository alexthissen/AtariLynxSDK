using KillerApps.AtariLynx.Tooling.Bll;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.Tests
{
    [TestClass]
    public class ReadMemoryCommandTest
    {
        [TestMethod]
        public void CreateCommand()
        {
            ushort address = 0x1234;
            byte length = 0x67;
            ReadMemoryDebugCommand command = new ReadMemoryDebugCommand(address, length);
            byte[] commandInBytes = command.ToBytes();

            CollectionAssert.AreEqual(
                new byte[] { (byte)DebugCommandBytes.ReadMemory, 0x12, 0x34, length },
                commandInBytes);
        }
    }
}
