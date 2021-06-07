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
    * Classes implementing this interface can create custom encodings or
    * replace existing ones. It is used in the context of <code>PdfEncoding</code>.
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public interface IExtraEncoding {
        
        /**
        * Converts an Unicode string to a byte array according to some encoding.
        * @param text the Unicode string
        * @param encoding the requested encoding. It's mainly of use if the same class
        * supports more than one encoding.
        * @return the conversion or <CODE>null</CODE> if no conversion is supported
        */    
        byte[] CharToByte(String text, String encoding);
        
        /**
        * Converts an Unicode char to a byte array according to some encoding.
        * @param char1 the Unicode char
        * @param encoding the requested encoding. It's mainly of use if the same class
        * supports more than one encoding.
        * @return the conversion or <CODE>null</CODE> if no conversion is supported
        */    
        byte[] CharToByte(char char1, String encoding);

        /**
        * Converts a byte array to an Unicode string according to some encoding.
        * @param b the input byte array
        * @param encoding the requested encoding. It's mainly of use if the same class
        * supports more than one encoding.
        * @return the conversion or <CODE>null</CODE> if no conversion is supported
        */
        String ByteToChar(byte[] b, String encoding);   
    }
}
