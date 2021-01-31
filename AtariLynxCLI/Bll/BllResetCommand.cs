using KillerApps.AtariLynx.Tooling.ComLynx;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Bll
{
    public class BllResetCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 9600;

        public BllResetCommand() : base("reset", "Reset debug command") {
            Option<string> portOption = new Option<string>("--portname", "Portname");
            portOption.AddAlias("-p");
            portOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new [] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for ComLynx");

            this.AddOption(portOption);
            this.AddOption(baudRateOption);
            this.Handler = CommandHandler.Create<string, int>(BllResetHandler);
        }

        private void BllResetHandler(string portName, int baudRate)
        {
            BllComLynxClient uploader = new BllComLynxClient();
            uploader.ResetProgram(portName, baudRate);
        }
    }
}
