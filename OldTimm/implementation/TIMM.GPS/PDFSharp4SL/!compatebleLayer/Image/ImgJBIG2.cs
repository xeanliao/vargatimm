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
using System.Security.Cryptography;

namespace iTextSharp.text {

    /**
    * Support for JBIG2 images.
    * @since 2.1.5
    */
    public class ImgJBIG2 : Image {
        
        /** JBIG2 globals */
        private byte[] global;
        /** A unique hash */
        private byte[] globalHash;
        
        /**
        * Copy contstructor.
        * @param    image another Image
        */
        ImgJBIG2(Image image) : base(image) {
        }

        /**
        * Empty constructor.
        */
        public ImgJBIG2() : base((Image) null) {
        }

        /**
        * Actual constructor for ImgJBIG2 images.
        * @param    width   the width of the image
        * @param    height  the height of the image
        * @param    data    the raw image data
        * @param    globals JBIG2 globals
        */
        public ImgJBIG2(int width, int height, byte[] data, byte[] globals) : base((Uri)null) {
            type = Element.JBIG2;
            originalType = ORIGINAL_JBIG2;
            scaledHeight = height;
            this.Top = scaledHeight;
            scaledWidth = width;
            this.Right = scaledWidth;
            bpc = 1;
            colorspace = 1;
            rawData = data;
            plainWidth = this.Width;
            plainHeight = this.Height;
            if ( globals != null ) {
                this.global = globals;
                try {
                    MD5Managed md5 = new MD5Managed();
                    md5.Initialize();
                    this.globalHash = md5.ComputeHash(this.global);
                } catch {
                    //ignore
                }
            }
        }
        
        /**
        * Getter for the JBIG2 global data.
        * @return   an array of bytes
        */
        public byte[] GlobalBytes {
            get {
                return this.global;
            }
        }
        
        /**
        * Getter for the unique hash.
        * @return   an array of bytes
        */
        public byte[] GlobalHash {
            get {
                return this.globalHash;
            }
        }
    }
}