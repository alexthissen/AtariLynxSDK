using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Flashcard;
using ShellProgressBar;
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
    public class EepromWriteCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 115200;
        private const string OK_TERMINATOR = "= OK ===========================================================================\r\n";
        private ProgressBar progressBar = null;

        public EepromWriteCommand() : base("eeprom", "Write to EEPROM of flashcard")
        {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);

            Option<bool> forceOption = new Option<bool>("--force");
            forceOption.AddAlias("-f");
            this.AddOption(forceOption);

            Option<int> sizeOption = new Option<int>(new[] { "--size", "-s" },
                "Size of EEPROM content.");
            sizeOption.FromAmong("128", "512", "2048");
            this.AddOption(sizeOption);

            Argument<FileInfo> inputFileArgument = new Argument<FileInfo>("file", "File with binary EEPROM content");
            inputFileArgument.ExistingOnly();
            inputFileArgument.AddValidator(result => { return ValidateParts(result); });
            this.AddArgument(inputFileArgument);
            
            this.AddValidator(result => { return null; });
            this.Handler = CommandHandler.Create<GlobalOptions, SerialPortOptions, EepromWriteOptions, IConsole>(EepromWriteHandler);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FlashcardSendStatus status = (FlashcardSendStatus)e.UserState;
            progressBar.Tick(e.ProgressPercentage, $"Writing {status.BytesWritten}/{status.TotalBytes} bytes");
        }

        private string ValidateParts(ArgumentResult result) //FileInfo file, int size)
        {
            //if (file.Length != size)
              //  throw new ArgumentException("File size is smaller than requested.");
            return null;
        }

        private void EepromWriteHandler(GlobalOptions global, SerialPortOptions serialPortOptions, EepromWriteOptions writeOptions, IConsole console)
        {
            string response = String.Empty;
            byte[] content = File.ReadAllBytes(writeOptions.File.FullName);

            using (progressBar = new ProgressBar(100, "Initializing", ProgressBarStyling.Options))
            {
                Progress<string> progress = new Progress<string>(message => {
                    if (global.Verbose) progressBar.WriteLine(message);
                });
                FlashcardClient proxy = new FlashcardClient(progress);

                // Add event handlers
                proxy.ProgressChanged += OnProgressChanged;

                IEnumerable<byte[]> parts = content.Slices(512, false);

                // Actual writing to card
                response = proxy.WriteEepromFile(serialPortOptions.PortName, serialPortOptions.Baudrate, parts, true);
            }

            if (global.Verbose)
            {
                console.Out.Write("Response from flashcard:");
                console.Out.Write(response);
            }
        }
    }

    public static class ArrayExtensions
    {
        public static T[] CopySlice<T>(this T[] source, int index, int length, bool padToLength = false)
        {
            int n = length;
            T[] slice = null;

            if (source.Length < index + length)
            {
                n = source.Length - index;
                if (padToLength)
                {
                    slice = new T[length];
                }
            }

            if (slice == null) slice = new T[n];
            Array.Copy(source, index, slice, 0, n);
            return slice;
        }

        public static IEnumerable<T[]> Slices<T>(this T[] source, int count, bool padToLength = false)
        {
            for (var i = 0; i < source.Length; i += count)
                yield return source.CopySlice(i, count, padToLength);
        }
    }
}