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
    public class FlashcartProxyCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 115200;
        
        private ProgressBar progressBar = null;

        public FlashcartProxyCommand() : base("flashcart", "Flashcart related command") {
            Option<string> portOption = new Option<string>("--portname", "Portname");
            portOption.AddAlias("-p");
            portOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new [] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for Flashcart");

            this.AddOption(portOption);
            this.AddOption(baudRateOption);

            //Argument<string> arg = new Argument<string>("command", "Command to send to Flashcart");
            //this.AddArgument(arg);
            //this.AddArgument(new Argument<string>("command2"));
            this.Handler = CommandHandler.Create<string, int, string>(FlashcartProxyHandler);
        }

        private void FlashcartProxyHandler(string portName, int baudRate, string command)
        {
            Console.WriteLine(command);
            FlashcartProxy proxy = new FlashcartProxy();
            //Console.Write("flash:>");
            //string command = Console.ReadLine();
            //ParseResult result = new ComLynxCommand().Parse(command);
            string response = proxy.SendMessageAndReceiveText(portName, baudRate);
            Console.WriteLine(response);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ComLynxReceiveStatus status = (ComLynxReceiveStatus)e.UserState;
            progressBar.Tick(e.ProgressPercentage, $"Received {status.BytesRead}/{status.TotalBytesToRead}");
        }
    }
}
