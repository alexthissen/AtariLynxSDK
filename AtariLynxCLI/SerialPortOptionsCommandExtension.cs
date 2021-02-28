using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine
{
    public static class CommandOptionsExtension
    {
        private const int DEFAULT_BAUDRATE = 115200;

        public static void AddSerialPortOptions(this Command command)
        {
            Option<string> portOption = new Option<string>("--portname", "Portname");
            portOption.AddAlias("-p");
            portOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new[] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for FlashCard");
            command.AddOption(portOption);
            command.AddOption(baudRateOption);
        }
    }
}
