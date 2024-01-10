using KillerApps.AtariLynx.Tooling.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.Tests
{
    [TestClass]
    [DeploymentItem(@"TestFiles\" + ComFilePath)]
    public class ComFileHeaderTest
    {
        public const string ComFilePath = @"minimalcomfile.bin";
        public const ushort LoadAddress = 0x0200;
        public const ushort ObjectSizeWithHeader = 0x12D1;

        private byte[] comFileHeaderBuffer;

        public TestContext TestContext { get; set; }

        [TestInitialize()]
        public void TestInitialize()
        {
            FileStream comFileStream = new FileStream(Path.Combine(TestContext.DeploymentDirectory, ComFilePath), FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(comFileStream);
            comFileHeaderBuffer = reader.ReadBytes(10);
        }

        [TestMethod]
        public void CanInitializeHeader()
        {
            ushort address = 0x0200;
            ushort size = 0x12D1;

            ComFileHeader header = new ComFileHeader(address, size);
            Assert.AreEqual(LoadAddress, header.LoadAddress);
            Assert.AreEqual(ObjectSizeWithHeader - ComFileHeader.HEADER_SIZE, header.ObjectSize);
        }

        [TestMethod]
        public void CanDeserializeHeader()
        {
            ComFileHeader header = ComFileHeader.FromBytes(comFileHeaderBuffer);
            Assert.AreEqual(0x80, header.MagicBytes[0]);
            Assert.AreEqual(0x08, header.MagicBytes[1]);
            Assert.AreEqual(LoadAddress, header.LoadAddress);
            Assert.AreEqual(ObjectSizeWithHeader - ComFileHeader.HEADER_SIZE, header.ObjectSize);
            Assert.AreEqual("BS93", Encoding.ASCII.GetString(header.Bs93Signature));
        }

        [TestMethod]
        public void CanSerializeHeader()
        {
            ComFileHeader header = new ComFileHeader(LoadAddress, ObjectSizeWithHeader);
            byte[] data = header.ToBytes();

            Assert.AreEqual(ComFileHeader.HEADER_SIZE, Marshal.SizeOf(typeof(ComFileHeader)), "COM header size should be 10");
            CollectionAssert.AreEqual(comFileHeaderBuffer, data);
        }
    }
}
