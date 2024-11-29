using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Conversion;
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.Drawing;
using System.IO;

namespace KillerApps.AtariLynx.CommandLine.Bll
{
    public class BllScreenshotCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 9600;

        public BllScreenshotCommand() : base("screenshot", "Request screenshot") {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);
            Option<FileInfo> outputFileOption = new Option<FileInfo>([ "--output", "-o" ]);
            this.AddOption(outputFileOption);
            this.Handler = CommandHandler.Create<string, int, FileInfo, InvocationContext>(BllScreenshotHandler);
        }

        private void BllScreenshotHandler(string portName, int baudRate, FileInfo output, InvocationContext context)
        {
            BllComLynxClient client = new BllComLynxClient();
            byte[] screenshotData = null;

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
                    var task = progress.AddTask("[green]Receiving[/]");
                    client.ProgressChanged += (sender, e) =>
                    {
                        task.Value(e.ProgressPercentage);

                        // Check whether output is piped
                        if (!context.Console.IsOutputRedirected)
                            progress.Refresh();
                    };

                    screenshotData = client.TakeScreenshot(portName, baudRate);
                });
            if (screenshotData == null)
            {
                throw new CommandException("Screenshot data not received");
            }

            BitmapConverter conv = new BitmapConverter();
                Bitmap bitmap = conv.ConvertToBitmap(screenshotData);
                bitmap.Save(output.FullName);
                console.MarkupLine("Converting image... Done");
        }
    }
}
