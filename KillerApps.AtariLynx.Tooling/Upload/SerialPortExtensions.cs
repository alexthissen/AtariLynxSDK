using System;
using System.IO;
using System.IO.Ports;

namespace KillerApps.AtariLynx.Tooling.ComLynx
{
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

		public static bool TryOpen(this SerialPort port)
        {
			try
			{
				port.Open();
			}
			catch (UnauthorizedAccessException)
			{
				Console.WriteLine($"Could not open port {port.PortName}. It might be in use.");
				return false;
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine($"Could not find port {port.PortName}. Available ports are " + String.Join(",", SerialPort.GetPortNames()));
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.GetType());
				return false;
			}
			return true;
		}
	}
}
