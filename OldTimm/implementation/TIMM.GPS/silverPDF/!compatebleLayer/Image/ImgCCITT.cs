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
using iTextSharp.text.pdf.codec;

namespace iTextSharp.text {
    /**
     * CCITT Image data that has to be inserted into the document
     *
     * @see        Element
     * @see        Image
     *
     * @author  Paulo Soares
     */
    /// <summary>
    /// CCITT Image data that has to be inserted into the document
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Image"/>
    public class ImgCCITT : Image {
        public ImgCCITT(Image image) : base(image) {}

        /// <summary>
        /// Creats an Image in CCITT mode.
        /// </summary>
        /// <param name="width">the exact width of the image</param>
        /// <param name="height">the exact height of the image</param>
        /// <param name="reverseBits">
        /// reverses the bits in data.
        /// Bit 0 is swapped with bit 7 and so on
        /// </param>
        /// <param name="typeCCITT">
        /// the type of compression in data. It can be
        /// CCITTG4, CCITTG31D, CCITTG32D
        /// </param>
        /// <param name="parameters">
        /// parameters associated with this stream. Possible values are
        /// CCITT_BLACKIS1, CCITT_ENCODEDBYTEALIGN, CCITT_ENDOFLINE and CCITT_ENDOFBLOCK or a
        /// combination of them
        /// </param>
        /// <param name="data">the image data</param>
        public ImgCCITT(int width, int height, bool reverseBits, int typeCCITT, int parameters, byte[] data) : base((Uri)null) {
            if (typeCCITT != Element.CCITTG4 && typeCCITT != Element.CCITTG3_1D && typeCCITT != Element.CCITTG3_2D)
                throw new Exception("The CCITT compression type must be CCITTG4, CCITTG3_1D or CCITTG3_2D");
            if (reverseBits)
                TIFFFaxDecoder.ReverseBits(data);
            type = Element.IMGRAW;
            scaledHeight = height;
            this.Top = scaledHeight;
            scaledWidth = width;
            this.Right = scaledWidth;
            colorspace = parameters;
            bpc = typeCCITT;
            rawData = data;
            plainWidth = this.Width;
            plainHeight = this.Height;
        }
    }
}
