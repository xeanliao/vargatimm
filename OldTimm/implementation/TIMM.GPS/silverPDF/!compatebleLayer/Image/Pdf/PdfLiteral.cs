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
using System.IO;

namespace iTextSharp.text.pdf 
{

    /**
     * a Literal
     */

    public class PdfLiteral : PdfObject {

        private int position;

        public PdfLiteral(string text) : base(0, text) {}
    
        public PdfLiteral(byte[] b) : base(0, b) {}

        public PdfLiteral(int type, string text) : base(type, text) {}
    
        public PdfLiteral(int type, byte[] b) : base(type, b) {}

        public PdfLiteral(int size) : base(0, (byte[])null) {
            bytes = new byte[size];
            for (int k = 0; k < size; ++k) {
               bytes[k] = 32;
            }
        }

        //public override void ToPdf(PdfWriter writer, Stream os) {
        //    if (os is OutputStreamCounter)
        //        position = ((OutputStreamCounter)os).Counter;
        //    base.ToPdf(writer, os);
        //}

        public int Position {
            get {
                return position;
            }
        }

        public int PosLength {
            get {
                if (bytes != null)
                    return bytes.Length;
                else
                    return 0;
            }
        }
    }
}