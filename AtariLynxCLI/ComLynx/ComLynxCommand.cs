using KillerApps.AtariLynx.Tooling.ComLynx;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.ComLynx
{
    public class ComLynxCommand : Command
    {
        private const int DEFAULT_RECEIVESIZE = 65536 * 8;
        private const int DEFAULT_BAUDRATE = 62500;

        private ProgressTask receiveTask = null;

        public ComLynxCommand() : base("comlynx", "ComLynx related command")
        {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);

            Option<int> sizeOption = new Option<int>(new[] { "--size", "-s" }, () => DEFAULT_RECEIVESIZE, "Size to receive in bytes");
            Option<FileInfo> outputFileOption = new Option<FileInfo>("--output");
            outputFileOption.AddAlias("-o");
            outputFileOption.AddSuggestions("output");

            this.AddOption(sizeOption);
            this.AddOption(outputFileOption);
            this.Handler = CommandHandler.Create<GlobalOptions, string, int, int, FileInfo>(ComLynxReceiveHandler);
        }

        private void ComLynxReceiveHandler(GlobalOptions global, string portName, int baudRate, int size, FileInfo output)
        {
            ComLynxReceiver receiver = new ComLynxReceiver();
            receiver.ProgressChanged += OnProgressChanged;

            AnsiConsole.MarkupLine("[yellow]Waiting to receive bytes from Lynx[/]...");

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
                    receiveTask = ctx.AddTask("Writing to flashcard", autoStart: true);
                    byte[] data = receiver.Receive(portName, baudRate, size);

                    if (data == null)
                    {
                        AnsiConsole.MarkupLine("[red]Download failed[/]...");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[green]Download completed[/]...");
                        receiveTask.Value = receiveTask.MaxValue;
                        if (global.Verbose)
                        {
                            AnsiConsole.MarkupLine($"[yellow]Writing to {output.FullName}[/]...");
                        }
                        File.WriteAllBytes(output.FullName, data);
                    }
                });
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ComLynxReceiveStatus status = (ComLynxReceiveStatus)e.UserState;
            receiveTask.MaxValue = status.TotalBytesToRead;
            receiveTask.Value = status.BytesRead;
        }
    }
}
