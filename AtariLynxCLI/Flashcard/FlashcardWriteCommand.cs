using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Flashcard;
using Kurukuru;
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
    public class FlashcardWriteCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 115200;
        private const string OK_TERMINATOR = "= OK ===========================================================================\r\n";
        private ProgressBar progressBar = null;

        public FlashcardWriteCommand() : base("write", "Write to flashcard")
        {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);

            Option<bool> forceOption = new Option<bool>("--force");
            forceOption.AddAlias("-f");
            this.AddOption(forceOption);

            Argument<FileInfo> inputFileArgument = new Argument<FileInfo>("romfile", "File to send to flashcard");
            inputFileArgument.ExistingOnly();
            this.AddArgument(inputFileArgument);

            this.Handler = CommandHandler.Create<GlobalOptions, SerialPortOptions, FlashcardWriteOptions, IConsole>(FlashcardWriteHandler);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FlashcardSendStatus status = (FlashcardSendStatus)e.UserState;
            progressBar.Tick(e.ProgressPercentage, $"Writing {status.BytesWritten}/{status.TotalBytes} bytes");
        }

        private void FlashcardWriteHandler(GlobalOptions global, SerialPortOptions serialPortOptions, FlashcardWriteOptions writeOptions, IConsole console)
        {
            string response = String.Empty;
            byte[] content = File.ReadAllBytes(writeOptions.RomFile.FullName);

            using (progressBar = new ProgressBar(100, "Initializing", ProgressBarStyling.Options))
            {
                Progress<string> progress = new Progress<string>(message => {
                    if (global.Verbose) progressBar.WriteLine(message);
                });
                FlashcardClient proxy = new FlashcardClient(progress);

                // Add event handlers
                proxy.ProgressChanged += OnProgressChanged;

                // Actual writing to card
                response = proxy.WriteRomFile(serialPortOptions.PortName, serialPortOptions.Baudrate, content, writeOptions.Force);
            }

            if (global.Verbose)
            {
                console.Out.Write($"Response from flashcard:\r\n{response}");
            }
        }
    }
}