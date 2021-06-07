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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Generic;

namespace silverPDF._compatebleLayer
{
    [Serializable, 
    //StructLayout(LayoutKind.Sequential), 
    //TypeConverter(typeof(PixelFormatConverter))
    ]
    public struct PixelFormat : IEquatable<PixelFormat>
    {
        //[NonSerialized]
        private PixelFormatFlags m_flags;
        //[NonSerialized]
        private PixelFormatEnum m_format;
        //[NonSerialized, SecurityCritical]
        private uint m_bitsPerPixel;
    //    //[NonSerialized]
    //    private SecurityCriticalDataForSet<Guid> m_guidFormat;
    //    //[NonSerialized]
    //    private static readonly Guid WICPixelFormatPhotonFirst;
    //    //[NonSerialized]
    //    private static readonly Guid WICPixelFormatPhotonLast;
    //    //[SecurityCritical]

        //[SecurityTreatAsSafe, SecurityCritical]
        internal PixelFormat(PixelFormatEnum format)
        {
            this.m_format = format;
            this.m_flags = GetPixelFormatFlagsFromEnum(format);
            this.m_bitsPerPixel = GetBitsPerPixelFromEnum(format);
            //this.m_guidFormat = new SecurityCriticalDataForSet<Guid>(GetGuidFromFormat(format));
        }

        private static PixelFormatFlags GetPixelFormatFlagsFromEnum(PixelFormatEnum pixelFormatEnum)
        {
            switch (pixelFormatEnum)
            {
                case PixelFormatEnum.Default:
                    return PixelFormatFlags.BitsPerPixelUndefined;

                case PixelFormatEnum.Indexed1:
                    return (PixelFormatFlags.Palettized | PixelFormatFlags.BitsPerPixel1);

                case PixelFormatEnum.Indexed2:
                    return (PixelFormatFlags.Palettized | PixelFormatFlags.BitsPerPixel2);

                case PixelFormatEnum.Indexed4:
                    return (PixelFormatFlags.Palettized | PixelFormatFlags.BitsPerPixel4);

                case PixelFormatEnum.Indexed8:
                    return (PixelFormatFlags.Palettized | PixelFormatFlags.BitsPerPixel8);

                case PixelFormatEnum.BlackWhite:
                    return (PixelFormatFlags.IsGray | PixelFormatFlags.BitsPerPixel1);

                case PixelFormatEnum.Gray2:
                    return (PixelFormatFlags.IsGray | PixelFormatFlags.BitsPerPixel2);

                case PixelFormatEnum.Gray4:
                    return (PixelFormatFlags.IsGray | PixelFormatFlags.BitsPerPixel4);

                case PixelFormatEnum.Gray8:
                    return (PixelFormatFlags.IsGray | PixelFormatFlags.BitsPerPixel8);

                case PixelFormatEnum.Bgr555:
                    return (PixelFormatFlags.ChannelOrderBGR | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel16);

                case PixelFormatEnum.Bgr565:
                    return (PixelFormatFlags.ChannelOrderBGR | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel16);

                case PixelFormatEnum.Gray16:
                    return (PixelFormatFlags.IsSRGB | PixelFormatFlags.IsGray | PixelFormatFlags.BitsPerPixel16);

                case PixelFormatEnum.Bgr24:
                    return (PixelFormatFlags.ChannelOrderBGR | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel24);

                case PixelFormatEnum.Rgb24:
                    return (PixelFormatFlags.ChannelOrderRGB | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel24);

                case PixelFormatEnum.Bgr32:
                    return (PixelFormatFlags.ChannelOrderBGR | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel32);

                case PixelFormatEnum.Bgra32:
                    return (PixelFormatFlags.ChannelOrderABGR | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel32);

                case PixelFormatEnum.Pbgra32:
                    return (PixelFormatFlags.ChannelOrderABGR | PixelFormatFlags.Premultiplied | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel32);

                case PixelFormatEnum.Gray32Float:
                    return (PixelFormatFlags.IsScRGB | PixelFormatFlags.IsGray | PixelFormatFlags.BitsPerPixel32);

                case PixelFormatEnum.Bgr101010:
                    return (PixelFormatFlags.ChannelOrderBGR | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel32);

                case PixelFormatEnum.Rgb48:
                    return (PixelFormatFlags.ChannelOrderRGB | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel48);

                case PixelFormatEnum.Rgba64:
                    return (PixelFormatFlags.ChannelOrderARGB | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel64);

                case PixelFormatEnum.Prgba64:
                    return (PixelFormatFlags.ChannelOrderARGB | PixelFormatFlags.Premultiplied | PixelFormatFlags.IsSRGB | PixelFormatFlags.BitsPerPixel64);

                case PixelFormatEnum.Rgba128Float:
                    return (PixelFormatFlags.ChannelOrderARGB | PixelFormatFlags.IsScRGB | PixelFormatFlags.BitsPerPixel128);

                case PixelFormatEnum.Prgba128Float:
                    return (PixelFormatFlags.ChannelOrderARGB | PixelFormatFlags.Premultiplied | PixelFormatFlags.IsScRGB | PixelFormatFlags.BitsPerPixel128);

                case PixelFormatEnum.Rgb128Float:
                    return (PixelFormatFlags.ChannelOrderRGB | PixelFormatFlags.IsScRGB | PixelFormatFlags.BitsPerPixel128);

                case PixelFormatEnum.Cmyk32:
                    return (PixelFormatFlags.IsCMYK | PixelFormatFlags.BitsPerPixel32);
            }
            return PixelFormatFlags.BitsPerPixelUndefined;
        }

        private static uint GetBitsPerPixelFromEnum(PixelFormatEnum pixelFormatEnum)
        {
            switch (pixelFormatEnum)
            {
                case PixelFormatEnum.Default:
                    return 0;

                case PixelFormatEnum.Indexed1:
                    return 1;

                case PixelFormatEnum.Indexed2:
                    return 2;

                case PixelFormatEnum.Indexed4:
                    return 4;

                case PixelFormatEnum.Indexed8:
                    return 8;

                case PixelFormatEnum.BlackWhite:
                    return 1;

                case PixelFormatEnum.Gray2:
                    return 2;

                case PixelFormatEnum.Gray4:
                    return 4;

                case PixelFormatEnum.Gray8:
                    return 8;

                case PixelFormatEnum.Bgr555:
                case PixelFormatEnum.Bgr565:
                    return 0x10;

                case PixelFormatEnum.Gray16:
                    return 0x10;

                case PixelFormatEnum.Bgr24:
                case PixelFormatEnum.Rgb24:
                    return 0x18;

                case PixelFormatEnum.Bgr32:
                case PixelFormatEnum.Bgra32:
                case PixelFormatEnum.Pbgra32:
                    return 0x20;

                case PixelFormatEnum.Gray32Float:
                    return 0x20;

                case PixelFormatEnum.Bgr101010:
                    return 0x20;

                case PixelFormatEnum.Rgb48:
                    return 0x30;

                case PixelFormatEnum.Rgba64:
                case PixelFormatEnum.Prgba64:
                    return 0x40;

                case PixelFormatEnum.Rgba128Float:
                case PixelFormatEnum.Prgba128Float:
                case PixelFormatEnum.Rgb128Float:
                    return 0x80;

                case PixelFormatEnum.Cmyk32:
                    return 0x20;
            }
            return 0;
        }

        public override string ToString()
        {
            return this.m_format.ToString();
        }

        #region IEquatable<PixelFormat> Members

        public bool Equals(PixelFormat other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
