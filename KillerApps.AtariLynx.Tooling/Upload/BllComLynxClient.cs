﻿using KillerApps.AtariLynx.Tooling.Bll;
using KillerApps.AtariLynx.Tooling.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace KillerApps.AtariLynx.Tooling.ComLynx
{
    public class BllComLynxClient
    {
		private const int WRITE_TIMEOUT = 5000;
		private const int WRITE_CHUNKSIZE = 64;
		private const int READ_CHUNKSIZE = 64;
		private const int BUFFER_SIZE = 256;
		private const int RECEIVED_BYTES_THRESHOLD = 256;
		private const int READ_TIMEOUT = 5000;
		private const ushort PALETTE_SIZE = 32;
		private const ushort SCREENSHOT_SIZE = 102 * 160 / 2;

		private int totalBytes = 0;
		private int bytesRead = 0;
		private byte[] data;

		public event ProgressChangedEventHandler ProgressChanged;

		public void UploadComFile(string portName, byte[] file, int baudRate = 62500, CancellationToken token = default)
		{
			ComFileHeader header = ComFileHeader.FromBytes(file);
			//if (!header.Verify())
			//{
			//	Console.WriteLine("Invalid Lynx Com file");
			//}

			UploadCore(portName, header, file, ComFileHeader.HEADER_SIZE,
				file.Length - ComFileHeader.HEADER_SIZE, baudRate, token);
		}

		public void ResetProgram(string portName, int baudRate = 62500)
		{
			ResetDebugMessage message = new ResetDebugMessage();

			using (SerialPort port = new SerialPort(portName, baudRate, Parity.Even, 8, StopBits.One))
			{
				//port.WriteTimeout = WRITE_TIMEOUT;
				port.Handshake = Handshake.None;
				if (!port.TryOpen()) return;

				// Write command
				byte[] commandBytes = message.ToBytes();
				port.Write(commandBytes, 0, commandBytes.Length);

				if (port.IsOpen) port.Close();
			}
		}

		public byte[] TakeScreenshot(string portName, int baudRate = 62500)
		{
			ScreenshotDebugMessage message = new ScreenshotDebugMessage();
			byte[] messageBytes = message.ToBytes();

			data = new byte[PALETTE_SIZE + SCREENSHOT_SIZE];

			using (SerialPort port = new SerialPort(portName, baudRate, Parity.Even, 8, StopBits.One))
			{
				port.Handshake = Handshake.None;
				port.ReadBufferSize = BUFFER_SIZE;
				port.ReadTimeout = READ_TIMEOUT;

				if (!port.TryOpen()) return null;

				// Write message to port
				try
				{
					port.Write(messageBytes, 0, messageBytes.Length);

					// Read back same bytes because RX and TX are connected
					port.Read(messageBytes, 0, messageBytes.Length);

					// Now Lynx should send back palette of 32 bytes and video memory
					while (totalBytes < data.Length) // or timeout
					{
						totalBytes += port.Read(data, totalBytes, Math.Min(data.Length - totalBytes, READ_CHUNKSIZE));
						int percentage = (totalBytes * 100) / data.Length;
						ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percentage, bytesRead));
					}
				}
				catch (TimeoutException)
				{
					Console.WriteLine("Timeout waiting for response.");
					return null;
				}
				finally
				{
					if (port.IsOpen) port.Close();
				}
				return data;
			}
		}

		void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			SerialPort port = (SerialPort)sender;
			byte[] buffer = new byte[256];
			bytesRead = port.Read(buffer, 0, 256);
			Array.Copy(buffer, 0, data, totalBytes, Math.Min(bytesRead, data.Length - totalBytes));
			totalBytes += bytesRead;

			int percentage = (totalBytes * 100) / data.Length;
			ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percentage, bytesRead));
		}

		protected void UploadCore(string portName, ComFileHeader header, byte[] program,
			int offset, int count, int baudRate, CancellationToken token)
        {
			UploadDebugMessage message = new UploadDebugMessage(header.LoadAddress, (ushort)(header.ObjectSize));

			using (SerialPort port = new SerialPort(portName, baudRate, Parity.Even, 8, StopBits.One))
			{
				port.WriteTimeout = WRITE_TIMEOUT;
				port.Handshake = Handshake.None;

				if (!port.TryOpen()) return;

				// Write command
				byte[] messageBytes = message.ToBytes();
				port.Write(messageBytes, 0, messageBytes.Length);

				int bytesSent = 0;
				while (bytesSent < header.ObjectSize && !token.IsCancellationRequested)
				{
					// Send single chunk
					int chunkSize = Math.Min(header.ObjectSize - bytesSent, WRITE_CHUNKSIZE);
					port.Write(program, offset + bytesSent, chunkSize);
					bytesSent += chunkSize;

					// Report progress
					int percentage = (bytesSent * 100) / header.ObjectSize;
					ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percentage, bytesSent));
				}

				if (port.IsOpen) port.Close();
			}
		}
	}
}
