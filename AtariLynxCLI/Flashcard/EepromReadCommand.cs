using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Flashcard;
using ShellProgressBar;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Flashcard
{
    public class EepromReadCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 115200;
        private const string OK_TERMINATOR = "= OK ===========================================================================\r\n";
        private ProgressBar progressBar = null;

        public EepromReadCommand() : base("eeprom-read", "Read from EEPROM of flashcard")
        {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);

            Option<bool> forceOption = new Option<bool>("--force");
            forceOption.AddAlias("-f");
            this.AddOption(forceOption);

            Option<int> sizeOption = new Option<int>(new[] { "--size", "-s" },
                "Size of EEPROM content.");
            sizeOption.FromAmong("128", "512", "2048");
            this.AddOption(sizeOption);

            Option<FileInfo> outputFileOption = new Option<FileInfo>("file", "Output file for binary EEPROM content");
            this.AddOption(outputFileOption);
            
            this.AddValidator(result => { return null; });
            this.Handler = CommandHandler.Create<GlobalOptions, SerialPortOptions, EepromReadOptions, IConsole>(EepromReadHandler);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FlashcardTransmitStatus status = (FlashcardTransmitStatus)e.UserState;
            //progressBar.Tick(e.ProgressPercentage, $"Reading {status.BytesWritten}/{status.TotalBytes} bytes");
        }

        private void EepromReadHandler(GlobalOptions global, SerialPortOptions serialPortOptions, EepromReadOptions readOptions, IConsole console)
        {
            string response = String.Empty;
            IEnumerable<byte> data = null;

            AnsiConsole.Status().Start("Reading EEPROM", ctx =>
            {
                Progress<string> progress = new Progress<string>(message =>
                {
                    if (global.Verbose) progressBar.WriteLine(message);
                });
                FlashcardClient proxy = new FlashcardClient(progress);

                // Add event handlers
                proxy.ProgressChanged += OnProgressChanged;
                ctx.Spinner(Spinner.Known.Aesthetic);
                ctx.SpinnerStyle(Style.Parse("green"));

                // Actual reading to card
                data = proxy.ReadEepromFile(serialPortOptions.PortName, serialPortOptions.Baudrate, readOptions.Size);
            });

            console.Out.Write(BitConverter.ToString(data.Take(readOptions.Size).ToArray()) + Environment.NewLine);


            if (global.Verbose)
            {
                console.Out.Write("Response from flashcard:" + Environment.NewLine);
                console.Out.Write($"[Binary data: {data.Count()} bytes]");
                console.Out.Write(Encoding.Default.GetString(data.Skip(readOptions.Size).ToArray()));
            }
        }
    }
}