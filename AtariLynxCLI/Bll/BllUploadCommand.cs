using KillerApps.AtariLynx.Tooling.ComLynx;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.NamingConventionBinder;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Bll
{
    public class BllUploadCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 62500;

//        private ProgressBar progressBar = null;

        public BllUploadCommand() : base("upload", "Upload .o file to Atari Lynx")
        {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);

            Argument<FileInfo> uploadFileArgument = new Argument<FileInfo>("--input");
            uploadFileArgument.Description = "File to upload";
            uploadFileArgument.ExistingOnly();
            this.AddArgument(uploadFileArgument);
            this.Handler = CommandHandler.Create<string, int, FileInfo, InvocationContext>(BllUploadHandler);
        }

        private void BllUploadHandler(string portName, int baudRate, FileInfo input, InvocationContext context)
        {
            BllComLynxClient client = new BllComLynxClient();

            byte[] bytes = File.ReadAllBytes(input.FullName);
            IAnsiConsole console = (IAnsiConsole)context.BindingContext.GetService(typeof(IAnsiConsole));
            console.Progress()
                .Columns([
                    new TaskDescriptionColumn(),    // Task description
                    new ProgressBarColumn(),        // Progress bar
                    new PercentageColumn(),         // Percentage
                    new RemainingTimeColumn()      // Remaining time
                ])
                .Start(progress =>
                {
                    // Define tasks
                    var task = progress.AddTask("[green]Uploading[/]");
                    client.ProgressChanged += (sender, e) =>
                    {
                        task.Value(e.ProgressPercentage);

                        // Check whether output is piped
                        if (!context.Console.IsOutputRedirected)
                            progress.Refresh();
                    };

                    client.UploadComFile(portName, bytes, baudRate, context.GetCancellationToken());
                });
        }
    }
}
