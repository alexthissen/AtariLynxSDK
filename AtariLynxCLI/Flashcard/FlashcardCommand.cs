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
    public class FlashCardCommand : Command
    {
        private const string OK_TERMINATOR = "= OK ===========================================================================\r\n";

        public FlashCardCommand() : base("flashcard", "Lynx FlashCard related command") 
        {
            this.AddCommand(new FlashcardWriteCommand());
            this.AddCommand(new FlashcardVerifyCommand());
            this.AddCommand(new FlashcardSettingsCommand());
        }

        private void FlashcardProxyHandlerSystemInfo(string portName, int baudRate, string command)
        {
            FlashcardClient proxy = new FlashcardClient();
            //Console.Write("flash:>");
            //string command = Console.ReadLine();
            //ParseResult result = new ComLynxCommand().Parse(command);
            string response = proxy.SendMessageAndReceiveText(portName, baudRate, FlashcardClient.FLASHCARD_SYSTEMINFO);

            if (response.EndsWith(OK_TERMINATOR))
            {
                response = response.Substring(0, response.Length - OK_TERMINATOR.Length - 1).Trim();
            }
            Console.WriteLine(response);
        }
    }
}
