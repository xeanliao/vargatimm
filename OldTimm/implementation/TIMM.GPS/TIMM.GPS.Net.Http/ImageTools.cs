using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;

namespace TIMM.GPS.Silverlight.Utility
{
    public static class ImageTools
    {
        public static void SaveToBMPImage(this UIElement element, Stream fileStream)
        {
            WriteableBitmap bitmap = new WriteableBitmap(element, null);
            
            bitmap.Invalidate();

            EncodeToBMPImage(bitmap.Pixels, bitmap.PixelWidth, bitmap.PixelHeight, fileStream);
        }

        public static void SaveToBMPImage(this UIElement element, Stream fileStream, Color backgroundColor)
        {
            WriteableBitmap bitmap = new WriteableBitmap((int)element.RenderSize.Width, (int)element.RenderSize.Height);
            Rectangle bg = new Rectangle
            {
                Width = element.RenderSize.Width,
                Height = element.RenderSize.Height,
                Fill = new SolidColorBrush(backgroundColor)
            };
            bitmap.Render(bg, null);
            bitmap.Render(element, null);
            bitmap.Invalidate();

            EncodeToBMPImage(bitmap.Pixels, bitmap.PixelWidth, bitmap.PixelHeight, fileStream);
        }
        
        public static void SaveToBMPImage(this WriteableBitmap bitmap, Stream fileStream)
        {
            EncodeToBMPImage(bitmap.Pixels, bitmap.PixelWidth, bitmap.PixelHeight, fileStream);
        }

        public static void EncodeToBMPImage(int[] bitmapPixels, int width, int height, Stream fileStream)
        {
            byte[] pixels = new byte[width * height * 4];

            #region Read from writeableBitmap
            if (bitmapPixels != null)
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int pixelIndex = width * y + x;
                        int pixel = bitmapPixels[pixelIndex];

                        byte a = (byte)((pixel >> 24) & 0xFF);

                        float aFactor = a / 255f;

                        if (aFactor > 0)
                        {
                            byte r = (byte)(((pixel >> 16) & 0xFF) / aFactor);
                            byte g = (byte)(((pixel >> 8) & 0xFF) / aFactor);
                            byte b = (byte)(((pixel >> 0) & 0xFF) / aFactor);

                            pixels[pixelIndex * 4 + 0] = b;
                            pixels[pixelIndex * 4 + 1] = g;
                            pixels[pixelIndex * 4 + 2] = r;
                            pixels[pixelIndex * 4 + 3] = 0x00;
                        }
                    }
                }
            }
            #endregion

            #region BMP File Header(14 bytes)
            //the magic number(2 bytes):BM
            fileStream.WriteByte(0x42);
            fileStream.WriteByte(0x4D);

            //the size of the BMP file in bytes(4 bytes)
            long len = pixels.Length * 4 + 0x36;

            fileStream.WriteByte((byte)len);
            fileStream.WriteByte((byte)(len >> 8));
            fileStream.WriteByte((byte)(len >> 16));
            fileStream.WriteByte((byte)(len >> 24));

            //reserved(2 bytes)
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);

            //reserved(2 bytes)
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);

            //the offset(4 bytes)
            fileStream.WriteByte(0x36);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            #endregion

            #region Bitmap Information(40 bytes:Windows V3)
            //the size of this header(4 bytes)
            fileStream.WriteByte(0x28);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);

            //the bitmap width in pixels(4 bytes)
            fileStream.WriteByte((byte)width);
            fileStream.WriteByte((byte)(width >> 8));
            fileStream.WriteByte((byte)(width >> 16));
            fileStream.WriteByte((byte)(width >> 24));

            //the bitmap height in pixels(4 bytes)
            fileStream.WriteByte((byte)height);
            fileStream.WriteByte((byte)(height >> 8));
            fileStream.WriteByte((byte)(height >> 16));
            fileStream.WriteByte((byte)(height >> 24));

            //the number of color planes(2 bytes)
            fileStream.WriteByte(0x01);
            fileStream.WriteByte(0x00);

            //the number of bits per pixel(2 bytes)
            fileStream.WriteByte(0x20);
            fileStream.WriteByte(0x00);

            //the compression method(4 bytes)
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);

            //the image size(4 bytes)
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);

            //the horizontal resolution of the image(4 bytes)
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);

            //the vertical resolution of the image(4 bytes)
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);

            //the number of colors in the color palette(4 bytes)
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);

            //the number of important colors(4 bytes)
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            fileStream.WriteByte(0x00);
            #endregion

            #region Bitmap data
            byte[] buffer = new byte[width * 4];
            for (int y = height - 1; y >= 0; y--)
            {
                Array.Copy(pixels, width * y * 4, buffer, 0, width * 4);
                fileStream.Write(buffer, 0, buffer.Length);
            }
            #endregion
        }
    }
}
