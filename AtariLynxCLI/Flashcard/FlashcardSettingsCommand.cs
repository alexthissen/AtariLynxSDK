using KillerApps.AtariLynx.Tooling.Flashcard;
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

        private const int DEFAULT_BAUDRATE = 115200;

        public FlashcardSettingsCommand() : base("set", "Flashcard settings") 
        {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);

            this.AddOption(new Option<FlashcardLanguage>(new string[] { "--language", "-l" }, "Language setting"));

            Option<int> rateOption = new Option<int>("--rate", "Baudrate for Flashcard");
            rateOption.AddAlias("-r");
            rateOption.FromAmong(Baudrates.Keys.Select(k => k.ToString()).ToArray());
            this.AddOption(rateOption);

            Option<FlashcardModus> modusOption = new Option<FlashcardModus>("--modus", "Modus");
            modusOption.AddAlias("-m");
            //modusOption.FromAmong("lnx", "bin", "lyx", "o");
            //modusOption.AddSuggestions("lnx", "bin");
            this.AddOption(modusOption);

            Option<string> sizeOption = new Option<string>("--size", "Size of roms");
            sizeOption.AddAlias("-s");
            sizeOption.FromAmong(Sizes.Keys.ToArray());
            this.AddOption(sizeOption);
            
            this.AddValidator(cmd =>
            {
                // *.o files do not allow setting size
                //if (cmd.ValueForOption<FlashcardModus>("modus") == FlashcardModus.O &&
                //    cmd.ValueForOption("size") != null)
                if (cmd.Children.Contains("modus") && cmd.Children.Contains("size"))
                {
                    return "You cannot specify a size for '*.o' files";
                }
                return null;
            });

            this.Handler = CommandHandler.Create<GlobalOptions, SerialPortOptions, FlashcardSettings, IConsole>(FlashcardProxyHandler);
        }

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
                //var color = Console.ForegroundColor;
                //console.ResetTerminalForegroundColor();
                console.SetTerminalForegroundColor(ConsoleColor.Yellow);
                //Console.ForegroundColor = ConsoleColor.DarkGreen;
                console.Out.Write(response);
                console.ResetTerminalForegroundColor();
                //Console.ForegroundColor = color;
            }
            else
            {
                console.Out.Write("Settings changed. (Specify --verbose for details)");
            }
        }
    }
}
