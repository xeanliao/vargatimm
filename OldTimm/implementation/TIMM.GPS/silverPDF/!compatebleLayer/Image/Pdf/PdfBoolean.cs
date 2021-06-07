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
     * <CODE>PdfBoolean</CODE> is the bool object represented by the keywords <VAR>true</VAR> or <VAR>false</VAR>.
     * <P>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 4.2 (page 37).
     *
     * @see        PdfObject
     * @see        BadPdfFormatException
     */

    public class PdfBoolean : PdfObject {
    
        // static membervariables (possible values of a bool object)
        public static readonly PdfBoolean PDFTRUE = new PdfBoolean(true);
        public static readonly PdfBoolean PDFFALSE = new PdfBoolean(false);
        /** A possible value of <CODE>PdfBoolean</CODE> */
        public const string TRUE = "true";
    
        /** A possible value of <CODE>PdfBoolean</CODE> */
        public const string FALSE = "false";
    
        // membervariables
    
        /** the bool value of this object */
        private bool value;
    
        // constructors
    
        /**
         * Constructs a <CODE>PdfBoolean</CODE>-object.
         *
         * @param        value            the value of the new <CODE>PdfObject</CODE>
         */
    
        public PdfBoolean(bool value) : base(BOOLEAN) {
            if (value) {
                this.Content = TRUE;
            }
            else {
                this.Content = FALSE;
            }
            this.value = value;
        }
    
        /**
         * Constructs a <CODE>PdfBoolean</CODE>-object.
         *
         * @param        value            the value of the new <CODE>PdfObject</CODE>, represented as a <CODE>string</CODE>
         *
         * @throws        BadPdfFormatException    thrown if the <VAR>value</VAR> isn't '<CODE>true</CODE>' or '<CODE>false</CODE>'
         */
    
        public PdfBoolean(string value) : base(BOOLEAN, value) {
            if (value.Equals(TRUE)) {
                this.value = true;
            }
            else if (value.Equals(FALSE)) {
                this.value = false;
            }
            else {
                throw new Exception("The value has to be 'true' of 'false', instead of '" + value + "'.");
            }
        }
    
        // methods returning the value of this object
    
        /**
         * Returns the primitive value of the <CODE>PdfBoolean</CODE>-object.
         *
         * @return        the actual value of the object.
         */
    
        public bool BooleanValue {
            get {
                return value;
            }
        }

        public override string ToString() {
            return value ? TRUE : FALSE;
        }
    }
}