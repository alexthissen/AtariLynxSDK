using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Conversion;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Bll
{
    public class BllScreenshotCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 9600;
        private ProgressBar progressBar = null;

        public BllScreenshotCommand() : base("screenshot", "Request screens shot") {
            Option<int> comPortOption = new Option<int>("--comport");
            comPortOption.AddAlias("-p");
            comPortOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(
                new [] { "--baudrate", "-b" }, 
                () => DEFAULT_BAUDRATE, "Baud rate for ComLynx");
            Option<FileInfo> outputFileOption = new Option<FileInfo>(new string[] { "--output", "-o" });

            this.AddOption(comPortOption);
            this.AddOption(baudRateOption);
            this.AddOption(outputFileOption);
            this.Handler = CommandHandler.Create<int, int, FileInfo>(BllScreenshotHandler);
        }

        private void BllScreenshotHandler(int comPort, int baudRate, FileInfo output)
        {
            ComLynxUploader uploader = new ComLynxUploader();
            uploader.ProgressChanged += OnProgressChanged;
            string comPortName = String.Format("COM{0}", comPort);
            using (progressBar = new ProgressBar(100, "Initializing"))
            {
                byte[] screenshotData = uploader.Screenshot(comPortName, baudRate);

                BitmapConverter conv = new BitmapConverter();
                Bitmap bitmap = conv.ConvertToBitmap(screenshotData);
                bitmap.Save(output.FullName);
            }
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Tick(e.ProgressPercentage, $"Received {e.UserState} bytes");
        }
    }
}
