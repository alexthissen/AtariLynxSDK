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
        public byte[] ConvertLynxPalette(byte[] palette)
        {
            return null;
        }

        public Bitmap ConvertToBitmap(byte[] picture)
        {
            Bitmap screenshot = new Bitmap(160, 102, PixelFormat.Format4bppIndexed);
            ColorPalette palette = screenshot.Palette;
            for (int index = 0; index < 16; index++)
            {
                palette.Entries[15 - index] = Color.FromArgb(
                    16 * (picture[index + 2] & 0x0f), // Red
                    16 * (picture[index + 18] & 0x0f),  // Green
                    16 * (picture[index + 2] >> 4));  // Blue
            }
            screenshot.Palette = palette; // you need to re-set this property to force the new ColorPalette

            BitmapData data = screenshot.LockBits(new Rectangle(Point.Empty, screenshot.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
            int byteCount = Math.Abs(data.Stride) * data.Height;
            IntPtr ptr = data.Scan0;
            byte[] pixels = new byte[byteCount];
            Marshal.Copy(picture, 34, ptr, byteCount);

            return screenshot;
        }

        public Bitmap ConvertToBitmap2(byte[] picture)
        {
            Bitmap screenshot = new Bitmap(160, 102, PixelFormat.Format4bppIndexed);
            //ColorPalette pal = screenshot.Palette;
            //for (int i = 0; i <= 16; i++)
            //{
            //    // create greyscale color table
            //    pal.Entries[i] = Color.FromArgb(i, i, i);
            //}
            //screenshot.Palette = pal; // you need to re-set this property to force the new ColorPalette

            int index = 34;
            Color[] palette = new Color[16];
            for (int pal = 0; pal < 16; pal++)
            {
                palette[15 - pal] = Color.FromArgb(
                    16 * (picture[pal + 2] & 0x0f), // Red
                    16 * (picture[pal + 18] & 0x0f),  // Green
                    16 * (picture[pal + 2] >> 4));  // Blue
            }

            for (int y = 0; y < 102; y++)
            {
                for (int x = 0; x < 80; x++)
                {
                    int color1 = (picture[index] & 0xf0) >> 4;
                    int color2 = (picture[index] & 0x0f);
                    screenshot.SetPixel(x * 2, y, palette[color1]);
                    screenshot.SetPixel(x * 2 + 1, y, palette[color2]);
                    index++;
                }
            }

            return screenshot;
        }
    }
}
