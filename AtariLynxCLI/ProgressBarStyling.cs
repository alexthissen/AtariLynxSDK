using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine
{
    public class ProgressBarStyling
    {
        public static readonly ProgressBarOptions Options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Yellow,
            ForegroundColorDone = ConsoleColor.DarkGreen,
            BackgroundColor = ConsoleColor.DarkGray,
            BackgroundCharacter = '\u2593'
        };
    }
}
