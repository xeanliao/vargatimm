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
using System.util;
using System.Collections;
using System.Text;
using System.IO;
using iTextSharp.text.pdf;

namespace iTextSharp.text {

    /**
    * A collection of convenience methods that were present in many different iText
    * classes.
    */

    public class Utilities {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        ////public static ICollection GetKeySet(Properties table) {
        ////    return (table == null) ? new Properties().Keys : table.Keys;
        ////}

        /**
        * Utility method to extend an array.
        * @param original the original array or <CODE>null</CODE>
        * @param item the item to be added to the array
        * @return a new array with the item appended
        */    
        public static Object[][] AddToArray(Object[][] original, Object[] item) {
            if (original == null) {
                original = new Object[1][];
                original[0] = item;
                return original;
            }
            else {
                Object[][] original2 = new Object[original.Length + 1][];
                Array.Copy(original, 0, original2, 0, original.Length);
                original2[original.Length] = item;
                return original2;
            }
        }

	    /**
	    * Checks for a true/false value of a key in a Properties object.
	    * @param attributes
	    * @param key
	    * @return
	    */
        //public static bool CheckTrueOrFalse(Properties attributes, String key) {
        //    return Util.EqualsIgnoreCase("true", attributes[key]);
        //}

        /// <summary>
        /// This method makes a valid URL from a given filename.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="filename">a given filename</param>
        /// <returns>a valid URL</returns>
        public static Uri ToURL(string filename) {
            try {
                return new Uri(filename);
            }
            catch {
                return new Uri("file:///" + filename);
            }
        }
    
        /**
        * Unescapes an URL. All the "%xx" are replaced by the 'xx' hex char value.
        * @param src the url to unescape
        * @return the eunescaped value
        */    
        //public static String UnEscapeURL(String src) {
        //    StringBuilder bf = new StringBuilder();
        //    char[] s = src.ToCharArray();
        //    for (int k = 0; k < s.Length; ++k) {
        //        char c = s[k];
        //        if (c == '%') {
        //            if (k + 2 >= s.Length) {
        //                bf.Append(c);
        //                continue;
        //            }
        //            int a0 = PRTokeniser.GetHex((int)s[k + 1]);
        //            int a1 = PRTokeniser.GetHex((int)s[k + 2]);
        //            if (a0 < 0 || a1 < 0) {
        //                bf.Append(c);
        //                continue;
        //            }
        //            bf.Append((char)(a0 * 16 + a1));
        //            k += 2;
        //        }
        //        else
        //            bf.Append(c);
        //    }
        //    return bf.ToString();
        //}
        
        private static byte[] skipBuffer = new byte[4096];

        /// <summary>
        /// This method is an alternative for the Stream.Skip()-method
        /// that doesn't seem to work properly for big values of size.
        /// </summary>
        /// <param name="istr">the stream</param>
        /// <param name="size">the number of bytes to skip</param>
        public static void Skip(Stream istr, int size) {
            while (size > 0) {
                int r = istr.Read(skipBuffer, 0, Math.Min(skipBuffer.Length, size));
                if (r <= 0)
                    return;
                size -= r;
            }
        }

        /**
        * Measurement conversion from millimeters to points.
        * @param    value   a value in millimeters
        * @return   a value in points
        * @since    2.1.2
        */
        public static float MillimetersToPoints(float value) {
            return InchesToPoints(MillimetersToInches(value));
        }

        /**
        * Measurement conversion from millimeters to inches.
        * @param    value   a value in millimeters
        * @return   a value in inches
        * @since    2.1.2
        */
        public static float MillimetersToInches(float value) {
            return value / 25.4f;
        }

        /**
        * Measurement conversion from points to millimeters.
        * @param    value   a value in points
        * @return   a value in millimeters
        * @since    2.1.2
        */
        public static float PointsToMillimeters(float value) {
            return InchesToMillimeters(PointsToInches(value));
        }

        /**
        * Measurement conversion from points to inches.
        * @param    value   a value in points
        * @return   a value in inches
        * @since    2.1.2
        */
        public static float PointsToInches(float value) {
            return value / 72f;
        }

        /**
        * Measurement conversion from inches to millimeters.
        * @param    value   a value in inches
        * @return   a value in millimeters
        * @since    2.1.2
        */
        public static float InchesToMillimeters(float value) {
            return value * 25.4f;
        }

        /**
        * Measurement conversion from inches to points.
        * @param    value   a value in inches
        * @return   a value in points
        * @since    2.1.2
        */
        public static float InchesToPoints(float value) {
            return value * 72f;
        }

        public static bool IsSurrogateHigh(char c) {
            return c >= '\ud800' && c <= '\udbff';
        }

        public static bool IsSurrogateLow(char c) {
            return c >= '\udc00' && c <= '\udfff';
        }

        public static bool IsSurrogatePair(string text, int idx) {
            if (idx < 0 || idx > text.Length - 2)
                return false;
            return IsSurrogateHigh(text[idx]) && IsSurrogateLow(text[idx + 1]);
        }

        public static bool IsSurrogatePair(char[] text, int idx) {
            if (idx < 0 || idx > text.Length - 2)
                return false;
            return IsSurrogateHigh(text[idx]) && IsSurrogateLow(text[idx + 1]);
        }

        public static int ConvertToUtf32(char highSurrogate, char lowSurrogate) {
             return (((highSurrogate - 0xd800) * 0x400) + (lowSurrogate - 0xdc00)) + 0x10000;
        }

        public static int ConvertToUtf32(char[] text, int idx) {
             return (((text[idx] - 0xd800) * 0x400) + (text[idx + 1] - 0xdc00)) + 0x10000;
        }

        public static int ConvertToUtf32(string text, int idx) {
             return (((text[idx] - 0xd800) * 0x400) + (text[idx + 1] - 0xdc00)) + 0x10000;
        }

        public static string ConvertFromUtf32(int codePoint) {
            if (codePoint < 0x10000)
                return Char.ToString((char)codePoint);
            codePoint -= 0x10000;
            return new string(new char[]{(char)((codePoint / 0x400) + 0xd800), (char)((codePoint % 0x400) + 0xdc00)});
        }
    }
}