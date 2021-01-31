using KillerApps.AtariLynx.CommandLine.Bll;
using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.CommandLine.FlashcartCommand;
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
            rootCommand.AddCommand(new FlashcartProxyCommand());

            var verboseOption = new Option<bool>("--verbose", "Show verbose output");
            verboseOption.AddAlias("-v");

            Argument<CardSuit> arg = new Argument<CardSuit>("suit");
            rootCommand.AddArgument(arg);
            rootCommand.TryAddGlobalOption(verboseOption);
            return await rootCommand.InvokeAsync(args);
        }
    }

    public enum CardSuit
    {
        Hearts, 
        Diamonds,
        Clubs,
        Spades
    }
}
