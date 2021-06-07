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
/*
 * $Id: Adler32.cs,v 1.1 2006/06/16 10:56:20 psoares33 Exp $
 *
Copyright (c) 2000,2001,2002,2003 ymnk, JCraft,Inc. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

  1. Redistributions of source code must retain the above copyright notice,
     this list of conditions and the following disclaimer.

  2. Redistributions in binary form must reproduce the above copyright 
     notice, this list of conditions and the following disclaimer in 
     the documentation and/or other materials provided with the distribution.

  3. The names of the authors may not be used to endorse or promote products
     derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT,
INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
/*
 * This program is based on zlib-1.1.3, so all credit should go authors
 * Jean-loup Gailly(jloup@gzip.org) and Mark Adler(madler@alumni.caltech.edu)
 * and contributors of zlib.
 */

namespace System.util.zlib {

    internal sealed class Adler32{

        // largest prime smaller than 65536
        private const int BASE=65521; 
        // NMAX is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1
        private const int NMAX=5552;

        internal long adler32(long adler, byte[] buf, int index, int len){
            if(buf == null){ return 1L; }

            long s1=adler&0xffff;
            long s2=(adler>>16)&0xffff;
            int k;

            while(len > 0) {
                k=len<NMAX?len:NMAX;
                len-=k;
                while(k>=16){
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    s1+=buf[index++]&0xff; s2+=s1;
                    k-=16;
                }
                if(k!=0){
                    do{
                        s1+=buf[index++]&0xff; s2+=s1;
                    }
                    while(--k!=0);
                }
                s1%=BASE;
                s2%=BASE;
            }
            return (s2<<16)|s1;
        }

    }
}