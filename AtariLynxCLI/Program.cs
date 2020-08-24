using KillerApps.AtariLynx.Tooling.ComLynx;
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
            rootCommand.AddCommand(new BllCommand());
            rootCommand.AddCommand(new ResetCommand());

            return await rootCommand.InvokeAsync(args);
        }
    }


}
