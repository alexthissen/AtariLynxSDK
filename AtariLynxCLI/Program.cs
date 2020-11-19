using KillerApps.AtariLynx.CommandLine.Bll;
using KillerApps.AtariLynx.CommandLine.ComLynx;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace KillerApps.AtariLynx.CommandLine
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand("Atari Lynx Command-line Interface");
            rootCommand.AddCommand(new ComLynxCommand());
            rootCommand.AddCommand(new BllUploadCommand());
            rootCommand.AddCommand(new BllResetCommand());
            rootCommand.AddCommand(new BllScreenshotCommand());

            return await rootCommand.InvokeAsync(args);
        }
    }
}
