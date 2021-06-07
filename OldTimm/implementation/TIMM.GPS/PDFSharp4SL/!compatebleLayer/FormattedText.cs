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
using System.Globalization;
using System.IO;
using System.Windows.Controls.Primitives;

namespace System.Windows.Media
{
    public class FormattedText
    {
        public FormattedText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground)
            : this(textToFormat, culture, flowDirection, typeface, emSize, foreground, null)
        {
        }

        public FormattedText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, NumberSubstitution numberSubstitution)
        {
            Text = textToFormat;
            CultureInfo = culture;
            FlowDirection = flowDirection;
            Typeface = typeface;
            EmSize = emSize;
        }

        public void SetTextDecorations(TextDecorationCollection textDecorationCollection)
        {
            throw new NotImplementedException();
        }

        private double EmSize;
        private Typeface Typeface;
        private FlowDirection FlowDirection;
        private CultureInfo CultureInfo;

        public string Text;
        public double Baseline;
        public double Extent;
        /// <summary>
        /// !!!!!!!!!!!!!!!!!!
        /// </summary>
        public double Height
        {
            get
            {
                return CalculeteSize().Height;
            }
        }
        public double OverhangAfter;
        public double OverhangLeading;
        public double OverhangTrailing;
        public double Width;
        /// <summary>
        /// !!!!!!!!!!!!!!!!!!
        /// </summary>
        public double WidthIncludingTrailingWhitespace
        {
            get
            {
                return CalculeteSize().Width;
            }
        }

        private Size CalculeteSize()
        {
            var g = new Grid();
            var s = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var fs = FontPool.GetFontStream(Typeface.FontFamily.Source);
            s.Children.Add(new TextBlock
                {
                    Text = Text,
                    FontSource = new FontSource(fs),
                    FontSize = EmSize,
                    FontFamily = Typeface.FontFamily,
                    FontStretch = Typeface.FontStretch,
                    FontStyle = Typeface.FontStyle,
                    FontWeight = Typeface.FontWeight,
                });
            g.Children.Add(s);

            g.Arrange(new Rect(0, 0, double.MaxValue, double.MaxValue));

            var aw = s.ActualWidth;
            var ah = s.ActualHeight;
            var size = new Size(aw, ah);

            //fs.Close();
            return size;
        }
        public TextAlignment TextAlignment;

        /// <summary>
        /// Возвращает объект Geometry, представляющий форматированный текст, включая все глифы и оформление текста.
        /// </summary>
        /// <param name="point">Начало координат в левом верхнем углу результирующей геометрии.</param>
        internal Geometry BuildGeometry(Point point)
        {
            return null;
            //throw new NotImplementedException();
        }
    }
}
