using KillerApps.AtariLynx.CommandLine.ComLynx;
using KillerApps.AtariLynx.Tooling.ComLynx;
using KillerApps.AtariLynx.Tooling.Flashcard;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.Flashcard
{
    public class FlashcardCreditsCommand : Command
    {
        public FlashcardCreditsCommand() : base("credits", "Credits for Flashcard") 
        {
            this.Handler = CommandHandler.Create(() => {
                FlashcardClient client = new FlashcardClient();
                string response = client.SendMessageAndReceiveText("COM4", 115200, 'c');
                Console.WriteLine(response);
            });
        }
    }
}
