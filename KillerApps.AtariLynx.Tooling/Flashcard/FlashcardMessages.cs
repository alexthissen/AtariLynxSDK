namespace KillerApps.AtariLynx.Tooling.Flashcard
{
    public static class FlashcardMessages
    {
        public const string NG = "= NG ===========================================================================";
        public const string OK = "= OK ===========================================================================";
        public const string StartUpload = "please start upload data";
        public const string StopUpload = "stop upload and press anykey";
        public const string VerifyFailed = "warning - verify not successfull";
        public const string VerifySuccess = "verify successfull";
        public const string EepromUploadPart1 = "address(hex) 000 - 1ff";
        public const string EepromUploadPart2 = "address(hex) 200 - 3ff(cancel press anykey)";
        public const string EepromUploadPart3 = "address(hex) 400 - 5ff(cancel press anykey)";
        public const string EepromUploadPart4 = "address(hex) 600 - 7ff(cancel press anykey)";
    }

    //public class FlashcardStatus
    //{
    //    public string Description { get; set; }
    //    public 
    //}
}

/*
FLASH
content: bin-file
[w] write       [v] verify
93C86 
[u] write       [y] verify      [m] modify byte [r] read        [e] erase
BAUDRATE
[9] 9600        [5] 57600       [X] 115200
MODE
[g] 128k        [X] 256k/bll    [i] 512k        [k] 512k-bll
[l] lnx         [X] bin/lyx     [o] *.o
LANGUAGE
[X] english     [4] deutsch     [6] francais    [7] espanol     [8] nederlands
SYSTEM
[s] systeminfo  [c] credits     [x] reset all
= OK ===========================================================================

// "s", "System Info", "= OK ==========================================================================="
// "c", "Credits",""
// "9", "9600 Baud Set", "= OK ==========================================================================="
// "2", "19200 Baud Set", "= OK ==========================================================================="
// "3", "38400 Baud Set", "= OK ==========================================================================="
// "5", "57600 Baud Set", "= OK ==========================================================================="
// "1", "115200 Baud Set", "= OK ==========================================================================="
// "g", "128K ROM Size Set","");
// "h", "256K/BLL ROM Size Set","");
// "i", "512K ROM Size Set","");
// "k", "512K-BLL ROM Size Set","");
// "b", ".BIN / .LYX ROM Type Set","");
// "l", ".LNX ROM Type Set","");
// "x", "Resetting","FLASH");

*/