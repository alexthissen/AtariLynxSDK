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
            Option<int> comPortOption = new Option<int>("--comport");
            comPortOption.AddAlias("-p");
            comPortOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new [] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for ComLynx");

            this.AddOption(comPortOption);
            this.AddOption(baudRateOption);
            this.Handler = CommandHandler.Create<int, int>(BllResetHandler);
        }

        private void BllResetHandler(int comPort, int baudRate)
        {
            BllComLynxClient uploader = new BllComLynxClient();
            string comPortName = String.Format("COM{0}", comPort);
            uploader.ResetProgram(comPortName, baudRate);
        }
    }
}
