namespace KillerApps.AtariLynx.Tooling.Flashcart
{
    public class FlashCardWriteStatus
    {
        public string Log { get; set; }
        public int BytesWritten { get; set; }
        public int TotalBytes { get; set; }
    }
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