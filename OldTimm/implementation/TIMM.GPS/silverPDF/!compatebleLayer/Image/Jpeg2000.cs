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
using System.Net;
using iTextSharp.text.pdf;

namespace iTextSharp.text {

    /**
    * An <CODE>Jpeg2000</CODE> is the representation of a graphic element (JPEG)
    * that has to be inserted into the document
    *
    * @see		Element
    * @see		Image
    */

    public class Jpeg2000 : Image {
        
        // public static final membervariables
        
        public const int JP2_JP = 0x6a502020;
        public const int JP2_IHDR = 0x69686472;
        public const int JPIP_JPIP = 0x6a706970;

        public const int JP2_FTYP = 0x66747970;
        public const int JP2_JP2H = 0x6a703268;
        public const int JP2_COLR = 0x636f6c72;
        public const int JP2_JP2C = 0x6a703263;
        public const int JP2_URL = 0x75726c20;
        public const int JP2_DBTL = 0x6474626c;
        public const int JP2_BPCC = 0x62706363;
        public const int JP2_JP2 = 0x6a703220;

        private Stream inp;
        private int boxLength;
        private int boxType;
        
        // Constructors
        
        public Jpeg2000(Image image) : base(image) { }

        public Jpeg2000(Stream stream) : base(Image.GetInstance(stream)) { }

        /**
        * Constructs a <CODE>Jpeg2000</CODE>-object, using an <VAR>url</VAR>.
        *
        * @param		url			the <CODE>URL</CODE> where the image can be found
        * @throws BadElementException
        * @throws IOException
        */
        [Obsolete("",true)]
        public Jpeg2000(Uri url) : base(url) {
            ProcessParameters();
        }
        
        /**
        * Constructs a <CODE>Jpeg2000</CODE>-object from memory.
        *
        * @param		img		the memory image
        * @throws BadElementException
        * @throws IOException
        */
        
        public Jpeg2000(byte[] img) : base((Uri)null) {
            rawData = img;
            originalData = img;
            ProcessParameters();
        }
        
        /**
        * Constructs a <CODE>Jpeg2000</CODE>-object from memory.
        *
        * @param		img			the memory image.
        * @param		width		the width you want the image to have
        * @param		height		the height you want the image to have
        * @throws BadElementException
        * @throws IOException
        */
        
        public Jpeg2000(byte[] img, float width, float height) : this(img) {
            scaledWidth = width;
            scaledHeight = height;
        }
        
        private int Cio_read(int n) {
            int v = 0;
            for (int i = n - 1; i >= 0; i--) {
                v += inp.ReadByte() << (i << 3);
            }
            return v;
        }
        
        public void Jp2_read_boxhdr() {
            boxLength = Cio_read(4);
            boxType = Cio_read(4);
            if (boxLength == 1) {
                if (Cio_read(4) != 0) {
                    throw new IOException("Cannot handle box sizes higher than 2^32");
                }
                boxLength = Cio_read(4);
                if (boxLength == 0) 
                    throw new IOException("Unsupported box size == 0");
            }
            else if (boxLength == 0) {
                throw new IOException("Unsupported box size == 0");
            }
        }
        
        /**
        * This method checks if the image is a valid JPEG and processes some parameters.
        * @throws BadElementException
        * @throws IOException
        */
        private void ProcessParameters() {
            type = JPEG2000;
            originalType = ORIGINAL_JPEG2000;
            inp = null;
            try {
                string errorID;
                if (rawData == null){
                    throw new Exception("!!!");
                    //WebRequest w = WebRequest.Create(url);
                    //inp = w.GetResponse().GetResponseStream();
                    errorID = url.ToString();
                }
                else{
                    inp = new MemoryStream(rawData);
                    errorID = "Byte array";
                }
                boxLength = Cio_read(4);
                if (boxLength == 0x0000000c) {
                    boxType = Cio_read(4);
                    if (JP2_JP != boxType) {
                        throw new IOException("Expected JP Marker");
                    }
                    if (0x0d0a870a != Cio_read(4)) {
                        throw new IOException("Error with JP Marker");
                    }

                    Jp2_read_boxhdr();
                    if (JP2_FTYP != boxType) {
                        throw new IOException("Expected FTYP Marker");
                    }
                    Utilities.Skip(inp, boxLength - 8);
                    Jp2_read_boxhdr();
                    do {
                        if (JP2_JP2H != boxType) {
                            if (boxType == JP2_JP2C) {
                                throw new IOException("Expected JP2H Marker");
                            }
                            Utilities.Skip(inp, boxLength - 8);
                            Jp2_read_boxhdr();
                        }
                    } while (JP2_JP2H != boxType);
                    Jp2_read_boxhdr();
                    if (JP2_IHDR != boxType) {
                        throw new IOException("Expected IHDR Marker");
                    }
                    scaledHeight = Cio_read(4);
                    Top = scaledHeight;
                    scaledWidth = Cio_read(4);
                    Right = scaledWidth;
                    bpc = -1;
                }
                else if ((uint)boxLength == 0xff4fff51) {
                    Utilities.Skip(inp, 4);
                    int x1 = Cio_read(4);
                    int y1 = Cio_read(4);
                    int x0 = Cio_read(4);
                    int y0 = Cio_read(4);
                    Utilities.Skip(inp, 16);
                    colorspace = Cio_read(2);
                    bpc = 8;
                    scaledHeight = y1 - y0;
                    Top = scaledHeight;
                    scaledWidth = x1 - x0;
                    Right = scaledWidth;
                }
                else {
                    throw new IOException("Not a valid Jpeg2000 file");
                }
            }
            finally {
                if (inp != null) {
                    try{inp.Close();}catch{}
                    inp = null;
                }
            }
            plainWidth = this.Width;
            plainHeight = this.Height;
        }
    }
}