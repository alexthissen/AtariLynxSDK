using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Flashcard;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Flashcard
{
    public class FlashcardCommand : Command
    {
        private const string OK_TERMINATOR = "= OK ===========================================================================\r\n";
        private const int DEFAULT_BAUDRATE = 115200;

        public FlashcardCommand() : base("flashcard", "Lynx Flashcard related command") 
        {
            this.AddCommand(new FlashcardWriteCommand());
            this.AddCommand(new FlashcardVerifyCommand());
            this.AddCommand(new FlashcardSettingsCommand());
            this.AddCommand(new EepromWriteCommand());
            this.AddCommand(CreateDirectCommand("info", "Flashcard board information", 's'));
            this.AddCommand(CreateDirectCommand("credits", "Credits", 'c'));
            this.AddCommand(CreateDirectCommand("reset", "Reset all", 'x'));
        }

        private static Command CreateDirectCommand(string name, string description, char message) {
            Command command = new Command(name, description)
                .AddSerialPortOptions(DEFAULT_BAUDRATE);
            command.Handler = CommandHandler.Create((SerialPortOptions options) => {
                FlashcardClient client = new FlashcardClient();
                string response = client.SendMessageAndReceiveText(options.PortName, options.Baudrate, message);
                Console.WriteLine(response);
            });
            return command;
        }
    }
}
