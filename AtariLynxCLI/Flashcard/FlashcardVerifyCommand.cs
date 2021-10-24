using KillerApps.AtariLynx.Tooling.Flashcard;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Flashcard
{
    public class FlashcardVerifyCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 115200;
        private ProgressTask verifyTask = null;

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
            verifyTask.Value = status.BytesWritten;
        }

        private void FlashcardVerifyHandler(GlobalOptions global, SerialPortOptions serialPortOptions, FileInfo input, IConsole console)
        {
            string response = String.Empty;
            byte[] content = File.ReadAllBytes(input.FullName);

            AnsiConsole.MarkupLine("[yellow]Initializing flashcard[/]...");

            AnsiConsole.Progress()
                .AutoClear(false)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),    // Task description
                    new ProgressBarColumn(),        // Progress bar
                    new PercentageColumn(),         // Percentage
                    new DownloadedColumn(),         // Upload
                    new RemainingTimeColumn(),      // Remaining time
                    new SpinnerColumn() { Spinner = Spinner.Known.Arc }            // Spinner
                })
                .Start(ctx =>
                {
                    verifyTask = ctx.AddTask("Verifying flashcard", autoStart: false);
                    verifyTask.MaxValue = content.Length;

                    Progress<string> progress = new Progress<string>(message =>
                    {
                        if (global.Verbose) AnsiConsole.WriteLine(message);
                    });
                    FlashcardClient proxy = new FlashcardClient(progress);

                    // Add event handlers
                    proxy.ProgressChanged += OnProgressChanged;

                    // Actual verifying of card
                    verifyTask.StartTask();
                    response = proxy.VerifyRomFile(serialPortOptions.PortName, serialPortOptions.Baudrate, content);
                });
        }
    }
}