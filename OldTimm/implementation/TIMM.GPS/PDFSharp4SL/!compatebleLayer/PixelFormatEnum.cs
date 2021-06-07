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
    internal enum PixelFormatEnum
    {
        Bgr101010 = 20,
        Bgr24 = 12,
        Bgr32 = 14,
        Bgr555 = 9,
        Bgr565 = 10,
        Bgra32 = 15,
        BlackWhite = 5,
        Cmyk32 = 0x1c,
        Default = 0,
        Extended = 0,
        Gray16 = 11,
        Gray2 = 6,
        Gray32Float = 0x11,
        Gray4 = 7,
        Gray8 = 8,
        Indexed1 = 1,
        Indexed2 = 2,
        Indexed4 = 3,
        Indexed8 = 4,
        Pbgra32 = 0x10,
        Prgba128Float = 0x1a,
        Prgba64 = 0x17,
        Rgb128Float = 0x1b,
        Rgb24 = 13,
        Rgb48 = 0x15,
        Rgba128Float = 0x19,
        Rgba64 = 0x16
    }

}
