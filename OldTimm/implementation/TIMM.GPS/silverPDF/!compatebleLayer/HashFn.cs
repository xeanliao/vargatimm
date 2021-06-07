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
using System.Security;

namespace MS.Internal.FontCache
{
    internal static class HashFn
    {
        private const int HASH_MULTIPLIER = 0x65;

        //[SecurityCritical]
        //internal static unsafe int HashMemory(void* pv, int numBytes, int hash)
        //{
        //    for (byte* numPtr = (byte*)pv; numBytes-- > 0; numPtr++)
        //    {
        //        hash = HashMultiply(hash) + numPtr[0];
        //    }
        //    return hash;
        //}

        internal static int HashMultiply(int hash)
        {
            return (hash * 0x65);
        }

        internal static int HashScramble(int hash)
        {
            uint num2 = (uint)(0x12b9b0a5 * hash);
            return (int)(num2 % 0x3b9aca07);
        }

        //internal static int HashString(string s, int hash)
        //{
        //    foreach (char ch in s)
        //    {
        //        hash = HashMultiply(hash) + ch;
        //    }
        //    return hash;
        //}
    }

}
