using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.ComLynx
{
    public class ResetCommand : Command
    {
        //ProgressBar progressBar = new ProgressBar(100, "Receiving");
        private const int DEFAULT_BAUDRATE = 9600;

        public ResetCommand() : base("reset", "Reset debug command") {
            Option<int> comPortOption = new Option<int>("--comport");
            comPortOption.AddAlias("-p");
            comPortOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new [] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for ComLynx");

            this.AddOption(comPortOption);
            this.AddOption(baudRateOption);
            this.Handler = CommandHandler.Create<int, int>(BllResetHandler);
        }

        private void BllResetHandler(int comPort, int baudRate)
        {
            ComLynxUploader uploader = new ComLynxUploader();
            string comPortName = String.Format("COM{0}", comPort);
            uploader.Reset(comPortName, baudRate);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ComLynxReceiver receiver = (ComLynxReceiver)sender;
            int percentage = (e.TotalBytes * 100) / e.ReceiveBytes;
            //progressBar.Tick(percentage, $"Received {e.TotalBytes}/{e.ReceiveBytes}");
        }
    }
}
