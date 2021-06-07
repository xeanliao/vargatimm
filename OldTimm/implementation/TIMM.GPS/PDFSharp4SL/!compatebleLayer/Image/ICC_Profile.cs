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
using System.Text;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Summary description for ICC_Profile.
    /// </summary>
    public class ICC_Profile
    {
        protected byte[] data;
        protected int numComponents;
        private static Dictionary<object, object> cstags = new Dictionary<object, object>();
        
        protected ICC_Profile() {
        }
        
        public static ICC_Profile GetInstance(byte[] data) {
            if (data.Length < 128 | data[36] != 0x61 || data[37] != 0x63 
                || data[38] != 0x73 || data[39] != 0x70)
                throw new ArgumentException("Invalid ICC profile");
            ICC_Profile icc = new ICC_Profile();
            icc.data = data;
            object cs = cstags[new ASCIIEncoding().GetString(data, 16, 4)];
            icc.numComponents = (cs == null ? 0 : (int)cs);
            return icc;
        }
        
        public static ICC_Profile GetInstance(Stream file) {
            byte[] head = new byte[128];
            int remain = head.Length;
            int ptr = 0;
            while (remain > 0) {
                int n = file.Read(head, ptr, remain);
                if (n <= 0)
                    throw new ArgumentException("Invalid ICC profile");
                remain -= n;
                ptr += n;
            }
            if (head[36] != 0x61 || head[37] != 0x63 
                || head[38] != 0x73 || head[39] != 0x70)
                throw new ArgumentException("Invalid ICC profile");
            remain = ((head[0] & 0xff) << 24) | ((head[1] & 0xff) << 16)
                      | ((head[2] & 0xff) <<  8) | (head[3] & 0xff);
            byte[] icc = new byte[remain];
            System.Array.Copy(head, 0, icc, 0, head.Length);
            remain -= head.Length;
            ptr = head.Length;
            while (remain > 0) {
                int n = file.Read(icc, ptr, remain);
                if (n <= 0)
                    throw new ArgumentException("Invalid ICC profile");
                remain -= n;
                ptr += n;
            }
            return GetInstance(icc);
        }

        public static ICC_Profile GetInstance(String fname) {
            FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read);
            ICC_Profile icc = GetInstance(fs);
            fs.Close();
            return icc;
        }

        public byte[] Data {
            get {
                return data;
            }
        }
        
        public int NumComponents {
            get {
                return numComponents;
            }
        }

        static ICC_Profile() {
            cstags["XYZ "] = 3;
            cstags["Lab "] = 3;
            cstags["Luv "] = 3;
            cstags["YCbr"] = 3;
            cstags["Yxy "] = 3;
            cstags["RGB "] = 3;
            cstags["GRAY"] = 1;
            cstags["HSV "] = 3;
            cstags["HLS "] = 3;
            cstags["CMYK"] = 4;
            cstags["CMY "] = 3;
            cstags["2CLR"] = 2;
            cstags["3CLR"] = 3;
            cstags["4CLR"] = 4;
            cstags["5CLR"] = 5;
            cstags["6CLR"] = 6;
            cstags["7CLR"] = 7;
            cstags["8CLR"] = 8;
            cstags["9CLR"] = 9;
            cstags["ACLR"] = 10;
            cstags["BCLR"] = 11;
            cstags["CCLR"] = 12;
            cstags["DCLR"] = 13;
            cstags["ECLR"] = 14;
            cstags["FCLR"] = 15;
        }
    }
}
