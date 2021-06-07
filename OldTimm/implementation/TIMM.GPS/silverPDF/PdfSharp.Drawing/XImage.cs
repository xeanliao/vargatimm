/*
silverPDF is sponsored by Aleyant Systems (http://www.aleyant.com)

silverPDF is based on PdfSharp (http://www.pdfsharp.net) and iTextSharp (http://itextsharp.sourceforge.net)

Developers: Ai_boy (aka Oleksii Okhrymenko)

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above information and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR SPONSORS
BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

*/
#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange (mailto:Stefan.Lange@pdfsharp.com)
//
// Copyright (c) 2005-2008 empira Software GmbH, Cologne (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
#endif
using PdfSharp.Internal;
using PdfSharp.Fonts.TrueType;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Advanced;
using System.Resources;
using System.Globalization;

// WPFHACK
#pragma warning disable 162

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Defines an object used to draw image files (bmp, png, jpeg, gif) and PDF forms.
    /// An abstract base class that provides functionality for the Bitmap and Metafile descended classes.
    /// </summary>
    public class XImage : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XImage"/> class.
        /// </summary>
        protected XImage()
        {
        }

#if GDI
    /// <summary>
    /// Initializes a new instance of the <see cref="XImage"/> class from a GDI+ image.
    /// </summary>
    XImage(Image image)
    {
      this.gdiImage = image;
#if WPF
      this.wpfImage = ImageHelper.CreateBitmapSource(image);
#endif
      Initialize();
    }
#endif

#if WPF
        /// <summary>
        /// Initializes a new instance of the <see cref="XImage"/> class from a WPF image.
        /// </summary>
        [Obsolete("Can't use this in Silverlight!", true)]
        XImage(System.Windows.Media.Imaging.BitmapSource image)
        {
            var i = new silverPDF._compatebleLayer.BitmapSource(image);
            this.wpfImage = i;
            Initialize();
        }
#endif

        [Obsolete("Can't use this in Silverlight!", true)]
        XImage(string path)
        {
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
                throw new FileNotFoundException(PSSR.FileNotFound(path));

            this.path = path;

            //FileStream file = new FileStream(filename, FileMode.Open);
            //BitsLength = (int)file.Length;
            //Bits = new byte[BitsLength];
            //file.Read(Bits, 0, BitsLength);
            //file.Close();
#if GDI
      this.gdiImage = Image.FromFile(path);
#endif
#if WPF
            //Sthis.wpfImage = new PDFSharp4SL._compatebleLayer.BitmapImage(new Uri(path));
#endif

#if false
      float vres = this.image.VerticalResolution;
      float hres = this.image.HorizontalResolution;
      SizeF size = this.image.PhysicalDimension;
      int flags  = this.image.Flags;
      Size sz    = this.image.Size;
      GraphicsUnit units = GraphicsUnit.Millimeter;
      RectangleF rect = this.image.GetBounds(ref units);
      int width = this.image.Width;
#endif
            Initialize();
        }

        XImage(Stream stream)
        {
            // Create a dummy unique path
            this.path = "*" + Guid.NewGuid().ToString("B");

#if GDI
      this.gdiImage = Image.FromStream(stream);
#endif
#if WPF
            this.wpfImage = new silverPDF._compatebleLayer.BitmapSource(stream);
            this.fakeImage = iTextSharp.text.Image.GetInstance(stream);
            //this.wpfImage = new BitmapImage(new Uri(path));
#endif

#if true_
      float vres = this.image.VerticalResolution;
      float hres = this.image.HorizontalResolution;
      SizeF size = this.image.PhysicalDimension;
      int flags  = this.image.Flags;
      Size sz    = this.image.Size;
      GraphicsUnit units = GraphicsUnit.Millimeter;
      RectangleF rect = this.image.GetBounds(ref units);
      int width = this.image.Width;
#endif
            Initialize();
        }

#if GDI
#if UseGdiObjects
    /// <summary>
    /// Implicit conversion from Image to XImage.
    /// </summary>
    public static implicit operator XImage(Image image)
    {
      return new XImage(image);
    }
#endif

    /// <summary>
    /// Conversion from Image to XImage.
    /// </summary>
    public static XImage FromGdiPlusImage(Image image)
    {
      return new XImage(image);
    }
#endif

#if WPF
        /// <summary>
        /// Conversion from BitmapSource to XImage.
        /// </summary>
        [Obsolete("Can't use this in Silverlight!", true)]
        public static XImage FromBitmapSource(System.Windows.Media.Imaging.BitmapSource image)
        {
            return new XImage(image);
        }

        /// <summary>
        /// Creates an image from file stream
        /// </summary>
        public static XImage FromStream(Stream stream)
        {
            return new XImage(stream);
        }
#endif

        /// <summary>
        /// Creates an image from the specified file.
        /// </summary>
        /// <param name="path">The path to a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        [Obsolete("Can't use this in Silverlight!", true)]
        public static XImage FromFile(string path)
        {
            if (PdfReader.TestPdfFile(path) > 0)
                return new XPdfForm(path);
            return new XImage(path);
        }

        /// <summary>
        /// Tests if a file exist. Supports PDF files with page number suffix.
        /// </summary>
        /// <param name="path">The path to a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        public static bool ExistsFile(string path)
        {
            if (PdfReader.TestPdfFile(path) > 0)
                return true;
            return File.Exists(path);
        }

        void Initialize()
        {
#if GDI
      if (this.gdiImage != null)
      {
        // ImageFormat has no overridden Equals...
        string guid = this.gdiImage.RawFormat.Guid.ToString("B").ToUpper();
        switch (guid)
        {
          case "{B96B3CAA-0728-11D3-9D7B-0000F81EF32E}":  // memoryBMP
          case "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}":  // bmp
          case "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}":  // png
            this.format = XImageFormat.Png;
            break;

          case "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}":  // jpeg
            this.format = XImageFormat.Jpeg;
            break;

          case "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}":  // gif
            this.format = XImageFormat.Gif;
            break;

          case "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}":  // tiff
            this.format = XImageFormat.Tiff;
            break;

          case "{B96B3CB5-0728-11D3-9D7B-0000F81EF32E}":  // icon
            this.format = XImageFormat.Icon;
            break;

          case "{B96B3CAC-0728-11D3-9D7B-0000F81EF32E}":  // emf
          case "{B96B3CAD-0728-11D3-9D7B-0000F81EF32E}":  // wmf
          case "{B96B3CB2-0728-11D3-9D7B-0000F81EF32E}":  // exif
          case "{B96B3CB3-0728-11D3-9D7B-0000F81EF32E}":  // photoCD
          case "{B96B3CB4-0728-11D3-9D7B-0000F81EF32E}":  // flashPIX

          default:
            throw new InvalidOperationException("Unsupported image format.");
        }
        return;
      }
#endif
#if WPF
            if (this.wpfImage != null)
            {
                string pixelFormat = this.wpfImage.Format.ToString();
                switch (pixelFormat)
                {
                    case "Bgr32":
                    case "Bgra32":
                    case "Pbgra32":
                    case "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}":  // bmp
                    case "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}":  // png
                        this.format = XImageFormat.Png;
                        break;

                    case "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}":  // jpeg
                        this.format = XImageFormat.Jpeg;
                        break;

                    case "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}":  // gif
                    case "BlackWhite":
                    case "Indexed1":
                    case "Indexed4":
                    case "Indexed8":
                        this.format = XImageFormat.Gif;
                        break;

                    case "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}":  // tiff
                        this.format = XImageFormat.Tiff;
                        break;

                    case "{B96B3CB5-0728-11D3-9D7B-0000F81EF32E}":  // icon
                        this.format = XImageFormat.Icon;
                        break;

                    case "{B96B3CAC-0728-11D3-9D7B-0000F81EF32E}":  // emf
                    case "{B96B3CAD-0728-11D3-9D7B-0000F81EF32E}":  // wmf
                    case "{B96B3CB2-0728-11D3-9D7B-0000F81EF32E}":  // exif
                    case "{B96B3CB3-0728-11D3-9D7B-0000F81EF32E}":  // photoCD
                    case "{B96B3CB4-0728-11D3-9D7B-0000F81EF32E}":  // flashPIX

                    default:
                        Debug.Assert(false, "Unknown pixel format: " + pixelFormat);
                        this.format = XImageFormat.Gif;
                        break;// throw new InvalidOperationException("Unsupported image format.");
                }
            }
#endif
        }

        /// <summary>
        /// under construction
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes underlying GDI+ object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
                this.disposed = true;

#if GDI
      if (this.gdiImage != null)
      {
        this.gdiImage.Dispose();
        this.gdiImage = null;
      }
#endif
#if WPF
#endif
        }
        bool disposed;


        /// <summary>
        /// Gets the width of the image.
        /// </summary>
        [Obsolete("Use either PixelWidth or PointWidth. Temporarily obsolete because of rearrangements for WPF. Currently same as PixelWidth, but will become PointWidth in future releases of PDFsharp.")]
        public virtual double Width
        {
            get
            {
#if GDI && WPF
        double gdiWidth = this.gdiImage.Width;
        double wpfWidth = this.wpfImage.PixelWidth;
        Debug.Assert(gdiWidth == wpfWidth);
        return wpfWidth;
#endif
#if GDI && !WPF
        return this.gdiImage.Width;
#endif
#if WPF && !GDI
                return this.wpfImage.PixelWidth;
#endif
            }
        }

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        [Obsolete("Use either PixelHeight or PointHeight. Temporarily obsolete because of rearrangements for WPF. Currently same as PixelHeight, but will become PointHeight in future releases of PDFsharp.")]
        public virtual double Height
        {
            get
            {
#if GDI && WPF
        double gdiHeight = this.gdiImage.Height;
        double wpfHeight = this.wpfImage.PixelHeight;
        Debug.Assert(gdiHeight == wpfHeight);
        return wpfHeight;
#endif
#if GDI && !WPF
        return this.gdiImage.Height;
#endif
#if WPF && !GDI
                return this.wpfImage.PixelHeight;
#endif
            }
        }

        /// <summary>
        /// Gets the width of the image in point.
        /// </summary>
        public virtual double PointWidth
        {
            get
            {
#if GDI && WPF
        double gdiWidth = this.gdiImage.Width * 72 / this.gdiImage.HorizontalResolution;
        double wpfWidth = this.wpfImage.Width * 72.0 / 96.0;
        //Debug.Assert(gdiWidth == wpfWidth);
        Debug.Assert(DoubleUtil.AreRoughlyEqual(gdiWidth, wpfWidth, 5));
        return wpfWidth;
#endif
#if GDI && !WPF
        return this.gdiImage.Width * 72 / this.gdiImage.HorizontalResolution;
#endif
#if WPF && !GDI
                //Debug.Assert(Math.Abs(this.wpfImage.PixelWidth * 72 / this.wpfImage.DpiX - this.wpfImage.Width * 72.0 / 96.0) < 0.001);
                return this.wpfImage.Width * 72.0 / 96.0;
#endif
            }
        }

        /// <summary>
        /// Gets the height of the image in point.
        /// </summary>
        public virtual double PointHeight
        {
            get
            {
#if GDI && WPF
        double gdiHeight = this.gdiImage.Height * 72 / this.gdiImage.HorizontalResolution;
        double wpfHeight = this.wpfImage.Height * 72.0 / 96.0;
        Debug.Assert(DoubleUtil.AreRoughlyEqual(gdiHeight, wpfHeight, 5));
        return wpfHeight;
#endif
#if GDI && !WPF
        return this.gdiImage.Height * 72 / this.gdiImage.HorizontalResolution;
#endif
#if WPF && !GDI
                //Debug.Assert(Math.Abs(this.wpfImage.PixelHeight * 72 / this.wpfImage.DpiY - this.wpfImage.Height * 72.0 / 96.0) < 0.001);
                return this.wpfImage.Height * 72.0 / 96.0;
#endif
            }
        }

        /// <summary>
        /// Gets the width of the image in pixels.
        /// </summary>
        public virtual int PixelWidth
        {
            get
            {
#if GDI && WPF
        int gdiWidth = this.gdiImage.Width;
        int wpfWidth = this.wpfImage.PixelWidth;
        Debug.Assert(gdiWidth == wpfWidth);
        return wpfWidth;
#endif
#if GDI && !WPF
        return this.gdiImage.Width;
#endif
#if WPF && !GDI
                return this.wpfImage.PixelWidth;
#endif
            }
        }

        /// <summary>
        /// Gets the height of the image in pixels.
        /// </summary>
        public virtual int PixelHeight
        {
            get
            {
#if GDI && WPF
        int gdiHeight = this.gdiImage.Height;
        int wpfHeight = this.wpfImage.PixelHeight;
        Debug.Assert(gdiHeight == wpfHeight);
        return wpfHeight;
#endif
#if GDI && !WPF
        return this.gdiImage.Height;
#endif
#if WPF && !GDI
                return this.wpfImage.PixelHeight;
#endif
            }
        }

        /// <summary>
        /// Gets the size in point of the image.
        /// </summary>
        public virtual XSize Size
        {
            get { return new XSize(PointWidth, PointHeight); }
        }

        /// <summary>
        /// Gets the horizontal resolution of the image.
        /// </summary>
        public virtual double HorizontalResolution
        {
            get
            {
#if GDI && WPF
        double gdiResolution = this.gdiImage.HorizontalResolution;
        double wpfResolution = this.wpfImage.PixelWidth * 96.0 / this.wpfImage.Width;
        Debug.Assert(gdiResolution == wpfResolution);
        return wpfResolution;
#endif
#if GDI && !WPF
        return this.gdiImage.HorizontalResolution;
#endif
#if WPF && !GDI
                return this.wpfImage.DpiX; //.PixelWidth * 96.0 / this.wpfImage.Width;
#endif
            }
        }

        /// <summary>
        /// Gets the vertical resolution of the image.
        /// </summary>
        public virtual double VerticalResolution
        {
            get
            {
#if GDI && WPF
        double gdiResolution = this.gdiImage.VerticalResolution;
        double wpfResolution = this.wpfImage.PixelHeight * 96.0 / this.wpfImage.Height;
        Debug.Assert(gdiResolution == wpfResolution);
        return wpfResolution;
#endif
#if GDI && !WPF
        return this.gdiImage.VerticalResolution;
#endif
#if WPF && !GDI
                return this.wpfImage.DpiY; //.PixelHeight * 96.0 / this.wpfImage.Height;
#endif
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether image interpolation is to be performed. 
        /// </summary>
        public virtual bool Interpolate
        {
            get { return this.interpolate; }
            set { this.interpolate = value; }
        }
        bool interpolate = true;

        /// <summary>
        /// Gets the format of the image.
        /// </summary>
        public XImageFormat Format
        {
            get { return this.format; }
        }
        XImageFormat format;

#if GDI
    internal Image gdiImage;
#endif
#if WPF
        internal silverPDF._compatebleLayer.BitmapSource wpfImage;
        internal iTextSharp.text.Image fakeImage;
#endif

        /// <summary>
        /// If path starts with '*' the image is created from a stream and the path is a GUID.
        /// </summary>
        internal string path;

        /// <summary>
        /// Cache PdfImageTable.ImageSelector to speed up finding the right PdfImage
        /// if this image is used more than once.
        /// </summary>
        internal PdfImageTable.ImageSelector selector;
    }
}