﻿using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Flashcart;
using Kurukuru;
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
    public class FlashcardWriteOptions
    {
        public FileInfo Input { get; set; }
        public bool Force { get; set; }
    }

    public class FlashcartWriteCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 115200;
        private const string OK_TERMINATOR = "= OK ===========================================================================\r\n";
        private ProgressBar progressBar = null;

        public FlashcartWriteCommand() : base("write", "Write to Flashcard")
        {
            Option<bool> forceOption = new Option<bool>("--force");
            forceOption.AddAlias("-f");
            this.AddOption(forceOption);

            Option<FileInfo> uploadFileOption = new Option<FileInfo>("--input");
            uploadFileOption.AddAlias("-i");
            uploadFileOption.ExistingOnly().IsRequired = true;
            this.AddOption(uploadFileOption);
            this.Handler = CommandHandler.Create<GlobalOptions, SerialPortOptions, FlashcardWriteOptions, IConsole>(FlashcartWriteHandler);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FlashCardWriteStatus status = (FlashCardWriteStatus)e.UserState;
            progressBar.Tick(e.ProgressPercentage, $"Writing {status.BytesWritten}/{status.TotalBytes} bytes");
        }

        private void FlashcartWriteHandler(GlobalOptions global, SerialPortOptions serialPortOptions, FlashcardWriteOptions writeOptions, IConsole console)
        {
            string response = String.Empty;
            byte[] content = File.ReadAllBytes(writeOptions.Input.FullName);

            using (progressBar = new ProgressBar(100, "Initializing", ProgressBarStyling.Options))
            {
                Progress<string> progress = new Progress<string>(message => {
                    if (global.Verbose) progressBar.WriteLine(message);
                });
                FlashcartClient proxy = new FlashcartClient(progress);

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