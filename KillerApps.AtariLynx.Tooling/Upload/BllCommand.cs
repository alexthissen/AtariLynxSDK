using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.ComLynx
{
    public class BllCommand : Command
    {
        //ProgressBar progressBar = new ProgressBar(100, "Receiving");
        private const int DEFAULT_BAUDRATE = 9600;

        public BllCommand() : base("bll", "Bll debug command") {
            Option<int> comPortOption = new Option<int>("--comport");
            comPortOption.AddAlias("-p");
            comPortOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new [] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for ComLynx");
            Option<FileInfo> uploadFileOption = new Option<FileInfo>("--input");
            uploadFileOption.AddAlias("-i");
            uploadFileOption.IsRequired = true;

            this.AddOption(comPortOption);
            this.AddOption(baudRateOption);
            this.AddOption(uploadFileOption);
            this.Handler = CommandHandler.Create<int, int, FileInfo>(BllUploadHandler);
        }

        private void BllUploadHandler(int comPort, int baudRate, FileInfo input)
        {
            ComLynxUploader uploader = new ComLynxUploader();
            
            string comPortName = String.Format("COM{0}", comPort);
            byte[] bytes = File.ReadAllBytes(input.FullName);

            uploader.UploadComFile(comPortName, bytes, baudRate);
            //progressBar.Tick(100, $"Download completed");
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ComLynxReceiver receiver = (ComLynxReceiver)sender;
            int percentage = (e.TotalBytes * 100) / e.ReceiveBytes;
            //progressBar.Tick(percentage, $"Received {e.TotalBytes}/{e.ReceiveBytes}");
        }
    }
}
