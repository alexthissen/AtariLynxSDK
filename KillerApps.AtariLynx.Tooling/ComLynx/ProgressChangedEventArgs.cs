using System;

namespace KillerApps.AtariLynx.Tooling.ComLynx
{
    public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);

    public class ProgressChangedEventArgs: EventArgs
	{
        public ProgressChangedEventArgs(int bytesRead, int totalBytes, int receiveBytes)
        {
            this.BytesRead = bytesRead;
            this.TotalBytes = totalBytes;
            this.ReceiveBytes = receiveBytes;
        }

        public int BytesRead { get; set; }
        public int TotalBytes { get; set; }
        public int ReceiveBytes { get; set; }
    }
}
