using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine
{
    public static class CommandOptionsExtension
    {
        public static Command AddSerialPortOptions(this Command command, int defaultBaudrate)
        {
            Option<string> portOption = new Option<string>("--portname", "Name of serial port");
            portOption.AddAlias("-p");
            portOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new[] { "--baudrate", "-b" }, () => defaultBaudrate, "Baudrate for serial port");
            command.AddOption(portOption);
            command.AddOption(baudRateOption);
            return command;
        }
    }
}
