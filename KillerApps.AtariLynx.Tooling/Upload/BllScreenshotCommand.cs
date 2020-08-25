using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.ComLynx
{
    public class BllScreenshotCommand : Command
    {
        //ProgressBar progressBar = new ProgressBar(100, "Receiving");
        private const int DEFAULT_BAUDRATE = 9600;

        public BllScreenshotCommand() : base("screenshot", "Request screens shot") {
            Option<int> comPortOption = new Option<int>("--comport");
            comPortOption.AddAlias("-p");
            comPortOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new [] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for ComLynx");
            Option<FileInfo> outputFileOption = new Option<FileInfo>("--output");
            outputFileOption.AddAlias("-o");

            this.AddOption(comPortOption);
            this.AddOption(baudRateOption);
            this.AddOption(outputFileOption);
            this.Handler = CommandHandler.Create<int, int, FileInfo>(BllScreenshotHandler);
        }

        private void BllScreenshotHandler(int comPort, int baudRate, FileInfo output)
        {
            ComLynxUploader uploader = new ComLynxUploader();
            string comPortName = String.Format("COM{0}", comPort);
            byte[] screenshotData = uploader.Screenshot(comPortName, baudRate);

            Conversion.BitmapConverter conv = new Conversion.BitmapConverter();
            Bitmap bitmap = conv.ConvertToBitmap(screenshotData);
            bitmap.Save(output.FullName);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ComLynxReceiver receiver = (ComLynxReceiver)sender;
            int percentage = (e.TotalBytes * 100) / e.ReceiveBytes;
            //progressBar.Tick(percentage, $"Received {e.TotalBytes}/{e.ReceiveBytes}");
        }
    }
}
