using System.IO;

namespace KillerApps.AtariLynx.CommandLine.Flashcard
{
    public class EepromReadOptions
    {
        public FileInfo File { get; set; }
        public int Size { get; set; }
    }
}