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
using System.Reflection;

namespace System
{
    public static class EnumHelper
    {
        [ComVisible(true)]
        public static object Parse(Type enumType, string value)
        {
            return Enum.Parse(enumType, value, false);
        }

        public static string[] GetNames(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(Env.GetResourceString("Arg_MustBeEnum"), "enumType");
            }
            string[] names = GetHashEntry(enumType).names;
            string[] destinationArray = new string[names.Length];
            Array.Copy(names, destinationArray, names.Length);
            return destinationArray;

        }

        private static HashEntry GetHashEntry(Type enumType)
        {
            ulong[] values = null;
            string[] names = null;

            FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            values = new ulong[fields.Length];
            names = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                names[i] = fields[i].Name;
                values[i] = Convert.ToUInt64(fields[i].GetValue(null));
            }
            for (int j = 1; j < values.Length; j++)
            {
                int index = j;
                string str = names[j];
                ulong num4 = values[j];
                bool flag = false;
                while (values[index - 1] > num4)
                {
                    names[index] = names[index - 1];
                    values[index] = values[index - 1];
                    index--;
                    flag = true;
                    if (index == 0)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    names[index] = str;
                    values[index] = num4;
                }
            }

            var entry = new HashEntry(names, values);

            return entry;
        }

        private class HashEntry
        {
            public string[] names;
            public ulong[] values;

            public HashEntry(string[] names, ulong[] values)
            {
                this.names = names;
                this.values = values;
            }
        }


    }
}
