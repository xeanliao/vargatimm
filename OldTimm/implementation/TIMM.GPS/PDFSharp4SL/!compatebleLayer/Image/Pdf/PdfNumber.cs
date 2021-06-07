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

namespace iTextSharp.text.pdf {

    /**
     * <CODE>PdfNumber</CODE> provides two types of numbers, int and real.
     * <P>
     * ints may be specified by signed or unsigned constants. Reals may only be
     * in decimal format.<BR>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 4.3 (page 37).
     *
     * @see        PdfObject
     * @see        BadPdfFormatException
     */

    public class PdfNumber : PdfObject {
    
        /** actual value of this <CODE>PdfNumber</CODE>, represented as a <CODE>double</CODE> */
        private double value;
    
        // constructors
    
        /**
         * Constructs a <CODE>PdfNumber</CODE>-object.
         *
         * @param        content            value of the new <CODE>PdfNumber</CODE>-object
         */
    
        public PdfNumber(string content) : base(NUMBER) {
            try {
                value = Double.Parse(content.Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                this.Content = content;
            }
            catch (Exception nfe){
                throw new Exception(content + " is not a valid number - " + nfe.ToString());
            }
        }
    
        /**
         * Constructs a new int <CODE>PdfNumber</CODE>-object.
         *
         * @param        value                value of the new <CODE>PdfNumber</CODE>-object
         */
    
        public PdfNumber(int value) : base(NUMBER) {
            this.value = value;
            this.Content = value.ToString();
        }
    
        /**
         * Constructs a new REAL <CODE>PdfNumber</CODE>-object.
         *
         * @param        value                value of the new <CODE>PdfNumber</CODE>-object
         */
    
        public PdfNumber(double value) : base(NUMBER) {
            this.value = value;
            Content = ByteBuffer.FormatDouble(value);
        }
    
        /**
         * Constructs a new REAL <CODE>PdfNumber</CODE>-object.
         *
         * @param        value                value of the new <CODE>PdfNumber</CODE>-object
         */
    
        public PdfNumber(float value) : this((double)value) {}
    
        // methods returning the value of this object
    
        /**
         * Returns the primitive <CODE>int</CODE> value of this object.
         *
         * @return        a value
         */
    
        public int IntValue {
            get {
                return (int) value;
            }
        }
    
        /**
         * Returns the primitive <CODE>double</CODE> value of this object.
         *
         * @return        a value
         */
    
        public double DoubleValue {
            get {
                return value;
            }
        }
    
        public float FloatValue {
            get {
                return (float)value;
            }
        }
    
        // other methods
    
        /**
         * Increments the value of the <CODE>PdfNumber</CODE>-object with 1.
         */
    
        public void Increment() {
            value += 1.0;
            Content = ByteBuffer.FormatDouble(value);
        }
    }
}