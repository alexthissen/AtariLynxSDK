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

	}
}
