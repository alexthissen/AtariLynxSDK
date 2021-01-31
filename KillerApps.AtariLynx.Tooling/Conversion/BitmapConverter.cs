using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.Conversion
{
    public class BitmapConverter
    {
        public Bitmap ConvertToBitmap(byte[] picture)
        {
            Bitmap screenshot = new Bitmap(160, 102, PixelFormat.Format4bppIndexed);
            ColorPalette palette = screenshot.Palette;
            screenshot.Palette = palette;

            BitmapData data = screenshot.LockBits(new Rectangle(Point.Empty, screenshot.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
            int byteCount = Math.Abs(data.Stride) * data.Height;
            IntPtr ptr = data.Scan0;
            byte[] pixels = new byte[byteCount];
            Marshal.Copy(picture, 34, ptr, byteCount);

            return screenshot;
        }
    }

    public static class ColorPaletteExtensions
    {
        public static void ConvertLynxPalette(this ColorPalette palette, byte[] lynxPalette)
        {
            for (int index = 0; index < 16; index++)
            {
                palette.Entries[15 - index] = Color.FromArgb(
                    16 * (lynxPalette[index] & 0x0f), // Red
                    16 * (lynxPalette[index + 16] & 0x0f),  // Green
                    16 * (lynxPalette[index] >> 4));  // Blue
            }
       }
    }
    

}
