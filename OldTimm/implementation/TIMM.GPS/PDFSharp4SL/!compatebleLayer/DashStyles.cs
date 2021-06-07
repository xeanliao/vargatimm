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

namespace System.Windows.Media
{
    public static class DashStyles
    {
        private static DashStyle _dash;
        private static DashStyle _dashDot;
        private static DashStyle _dashDotDot;
        private static DashStyle _dot;
        private static DashStyle _solid;

        public static DashStyle Dash
        {
            get
            {
                if (_dash == null)
                {
                    DashStyle style = new DashStyle(new double[] { 2.0, 2.0 }, 1.0);
                    //style.Freeze();
                    _dash = style;
                }
                return _dash;
            }
        }

        public static DashStyle DashDot
        {
            get
            {
                if (_dashDot == null)
                {
                    DashStyle style = new DashStyle(new double[] { 2.0, 2.0, 0.0, 2.0 }, 1.0);
                    //style.Freeze();
                    _dashDot = style;
                }
                return _dashDot;
            }
        }

        public static DashStyle DashDotDot
        {
            get
            {
                if (_dashDotDot == null)
                {
                    DashStyle style = new DashStyle(new double[] { 2.0, 2.0, 0.0, 2.0, 0.0, 2.0 }, 1.0);
                    //style.Freeze();
                    _dashDotDot = style;
                }
                return _dashDotDot;
            }
        }

        public static DashStyle Dot
        {
            get
            {
                if (_dot == null)
                {
                    double[] dashes = new double[2];
                    dashes[1] = 2.0;
                    DashStyle style = new DashStyle(dashes, 0.0);
                    //style.Freeze();
                    _dot = style;
                }
                return _dot;
            }
        }

        public static DashStyle Solid
        {
            get
            {
                if (_solid == null)
                {
                    DashStyle style = new DashStyle();
                    //style.Freeze();
                    _solid = style;
                }
                return _solid;
            }
        }
    }


}
