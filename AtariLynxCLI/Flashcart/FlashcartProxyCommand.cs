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
        private const string OK_TERMINATOR = "= OK ===========================================================================\r\n";
        private ProgressBar progressBar = null;

        public FlashcartProxyCommand() : base("flashcart", "Flashcart related command") {
            Option<string> portOption = new Option<string>("--portname", "Portname");
            portOption.AddAlias("-p");
            portOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new [] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for Flashcart");

            this.AddOption(portOption);
            this.AddOption(baudRateOption);
            Option<FileInfo> uploadFileOption = new Option<FileInfo>("--input");
            uploadFileOption.AddAlias("-i");
            uploadFileOption.ExistingOnly().IsRequired = true;
            this.AddOption(uploadFileOption);

            //Argument<string> arg = new Argument<string>("command", "Command to send to Flashcart");
            //this.AddArgument(arg);
            //this.AddArgument(new Argument<string>("command2"));
            this.Handler = CommandHandler.Create<string, int, FileInfo>(FlashcartProxyHandler);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FlashCardWriteStatus status = (FlashCardWriteStatus)e.UserState;
            progressBar.Tick(e.ProgressPercentage, $"Writing {status.BytesWritten}/{status.TotalBytes} bytes");
        }

        private void FlashcartProxyHandler(string portName, int baudRate, FileInfo input)
        {
            FlashcartClient proxy = new FlashcartClient();
            //string response = proxy.SendMessageAndReceiveText(portName, baudRate);
            byte[] content = File.ReadAllBytes(input.FullName);
            proxy.ProgressChanged += OnProgressChanged;

            string response;
            using (progressBar = new ProgressBar(100, "Initializing", ProgressBarStyling.Options))
            {
                response = proxy.WriteRomFile(portName, baudRate, content, false);
            }

            if (response.EndsWith(OK_TERMINATOR))
            {
                response = response.Substring(0, response.Length - OK_TERMINATOR.Length - 1).Trim();
            }
            Console.WriteLine();
            Console.WriteLine(response);
        }

        //private void FlashcartProxyHandlerSystemInfo(string portName, int baudRate, string command)
        //{
        //    Console.WriteLine(command);
        //    FlashcartProxy proxy = new FlashcartProxy();
        //    //Console.Write("flash:>");
        //    //string command = Console.ReadLine();
        //    //ParseResult result = new ComLynxCommand().Parse(command);
        //    string response = proxy.SendMessageAndReceiveText(portName, baudRate);

        //    if (response.EndsWith(OK_TERMINATOR))
        //    {
        //        response = response.Substring(0, response.Length - OK_TERMINATOR.Length - 1).Trim();
        //    }
        //    Console.WriteLine(response);
        //}

        //private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    ComLynxReceiveStatus status = (ComLynxReceiveStatus)e.UserState;
        //    progressBar.Tick(e.ProgressPercentage, $"Received {status.BytesRead}/{status.TotalBytesToRead}");
        //}
    }
}
