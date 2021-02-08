using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Flashcart;
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
    public class FlashCardCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 115200;
        private const string OK_TERMINATOR = "= OK ===========================================================================\r\n";

        public FlashCardCommand() : base("flashcard", "Lynx FlashCard related command") {
            Option<string> portOption = new Option<string>("--portname", "Portname");
            portOption.AddAlias("-p");
            portOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new [] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for FlashCard");
            this.AddOption(portOption);
            this.AddOption(baudRateOption);

            this.AddCommand(new FlashcartWriteCommand());
            this.AddCommand(new FlashcartVerifyCommand());
            //this.AddCommand(new FlashcartSettingsCommand());

            //this.Handler = CommandHandler.Create<string, int, FileInfo>(FlashcartProxyHandler);
        }

        private void FlashcartProxyHandlerSystemInfo(string portName, int baudRate, string command)
        {
            FlashcartClient proxy = new FlashcartClient();
            //Console.Write("flash:>");
            //string command = Console.ReadLine();
            //ParseResult result = new ComLynxCommand().Parse(command);
            string response = proxy.SendMessageAndReceiveText(portName, baudRate, FlashcartClient.FLASHCART_SYSTEMINFO);

            if (response.EndsWith(OK_TERMINATOR))
            {
                response = response.Substring(0, response.Length - OK_TERMINATOR.Length - 1).Trim();
            }
            Console.WriteLine(response);
        }
    }
}
