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
    public class FlashcardWriteCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 115200;
        private ProgressTask writeTask = null;

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
            writeTask.Value = status.BytesWritten;
        }

        private void FlashcardWriteHandler(GlobalOptions global, SerialPortOptions serialPortOptions, FlashcardWriteOptions writeOptions, IConsole console)
        {
            string response = String.Empty;
            byte[] content = File.ReadAllBytes(writeOptions.RomFile.FullName);

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
                    new SpinnerColumn() {           // Spinner
                        Spinner = Spinner.Known.Default
                    }
                })
                .Start(ctx =>
                {
                    writeTask = ctx.AddTask("Writing to flashcard", autoStart: false);
                    writeTask.MaxValue = content.Length;
                    
                    Progress<string> progress = new Progress<string>(message =>
                    {
                        if (global.Verbose) AnsiConsole.WriteLine(message);
                    });
                    FlashcardClient proxy = new FlashcardClient(progress);

                    // Add event handlers
                    proxy.ProgressChanged += OnProgressChanged;

                    // Actual writing to card
                    writeTask.StartTask();
                    response = proxy.WriteRomFile(serialPortOptions.PortName, serialPortOptions.Baudrate, content, writeOptions.Force);
                });

            if (global.Verbose)
            {
                console.Out.Write($"Response from flashcard:\r\n{response}");
            }
        }
    }
}