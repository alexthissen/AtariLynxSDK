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
using System.Linq;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Flashcard
{
    public class FlashcardSettingsCommand : Command
    {
        public Dictionary<int, char> Baudrates = new Dictionary<int, char>
        {
            { 9600, '9' },
            { 57600, '5' },
            { 115200, '1' }
        };
        public Dictionary<string, char> Sizes = new Dictionary<string, char>
        {
            { "auto", 'a' },
            { "128k", 'g' },
            { "256k-bll", 'h' },
            { "512k", 'i' },
            { "512k-bll", 'k' }
        };

        private ProgressBar progressBar = null;

        public FlashcardSettingsCommand() : base("set", "Flashcard settings") 
        {
            this.AddSerialPortOptions();

            this.AddOption(new Option<FlashcardLanguage>(new string[] { "--language", "-l" }, "Language setting"));

            Option<int> rateOption = new Option<int>("--rate");
            rateOption.AddAlias("-r");
            rateOption.FromAmong(Baudrates.Keys.Select(k => k.ToString()).ToArray());
            this.AddOption(rateOption);

            Option<string> modusOption = new Option<string>("--modus", "Modus");
            modusOption.AddAlias("-m");
            modusOption.FromAmong("lnx", "bin", "lyx", "o");
            this.AddOption(modusOption);

            Option<string> sizeOption = new Option<string>("--size", "Size");
            sizeOption.AddAlias("-s");
            sizeOption.FromAmong(Sizes.Keys.ToArray());
            this.AddOption(sizeOption);

            //Argument<string> arg = new Argument<string>("command", "Command to send to Flashcard");
            //this.AddArgument(arg);
            //this.AddArgument(new Argument<string>("command2"));
            
            //this.AddValidator(cmd => {
            //    if (cmd.ValueForOption("size") != null) return "yes"; else return null;
            //});

            this.Handler = CommandHandler.Create<GlobalOptions, SerialPortOptions, FlashcardSettings, IConsole>(FlashcardProxyHandler);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FlashcardWriteStatus status = (FlashcardWriteStatus)e.UserState;
            progressBar.Tick(e.ProgressPercentage, $"Writing {status.BytesWritten}/{status.TotalBytes} bytes");
        }

        // o does not allow setting size
        // bin/lyx 128k 256k/bll 512k 512k-bll

        private void FlashcardProxyHandler(GlobalOptions global, SerialPortOptions serialPortOptions, FlashcardSettings settings, IConsole console)
        {
            FlashcardClient client = new FlashcardClient();

            string response = String.Empty;
            
            if (settings.Modus.HasValue)
            {
                response = client.SendMessageAndReceiveText(serialPortOptions.PortName, serialPortOptions.Baudrate, (char)settings.Modus.Value);
            }

            if (!String.IsNullOrEmpty(settings.Size))
            {
                char size = Sizes[settings.Size.ToLower()];
                response = client.SendMessageAndReceiveText(serialPortOptions.PortName, serialPortOptions.Baudrate, size);
            }

            if (settings.Language.HasValue)
            {
                response = client.SendMessageAndReceiveText(serialPortOptions.PortName,
                    serialPortOptions.Baudrate, (char)settings.Language.Value);
            }

            // Rate should be last setting to change, as it requires a new baudrate for communication
            if (settings.Rate.HasValue)
            {
                char rate = Baudrates[settings.Rate.Value];
                response = client.SendMessageAndReceiveText(serialPortOptions.PortName, serialPortOptions.Baudrate, rate);
                serialPortOptions.Baudrate = settings.Rate.Value;
            }

            if (global.Verbose)
            {
                var color = Console.ForegroundColor;
                //Console.ForegroundColor = ConsoleColor.DarkGreen;
                console.Out.Write(response);
                //Console.ForegroundColor = color;
            }
        }
    }
}
