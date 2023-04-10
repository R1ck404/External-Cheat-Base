using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Client.ClientBase.Utils
{
    public static class Screen
    {
        public static Point FindText(string searchText)
        {
            Rectangle screenBounds = new(0, 0, 1920, 1080);

            Bitmap screenshot = new(screenBounds.Width, screenBounds.Height);
            Graphics graphics = Graphics.FromImage(screenshot);
            graphics.CopyFromScreen(screenBounds.X, screenBounds.Y, 0, 0, screenBounds.Size);

            for (int x = 0; x < screenshot.Width; x++)
            {
                for (int y = 0; y < screenshot.Height; y++)
                {
                    if (screenshot.GetPixel(x, y).ToArgb() == Color.Black.ToArgb())
                    {
                        bool found = true;
                        for (int i = 1; i < searchText.Length; i++)
                        {
                            if (x + i >= screenshot.Width || screenshot.GetPixel(x + i, y).ToArgb() != Color.Black.ToArgb())
                            {
                                found = false;
                                break;
                            }
                        }
                        if (found)
                        {
                            return new Point(x, y);
                        }
                    }
                }
            }
            return Point.Empty;
        }

        public static Point[] FindPixels(Rectangle rect, Color Pixel_Color, int Shade_Variation)
        {
            System.Collections.ArrayList points = new();
            Bitmap RegionIn_Bitmap = new(rect.Width, rect.Height, PixelFormat.Format24bppRgb);

            using (Graphics graphics = Graphics.FromImage(RegionIn_Bitmap))
            {
                graphics.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }

            BitmapData RegionIn_BitmapData = RegionIn_Bitmap.LockBits(new Rectangle(0, 0, RegionIn_Bitmap.Width, RegionIn_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] Formatted_Color = new int[3] { Pixel_Color.B, Pixel_Color.G, Pixel_Color.R };

            IntPtr Scan0 = RegionIn_BitmapData.Scan0;
            int stride = RegionIn_BitmapData.Stride;

            for (int y = 0; y < RegionIn_BitmapData.Height; y++)
            {
                for (int x = 0; x < RegionIn_BitmapData.Width; x++)
                {
                    int offset = (y * stride) + (x * 3);
                    byte blue = Marshal.ReadByte(Scan0, offset);
                    byte green = Marshal.ReadByte(Scan0, offset + 1);
                    byte red = Marshal.ReadByte(Scan0, offset + 2);

                    if (blue >= (Formatted_Color[0] - Shade_Variation) && blue <= (Formatted_Color[0] + Shade_Variation) &&
                        green >= (Formatted_Color[1] - Shade_Variation) && green <= (Formatted_Color[1] + Shade_Variation) &&
                        red >= (Formatted_Color[2] - Shade_Variation) && red <= (Formatted_Color[2] + Shade_Variation))
                    {
                        points.Add(new Point(rect.X + x, rect.Y + y - 8));
                    }
                }
            }

            RegionIn_Bitmap.UnlockBits(RegionIn_BitmapData);

            return (Point[])points.ToArray(typeof(Point));
        }
    }
}