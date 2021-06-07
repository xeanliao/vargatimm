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
using System.Runtime.InteropServices;
using System.Collections;

namespace System
{
    [ComVisible(true)]
    public sealed class SerializationInfoEnumerator : IEnumerator
    {
        private bool m_current;
        private int m_currItem;
        private object[] m_data;
        private string[] m_members;
        private int m_numItems;
        private Type[] m_types;

        internal SerializationInfoEnumerator(string[] members, object[] info, Type[] types, int numItems)
        {
            this.m_members = members;
            this.m_data = info;
            this.m_types = types;
            this.m_numItems = numItems - 1;
            this.m_currItem = -1;
            this.m_current = false;
        }

        public bool MoveNext()
        {
            if (this.m_currItem < this.m_numItems)
            {
                this.m_currItem++;
                this.m_current = true;
            }
            else
            {
                this.m_current = false;
            }
            return this.m_current;
        }

        public void Reset()
        {
            this.m_currItem = -1;
            this.m_current = false;
        }

        public SerializationEntry Current
        {
            get
            {
                if (!this.m_current)
                {
                    throw new InvalidOperationException(Env.GetResourceString("InvalidOperation_EnumOpCantHappen"));
                }
                return new SerializationEntry(this.m_members[this.m_currItem], this.m_data[this.m_currItem], this.m_types[this.m_currItem]);
            }
        }

        public string Name
        {
            get
            {
                if (!this.m_current)
                {
                    throw new InvalidOperationException(Env.GetResourceString("InvalidOperation_EnumOpCantHappen"));
                }
                return this.m_members[this.m_currItem];
            }
        }

        public Type ObjectType
        {
            get
            {
                if (!this.m_current)
                {
                    throw new InvalidOperationException(Env.GetResourceString("InvalidOperation_EnumOpCantHappen"));
                }
                return this.m_types[this.m_currItem];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (!this.m_current)
                {
                    throw new InvalidOperationException(Env.GetResourceString("InvalidOperation_EnumOpCantHappen"));
                }
                return new SerializationEntry(this.m_members[this.m_currItem], this.m_data[this.m_currItem], this.m_types[this.m_currItem]);
            }
        }

        public object Value
        {
            get
            {
                if (!this.m_current)
                {
                    throw new InvalidOperationException(Env.GetResourceString("InvalidOperation_EnumOpCantHappen"));
                }
                return this.m_data[this.m_currItem];
            }
        }
    }

 

}
