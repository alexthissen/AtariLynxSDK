using KillerApps.AtariLynx.Tooling.ComLynx;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Bll
{
    public class BllResetCommand : Command
    {
        private const int DEFAULT_BAUDRATE = 9600;

        public BllResetCommand() : base("reset", "Reset debug command") {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);
            this.Handler = CommandHandler.Create<string, int>(BllResetHandler);
        }

        private void BllResetHandler(string portName, int baudRate)
        {
            BllComLynxClient uploader = new BllComLynxClient();
            uploader.ResetProgram(portName, baudRate);
        }
    }
}
