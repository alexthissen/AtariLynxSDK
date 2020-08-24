using KillerApps.AtariLynx.Tooling.Bll;
using KillerApps.AtariLynx.Tooling.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace KillerApps.AtariLynx.Tooling.ComLynx
{
    public class ComLynxUploader
    {
		private const int WRITE_TIMEOUT = 5000;
		public readonly byte[] UPLOAD_COMMAND = new byte[] { 0x81 };

		public void UploadComFile(string comPort, byte[] file, int baudRate = 62500)
		{
			ComFileHeader header = ComFileHeader.FromBytes(file);
			//if (!header.Verify())
			//{
			//	Console.WriteLine("Invalid Lynx Com file");
			//}

			UploadCore(comPort, header, file, ComFileHeader.HEADER_SIZE, 
				file.Length - ComFileHeader.HEADER_SIZE, baudRate);
		}

		public void Reset(string comPort, int baudRate = 62500)
		{
			ResetDebugCommand command = new ResetDebugCommand();

			using (SerialPort port = new SerialPort(comPort, baudRate, Parity.Even, 8, StopBits.One))
			{
				//port.WriteTimeout = WRITE_TIMEOUT;
				port.Handshake = Handshake.None;
				port.Open();

				// Write command 
				byte[] commandBytes = command.ToBytes();
				port.Write(commandBytes, 0, commandBytes.Length);

				if (port.IsOpen) port.Close();
			}
		}

		protected void UploadCore(string comPort, ComFileHeader header, byte[] program, int offset, int count, int baudRate)
        {
			UploadDebugCommand command = new UploadDebugCommand(header.LoadAddress, header.ObjectSize);

			using (SerialPort port = new SerialPort(comPort, baudRate, Parity.Even, 8, StopBits.One))
			{
				//port.WriteTimeout = WRITE_TIMEOUT;
				port.Handshake = Handshake.None;
				port.Open();
				
				// Write command 
				byte[] commandBytes = command.ToBytes();
				port.Write(commandBytes, 0, commandBytes.Length);

				port.Write(program, offset, command.Length);

				if (port.IsOpen) port.Close();
			}
		}
	}

	public static class SerialPortExtensions
    {
		public static void WriteByte(this SerialPort port, byte data)
        {
			port.Write(new byte[] { data }, 0, 1);
        }

		public static void WriteUshort(this SerialPort port, ushort data)
		{
			port.Write(new byte[] { (byte)(data >> 8), (byte)(data & 0xff) }, 0, 2);
		}

	}
}
