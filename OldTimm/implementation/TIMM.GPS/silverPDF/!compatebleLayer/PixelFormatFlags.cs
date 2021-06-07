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

namespace silverPDF._compatebleLayer
{
    [Flags]
    internal enum PixelFormatFlags
    {
        BitsPerPixel1 = 1,
        BitsPerPixel128 = 0x80,
        BitsPerPixel16 = 0x10,
        BitsPerPixel2 = 2,
        BitsPerPixel24 = 0x18,
        BitsPerPixel32 = 0x20,
        BitsPerPixel4 = 4,
        BitsPerPixel48 = 0x30,
        BitsPerPixel64 = 0x40,
        BitsPerPixel8 = 8,
        BitsPerPixel96 = 0x60,
        BitsPerPixelMask = 0xff,
        BitsPerPixelUndefined = 0,
        ChannelOrderABGR = 0x10000,
        ChannelOrderARGB = 0x8000,
        ChannelOrderBGR = 0x4000,
        ChannelOrderMask = 0x1e000,
        ChannelOrderRGB = 0x2000,
        IsCMYK = 0x200,
        IsGray = 0x100,
        IsNChannel = 0x80000,
        IsScRGB = 0x800,
        IsSRGB = 0x400,
        NChannelAlpha = 0x40000,
        Palettized = 0x20000,
        Premultiplied = 0x1000
    }
}
