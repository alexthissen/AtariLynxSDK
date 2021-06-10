using System.IO;

namespace KillerApps.AtariLynx.CommandLine.Flashcard
{
    public class FlashcardWriteOptions
    {
        public FileInfo RomFile { get; set; }
        public bool Force { get; set; }
    }
}