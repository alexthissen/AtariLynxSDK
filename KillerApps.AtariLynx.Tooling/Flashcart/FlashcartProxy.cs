using System;
using System.Collections.Generic;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.Flashcart
{
    public class FlashcartProxy
    {
        public const string FLASHCART_SYSTEMINFO = "s";
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

        public void GetSystemInfo()
        {
            //SendMessage()
        }


    }
}
