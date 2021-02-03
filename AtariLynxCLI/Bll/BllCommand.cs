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
    public class BllCommand : Command
    {
        public BllCommand() : base("bll", "BLL context") 
        {
            Option<string> portOption = new Option<string>("--comport", "Portname");
            portOption.AddAlias("-c");
            portOption.IsRequired = true;

            this.AddOption(portOption);

            this.AddCommand(new BllUploadCommand());
            this.AddCommand(new BllResetCommand());
            this.AddCommand(new BllScreenshotCommand());
        }
    }
}
