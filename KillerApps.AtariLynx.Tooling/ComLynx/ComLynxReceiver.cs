using System;
using System.ComponentModel;
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
		private ComLynxReceiveStatus status;

        private int receiveSize { get; set; }

        public event ProgressChangedEventHandler ProgressChanged;

		public byte[] Receive(string portName, int baudRate = 62500, int size = 65536 * 8)
        {
			status = new ComLynxReceiveStatus() { TotalBytesToRead = size };
			receiveSize = size + OVERFLOW_SIZE;
			data = new byte[receiveSize];

			using (SerialPort port = new SerialPort(portName, baudRate, Parity.Mark, 8, StopBits.One))
			{
				port.ReceivedBytesThreshold = RECEIVED_BYTES_THRESHOLD;
				port.ReadBufferSize = BUFFER_SIZE;
				port.ReadTimeout = READ_TIMEOUT;

				if (!port.TryOpen()) return null;
				
				port.DataReceived += OnDataReceived;
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
			int percentage = (totalBytes * 100) / receiveSize;
			status.BytesRead = bytesRead;
			ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percentage, status));
        }
	}

    public class ComLynxReceiveStatus
    {
        public int BytesRead { get; set; }
        public int TotalBytesToRead { get; set; }
	}
}
