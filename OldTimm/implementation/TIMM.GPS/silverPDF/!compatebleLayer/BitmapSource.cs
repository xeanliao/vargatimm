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
using System.IO;
using System.Security;
using Imaging = System.Windows.Media.Imaging;

namespace silverPDF._compatebleLayer
{
    public class BitmapSource: Imaging.BitmapSource
    {
        //public static readonly DependencyProperty PixelHeightProperty;
        //public static readonly DependencyProperty PixelWidthProperty;

        //internal Imaging.BitmapImage i;
        //internal Imaging.BitmapSource bs;
        internal Stream s;
        internal iTextSharp.text.Image ti;

        public BitmapSource()
        {
            //throw new NotImplementedException();
        }

        [Obsolete("Can't use this in Silverlight", true)]
        public BitmapSource(Imaging.BitmapSource source)
        {
            //bs = source;
            
            //throw new NotImplementedException();
        }

        public BitmapSource(Stream source)
        {
            s = source.CloneToMemoryStream();

            //bs = new Imaging.BitmapImage();
            ti = iTextSharp.text.Image.GetInstance(s.CloneToMemoryStream());
           // bs.SetSource(s.CloneToMemoryStream());

            //bs.SetSource(s);

            //throw new NotImplementedException();
        }

        public virtual double DpiX
        {
            get { return (double)ti.DpiX; }
        }
        public virtual double DpiY
        {
            get { return (double)ti.DpiY; }
        }
        //internal virtual BitmapSourceSafeMILHandle DUCECompatibleMILPtr { get; }
        public virtual PixelFormat Format
        {
            get { return PixelFormats.Bgr32; }
            set { }
        }
        public double Height
        {
            get { return PixelsToDIPs(ti.DpiY, (int)ti.Height); }
        }
        public double Width
        {
            get { return PixelsToDIPs(ti.DpiX, (int)ti.Width); }
        }
        public virtual bool IsDownloading { get; set; }
        //internal bool IsSourceCached { get; set; }
        //public override ImageMetadata Metadata { get; }
        //public virtual BitmapPalette Palette { [SecurityCritical] get;
        //internal override Size Size { get; set; }
        //internal object SyncObject { get; }
        //internal BitmapSourceSafeMILHandle WicSourceHandle { [SecurityCritical] get; [SecurityTreatAsSafe, SecurityCritical] set; }
        
        public new int PixelHeight
        {
            get { return (int)ti.Height; }
        }
        public new int PixelWidth
        {
            get { return (int)ti.Width; }
        }

        public new void SetSource(Stream streamSource)
        {
            ti = iTextSharp.text.Image.GetInstance(streamSource.CloneToMemoryStream());
            //i = new Imaging.BitmapImage();
            //i.SetSource(streamSource);
            s = streamSource;

            base.SetSource(streamSource);
        }

        protected static double PixelsToDIPs(double dpi, int pixels)
        {
            float denominator = (float)dpi;
            if ((denominator < 0f) || FloatUtil.IsCloseToDivideByZero(96f, denominator))
            {
                denominator = 96f;
            }
            return (double)(pixels * (96f / denominator));
        }
    }

    internal static class FloatUtil
    {
        internal static float FLT_EPSILON = 1.192093E-07f;
        internal static float FLT_MAX_PRECISION = 1.677722E+07f;
        internal static float INVERSE_FLT_MAX_PRECISION = (1f / FLT_MAX_PRECISION);

        public static bool AreClose(float a, float b)
        {
            if (a == b)
            {
                return true;
            }
            float num2 = ((Math.Abs(a) + Math.Abs(b)) + 10f) * FLT_EPSILON;
            float num = a - b;
            return ((-num2 < num) && (num2 > num));
        }

        public static bool IsCloseToDivideByZero(float numerator, float denominator)
        {
            return (Math.Abs(denominator) <= (Math.Abs(numerator) * INVERSE_FLT_MAX_PRECISION));
        }

        public static bool IsOne(float a)
        {
            return (Math.Abs((float)(a - 1f)) < (10f * FLT_EPSILON));
        }

        public static bool IsZero(float a)
        {
            return (Math.Abs(a) < (10f * FLT_EPSILON));
        }
    }


}
