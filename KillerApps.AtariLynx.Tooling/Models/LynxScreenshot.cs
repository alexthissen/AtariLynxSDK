using System;
using System.Collections.Generic;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.Models
{
    public class LynxScreenshot
    {
        public const ushort SCREENSHOT_SIZE = 102 * 160 / 2;

        public byte[] Palette = new byte[32];
        public byte[] Pixels = new byte[SCREENSHOT_SIZE];

        public LynxScreenshot FromByteArray(byte[] data)
        {
            LynxScreenshot screenshot = new LynxScreenshot();
            return null;
        }
    }
}
