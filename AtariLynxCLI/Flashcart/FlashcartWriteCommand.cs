using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Flashcart;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.FlashcartCommand
{
    public class FlashcartWriteCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 115200;
        private const string OK_TERMINATOR = "= OK ===========================================================================\r\n";
        private ProgressBar progressBar = null;

        public FlashcartWriteCommand() : base("write", "Write to Flashcart") 
        {
            Option<string> portOption = new Option<string>("--portname", "Portname");
            portOption.AddAlias("-p");
            portOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new[] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for Flashcart");

            this.AddOption(portOption);
            this.AddOption(baudRateOption);

            Option<FileInfo> uploadFileOption = new Option<FileInfo>("--input");
            uploadFileOption.AddAlias("-i");
            uploadFileOption.ExistingOnly().IsRequired = true;
            this.AddOption(uploadFileOption);
            this.Handler = CommandHandler.Create<bool, string, int, FileInfo>(FlashcartWriteHandler);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WriteStatus status = (WriteStatus)e.UserState;
            progressBar.Tick(e.ProgressPercentage, $"Writing {status.BytesWritten}/{status.TotalBytes} bytes");
        }

        private void FlashcartWriteHandler(bool verbose, string portName, int baudRate, FileInfo input)
        {
            FlashcartProxy proxy = new FlashcartProxy();
            byte[] content = File.ReadAllBytes(input.FullName);
            proxy.ProgressChanged += OnProgressChanged;
            string response;
            using (progressBar = new ProgressBar(100, "Initializing", ProgressBarStyling.Options))
            {
                response = proxy.WriteRomFile(portName, baudRate, content);
            }

            if (response.EndsWith(OK_TERMINATOR))
            {
                response = response.Substring(0, response.Length - OK_TERMINATOR.Length - 1).Trim();
            }
            if (verbose)
            {
                Console.WriteLine();
                Console.WriteLine(response);
            }
        }
    }
}
