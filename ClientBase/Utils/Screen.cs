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

        public static Point[] FindPixels(Rectangle rect, Color pixelColor, int shadeVariation)
        {
            List<Point> points = new();

            using (Bitmap regionInBitmap = new(rect.Width, rect.Height, PixelFormat.Format24bppRgb))
            {
                using (Graphics graphics = Graphics.FromImage(regionInBitmap))
                {
                    graphics.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
                }

                BitmapData bitmapData = regionInBitmap.LockBits(new Rectangle(0, 0, regionInBitmap.Width, regionInBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int[] formattedColor = new int[] { pixelColor.B, pixelColor.G, pixelColor.R };
                IntPtr scan0 = bitmapData.Scan0;
                int stride = bitmapData.Stride;

                for (int y = 0; y < bitmapData.Height; y++)
                {
                    for (int x = 0; x < bitmapData.Width; x++)
                    {
                        int offset = (y * stride) + (x * 3);
                        byte blue = Marshal.ReadByte(scan0, offset);
                        byte green = Marshal.ReadByte(scan0, offset + 1);
                        byte red = Marshal.ReadByte(scan0, offset + 2);

                        if (blue >= (formattedColor[0] - shadeVariation) && blue <= (formattedColor[0] + shadeVariation) &&
                            green >= (formattedColor[1] - shadeVariation) && green <= (formattedColor[1] + shadeVariation) &&
                            red >= (formattedColor[2] - shadeVariation) && red <= (formattedColor[2] + shadeVariation))
                        {
                            Point point = new(rect.X + x, rect.Y + y - 8);
                            points.Add(point);
                        }
                    }
                }

                regionInBitmap.UnlockBits(bitmapData);
            }

            return points.ToArray();
        }
    }
}