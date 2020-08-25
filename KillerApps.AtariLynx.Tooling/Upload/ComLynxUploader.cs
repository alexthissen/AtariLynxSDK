using KillerApps.AtariLynx.Tooling.Bll;
using KillerApps.AtariLynx.Tooling.Models;
using System;
using System.CommandLine;
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
		private const int BUFFER_SIZE = 256;
		private const int RECEIVED_BYTES_THRESHOLD = 256;
		private const int READ_TIMEOUT = 5000;

		//public readonly byte[] UPLOAD_COMMAND = new byte[] { 0x81 };

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

		public byte[] Screenshot(string comPort, int baudRate = 62500)
		{
			ScreenshotDebugCommand command = new ScreenshotDebugCommand();
			byte[] commandBytes = command.ToBytes();

			data = new byte[SCREENSHOT_SIZE + PALETTE_SIZE + commandBytes.Length];

			using (SerialPort port = new SerialPort(comPort, baudRate, Parity.Even, 8, StopBits.One))
			{
				//port.WriteTimeout = WRITE_TIMEOUT;
				port.Handshake = Handshake.None;
				//port.ReceivedBytesThreshold = RECEIVED_BYTES_THRESHOLD;
				port.ReadBufferSize = BUFFER_SIZE;
				port.ReadTimeout = READ_TIMEOUT;
				port.DataReceived += OnDataReceived;

				port.Open();

				// Write command 
				port.Write(commandBytes, 0, commandBytes.Length);

				// Now Lynx should send back palette of 32 bytes and video memory 
				while (totalBytes < data.Length) // or timeout
				{
					Thread.Sleep(500);
				}

				if (port.IsOpen) port.Close();
				return data;
			}
		}

		private int totalBytes = 0;
		private int bytesRead = 0;
		private byte[] data;
		public event ProgressChangedEventHandler ProgressChanged;

		private const ushort SCREENSHOT_SIZE = 102 * 160 / 2;
		private const ushort PALETTE_SIZE = 32;

		void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			SerialPort port = (SerialPort)sender;
			byte[] buffer = new byte[256];
			bytesRead = port.Read(buffer, 0, 256);
			Array.Copy(buffer, 0, data, totalBytes, Math.Min(bytesRead, data.Length - totalBytes));
			totalBytes += bytesRead;
			Console.WriteLine($"Received Read:{bytesRead} Total:{totalBytes} Target:{data.Length}");

			ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(bytesRead, totalBytes, SCREENSHOT_SIZE));
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
