using KillerApps.AtariLynx.Tooling.ComLynx;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Bll
{
    public class BllUploadCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 62500;

        private ProgressBar progressBar = null;

        public BllUploadCommand() : base("upload", "Upload command") 
        {
            Option<string> portOption = new Option<string>("--portname", "Portname");
            portOption.AddAlias("-p");
            portOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new [] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for ComLynx");
            Option<FileInfo> uploadFileOption = new Option<FileInfo>("--input");
            uploadFileOption.AddAlias("-i");
            uploadFileOption.ExistingOnly().IsRequired = true;

            this.AddOption(portOption);
            this.AddOption(baudRateOption);
            this.AddOption(uploadFileOption);
            this.Handler = CommandHandler.Create<string, int, FileInfo>(BllUploadHandler);
        }

        private void BllUploadHandler(string portName, int baudRate, FileInfo input)
        {
            BllComLynxClient uploader = new BllComLynxClient();
            uploader.ProgressChanged += OnProgressChanged;
            byte[] bytes = File.ReadAllBytes(input.FullName);
            using (progressBar = new ProgressBar(100, "Initializing", ProgressBarStyling.Options))
            {
                uploader.UploadComFile(portName, bytes, baudRate);
                progressBar.Tick(100, $"Upload completed");
            }
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Tick(e.ProgressPercentage, $"Sent {e.UserState} bytes");
        }
    }
}
