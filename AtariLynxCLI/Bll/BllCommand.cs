using System.CommandLine;

namespace KillerApps.AtariLynx.CommandLine.Bll;

public class BllCommand : Command
{
    public BllCommand() : base("bll", "BLL context") 
    {
        this.AddCommand(new BllUploadCommand());
        this.AddCommand(new BllResetCommand());
        this.AddCommand(new BllScreenshotCommand());
    }
}
