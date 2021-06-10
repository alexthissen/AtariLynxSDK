using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Conversion;
using Kurukuru;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;

namespace KillerApps.AtariLynx.CommandLine.Bll
{
    public class BllScreenshotCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 9600;

        private ProgressBar progressBar = null;

        public BllScreenshotCommand() : base("screenshot", "Request screenshot") {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);
            Option<FileInfo> outputFileOption = new Option<FileInfo>(new string[] { "--output", "-o" });
            this.AddOption(outputFileOption);
            this.Handler = CommandHandler.Create<string, int, FileInfo>(BllScreenshotHandler);
        }

        private void BllScreenshotHandler(string portName, int baudRate, FileInfo output)
        {
            BllComLynxClient client = new BllComLynxClient();
            client.ProgressChanged += OnProgressChanged;
            byte[] screenshotData;

            using (progressBar = new ProgressBar(100, "Initializing"))
            {
                screenshotData = client.TakeScreenshot(portName, baudRate);
                if (screenshotData == null)
                {
                    throw new CommandException("Screenshot data not received");
                }
            }

            Spinner.Start("Converting image...", spinner => {
                BitmapConverter conv = new BitmapConverter();
                Bitmap bitmap = conv.ConvertToBitmap(screenshotData);
                bitmap.Save(output.FullName);
                spinner.Succeed("Converting image... Done");
            });
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Tick(e.ProgressPercentage, $"Received {e.UserState} bytes");
        }
    }
}
