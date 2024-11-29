using KillerApps.AtariLynx.CommandLine.Bll;
using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.CommandLine.Flashcard;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Threading.Tasks;
using Spectre.Console;

namespace KillerApps.AtariLynx.CommandLine
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand("Atari Lynx Command-line Interface");
            rootCommand.AddCommand(new ComLynxCommand());
            rootCommand.AddCommand(new BllCommand());
            rootCommand.AddCommand(new FlashcardCommand());

            // Show command-line help unless a subcommand was used.
            rootCommand.Handler = CommandHandler.Create(() => rootCommand.Invoke("-h"));

            var verboseOption = new Option<bool>("--verbose", "Show verbose output");
            verboseOption.AddAlias("-v");
            rootCommand.AddGlobalOption(verboseOption);

            var builder = new CommandLineBuilder(rootCommand)
                .UseVersionOption()
                .UseHelp()
                .UseParseDirective()
                //.UseDebugDirective()
                .UseEnvironmentVariableDirective()
                .UseSuggestDirective()
                .RegisterWithDotnetSuggest()
                .UseTypoCorrections()
                .UseParseErrorReporting()
                // Removed from beta1
                //.ParseResponseFileAs(ResponseFileHandling.ParseArgsAsSpaceSeparated)
                .CancelOnProcessTermination()
                .UseExceptionHandler(HandleException)
                .AddMiddleware(async (context, next) => {
                    context.BindingContext.AddService<IAnsiConsole>(provider => AnsiConsole.Console);
                    await next(context);
                }, MiddlewareOrder.Configuration);

            //builder.UseDefaults();
            //builder.UseMiddleware(DefaultOptionsMiddleware);

            var parser = builder.Build();
            return await parser.InvokeAsync(args);

            //return await rootCommand.InvokeAsync(args);
        }

        private static void HandleException(Exception exception, InvocationContext context)
        {
            context.Console.ResetTerminalForegroundColor();
            context.Console.SetTerminalForegroundColor(ConsoleColor.Red);

            if (exception is TargetInvocationException tie && tie.InnerException is object)
            {
                exception = tie.InnerException;
            }

            if (exception is OperationCanceledException)
            {
                context.Console.Error.WriteLine("Operation has been canceled.");
            }
            else if (exception is CommandException command)
            {
                context.Console.Error.WriteLine($"Command '{context.ParseResult.CommandResult.Command.Name}' failed:");
                context.Console.Error.WriteLine($"\t{command.Message}");

                if (command.InnerException != null)
                {
                    context.Console.Error.WriteLine();
                    context.Console.Error.WriteLine(command.InnerException.ToString());
                }
            }

            context.Console.ResetTerminalForegroundColor();
            context.ExitCode = 1;
        }
    }
}

//private static Command WithHandler(this Command command, string methodName)
//{
//    var method = typeof(Program).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
//    var handler = CommandHandler.Create(method!);
//    command.Handler = handler;
//    return command;
//}