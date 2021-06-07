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
using System.util.zlib;

namespace silverPDF._compatebleLayer.Image.Pdf
{
    public class PdfReader
    {
        public static byte[] FlateDecode(byte[] inp)
        {
            byte[] b = FlateDecode(inp, true);
            if (b == null)
                return FlateDecode(inp, false);
            return b;
        }

        /** A helper to FlateDecode.
                * @param in the input data
                * @param strict <CODE>true</CODE> to read a correct stream. <CODE>false</CODE>
                * to try to read a corrupted stream
                * @return the decoded data
                */
        public static byte[] FlateDecode(byte[] inp, bool strict)
        {
            MemoryStream stream = new MemoryStream(inp);
            ZInflaterInputStream zip = new ZInflaterInputStream(stream);
            MemoryStream outp = new MemoryStream();
            byte[] b = new byte[strict ? 4092 : 1];
            try
            {
                int n;
                while ((n = zip.Read(b, 0, b.Length)) > 0)
                {
                    outp.Write(b, 0, n);
                }
                zip.Close();
                outp.Close();
                return outp.ToArray();
            }
            catch
            {
                if (strict)
                    return null;
                return outp.ToArray();
            }
        }
    }
}
