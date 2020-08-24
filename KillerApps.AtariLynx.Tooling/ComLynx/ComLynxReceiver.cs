﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace KillerApps.AtariLynx.Tooling.ComLynx
{
    public class ComLynxReceiver
    {
		private const int BUFFER_SIZE = 256;
		private const int RECEIVED_BYTES_THRESHOLD = 256;
		private const int READ_TIMEOUT = 5000;
		private const int OVERFLOW_SIZE = 1024;

		private int totalBytes = 0;
		private int bytesRead = 0;

		private byte[] data;

        private int receiveSize { get; set; }

        public event ProgressChangedEventHandler ProgressChanged;

		public byte[] Receive(string comPort, int baudRate = 62500, int size = 65536 * 8)
        {
			receiveSize = size + OVERFLOW_SIZE;
			data = new byte[receiveSize];

			using (SerialPort port = new SerialPort(comPort, baudRate, Parity.Mark, 8, StopBits.One))
			{
				port.ReceivedBytesThreshold = RECEIVED_BYTES_THRESHOLD;
				port.ReadBufferSize = BUFFER_SIZE;
				port.ReadTimeout = READ_TIMEOUT;
				port.DataReceived += OnDataReceived;
				port.Open();

				while (totalBytes < size) // or timeout
                {
					Thread.Sleep(500);
                }
				if (port.IsOpen) port.Close();
			}

			byte[] file = new byte[size];
			Array.Copy(data, 0, file, 0, size);
			return file;
		}

		void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			SerialPort port = (SerialPort)sender;
			byte[] buffer = new byte[256];
			bytesRead = port.Read(buffer, 0, 256);
			Array.Copy(buffer, 0, data, totalBytes, Math.Min(bytesRead, data.Length - totalBytes));
			totalBytes += bytesRead;
			Console.WriteLine($"Received BTR:{port.BytesToRead} Read:{bytesRead} Total:{totalBytes} Target:{receiveSize}");

            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(bytesRead, totalBytes, receiveSize));
        }
	}
}