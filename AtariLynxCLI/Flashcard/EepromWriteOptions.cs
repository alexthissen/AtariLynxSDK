using System.IO;

namespace KillerApps.AtariLynx.CommandLine.Flashcard
{
    public class EepromWriteOptions
    {
        public FileInfo File { get; set; }
        public int Size { get; set; }
    }
}