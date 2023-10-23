using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Flashcard;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Flashcard
{
    public class FlashcardVerifyCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 115200;
        private const string OK_TERMINATOR = "= OK ===========================================================================\r\n";
        private ProgressBar progressBar = null;

        public FlashcardVerifyCommand() : base("verify", "Verify ROM on Flashcard")
        {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);

            Option<FileInfo> uploadFileOption = new Option<FileInfo>("--input");
            uploadFileOption.AddAlias("-i");
            uploadFileOption.ExistingOnly().IsRequired = true;
            this.AddOption(uploadFileOption);
            this.Handler = CommandHandler.Create<GlobalOptions, SerialPortOptions, FileInfo, IConsole>(FlashcardVerifyHandler);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FlashcardSendStatus status = (FlashcardSendStatus)e.UserState;
            progressBar.Tick(e.ProgressPercentage, $"Verifying {status.BytesWritten}/{status.TotalBytes} bytes");
        }

        private void FlashcardVerifyHandler(GlobalOptions global, SerialPortOptions serialPortOptions, FileInfo input, IConsole console)
        {
            string response = String.Empty;
            byte[] content = File.ReadAllBytes(input.FullName);

            using (progressBar = new ProgressBar(100, "Initializing", ProgressBarStyling.Options))
            {
                Progress<string> progress = new Progress<string>(message => {
                    if (global.Verbose) progressBar.WriteLine(message);
                });
                FlashcardClient proxy = new FlashcardClient(progress);

                // Add event handlers
                proxy.ProgressChanged += OnProgressChanged;

                // Actual writing to card
                response = proxy.VerifyRomFile(serialPortOptions.PortName, serialPortOptions.Baudrate, content);
            }

            if (global.Verbose)
            {
                console.Out.Write($"Response from flashcard:\r\n{response}");
            }
        }
    }
}