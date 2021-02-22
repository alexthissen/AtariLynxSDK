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
    public enum Language
    {
        English = 0,
        Deutsch = 4,
        Francais = 6,
        Espanol = 7,
        Nederlands = 8
    }

    public enum Modus
    {
        Lnx,
        Bin,
        Lyx,
        O
    }

    //public enum Size
    //{
    //    Auto,
    //    128k,
    //    256k,
    //    bll,
    //    512k,
    //    512kbll
    //}

    public class FlashcardSettings
    {
        public Language? Language { get; set; }
        public int? Rate { get; set; }
        public Modus? Modus { get; set; }
    }

    public class FlashcartSettingsCommand : Command
    {
        private ProgressBar progressBar = null;

        public FlashcartSettingsCommand() : base("set", "Flashcart settings") {
            Option<Language> languageOption = new Option<Language>("--language", "Language setting");
            languageOption.AddAlias("-l");
            this.AddOption(languageOption);

            Option<int> rateOption = new Option<int>("--rate");
            rateOption.AddAlias("-r");
            rateOption.FromAmong("9600", "57600", "115200");
            this.AddOption(rateOption);

            Option<string> modusOption = new Option<string>("--modus", "Modus");
            modusOption.AddAlias("-m");
            modusOption.FromAmong("lnx", "bin", "lyx", "o");
            this.AddOption(modusOption);

            Option<string> sizeOption = new Option<string>("--size", "Size");
            sizeOption.AddAlias("-s");
            sizeOption.FromAmong("auto", "128k", "256k", "bll", "512k", "512k-bll");
            this.AddOption(sizeOption);

            //Argument<string> arg = new Argument<string>("command", "Command to send to Flashcart");
            //this.AddArgument(arg);
            //this.AddArgument(new Argument<string>("command2"));
            this.AddValidator(cmd => {
                if (cmd.ValueForOption("size") != null) return "yes"; else return null;
            });
            this.Handler = CommandHandler.Create<GlobalOptions, SerialPortOptions, FlashcardSettings>(FlashcartProxyHandler);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FlashCardWriteStatus status = (FlashCardWriteStatus)e.UserState;
            progressBar.Tick(e.ProgressPercentage, $"Writing {status.BytesWritten}/{status.TotalBytes} bytes");
        }

        private void FlashcartProxyHandler(GlobalOptions global, SerialPortOptions serialPortOptions, FlashcardSettings settings)
        {
            FlashcartClient proxy = new FlashcartClient();

            string response = String.Empty;
            if (settings.Language.HasValue)
            {
                response = proxy.SendMessageAndReceiveText(serialPortOptions.PortName, serialPortOptions.Baudrate, '4');
            }
            if (settings.Rate.HasValue)
            {
                response = proxy.SendMessageAndReceiveText(serialPortOptions.PortName, serialPortOptions.Baudrate, '5');
            }
            if (settings.Modus.HasValue)
            {
                response = proxy.SendMessageAndReceiveText(serialPortOptions.PortName, serialPortOptions.Baudrate, 'b');
            }

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
