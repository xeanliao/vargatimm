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
using System.Windows.Resources;
using System.Collections.Generic;

namespace System.Windows.Media
{
    public class GlyphTypeface
    {
        public string CurrentFont { get; set; }

        public Stream GetFontStream()
        {
            return FontPool.GetFontStream(CurrentFont).CloneToMemoryStream();
        }
    }

    public static class FontPool
    {
        static FontPool()
        {
            StreamResourceInfo sri = Application.GetResourceStream(
                new Uri(
                    "silverPDF;component/Fonts/arialbd.ttf",
                    UriKind.RelativeOrAbsolute
                    ));

            FontPool.Register(sri.Stream, "Arial");
        }

        public static Stream GetFontStream(string key)
        {
            Stream stream = null;

            if (FontPool.Fonts.ContainsKey(key))
            {
                stream = FontPool.Fonts[key];
            }
            else
            {
                stream = FontPool.Fonts["Arial"];
            }

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        private static Dictionary<string, Stream> Fonts = new Dictionary<string,Stream>();

        public static void Register(Stream fontStream, params string[] fontFaceNames)
        {
            var s = fontStream.CloneToMemoryStream();
            foreach (var fn in fontFaceNames)
            {
                Fonts.Add(fn, s);
            }
            fontStream.Close();
        }

        [Obsolete("Very unstable! Do not use this", true)]
        public static void RegisterObfuscated(Guid guid, Stream fontStream, params string[] fontFaceNames)
        {
            var s = Deobfuscate(fontStream, guid);

            foreach (var fn in fontFaceNames)
            {
                Fonts.Add(fn, s);
            }
        }

        public static Stream Deobfuscate(Stream fontStream, Guid guid)
        {
            var streamOut = new MemoryStream();

            if ((guid == null) || (fontStream == null))
            {
                throw new ArgumentNullException();
            }
            int count = 0x1000;
            int num2 = 0x10;
            int num3 = 0x20;

            string str2 = guid.ToString("N");

            byte[] buffer = new byte[num2];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Convert.ToByte(str2.Substring(i * 2, 2), 0x10);
            }

            byte[] buffer2 = new byte[num3];
            fontStream.Read(buffer2, 0, num3);
            for (int j = 0; j < num3; j++)
            {
                int index = (buffer.Length - (j % buffer.Length)) - 1;
                buffer2[j] = (byte)(buffer2[j] ^ buffer[index]);
            }

            streamOut.Write(buffer2, 0, num3);
            buffer2 = new byte[count];
            int num9 = 0;
            while ((num9 = fontStream.Read(buffer2, 0, count)) > 0)
            {
                streamOut.Write(buffer2, 0, num9);
            }
            streamOut.Position = 0L;

            return streamOut;
        }



    }
}
