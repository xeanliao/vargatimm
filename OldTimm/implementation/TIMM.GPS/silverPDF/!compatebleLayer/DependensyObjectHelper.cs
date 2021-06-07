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
using System.Reflection;

namespace System.Windows
{
    public static class DependensyObjectHelper
    {
        public static T Clone<T>(this T source) where T : DependencyObject
        {
            Type t = source.GetType();
            T no = (T)Activator.CreateInstance(t);

            Type wt = t;
            while (wt.BaseType != typeof(DependencyObject))
            {
                FieldInfo[] fi = wt.GetFields(BindingFlags.Static | BindingFlags.Public);
                for (int i = 0; i < fi.Length; i++)
                {
                    {
                        DependencyProperty dp = fi[i].GetValue(source) as DependencyProperty;
                        if (dp != null && fi[i].Name != "NameProperty")
                        {
                            DependencyObject obj = source.GetValue(dp) as DependencyObject;
                            if (obj != null)
                            {
                                try
                                {
                                    object o = obj.Clone();
                                    no.SetValue(dp, o);
                                }
                                catch { }
                            }
                            else
                            {
                                if (fi[i].Name != "CountProperty" &&
                                    fi[i].Name != "GeometryTransformProperty" &&
                                    fi[i].Name != "ActualWidthProperty" &&
                                    fi[i].Name != "ActualHeightProperty" &&
                                    fi[i].Name != "MaxWidthProperty" &&
                                    fi[i].Name != "MaxHeightProperty" &&
                                    fi[i].Name != "StyleProperty" &&
                                    fi[i].Name != "IsFocusedProperty" &&
                                    fi[i].Name != "IsMouseOverProperty" &&
                                    fi[i].Name != "IsPressedProperty"
                                    )
                                {
                                    no.SetValue(dp, source.GetValue(dp));
                                }

                            }
                        }
                    }
                }
                wt = wt.BaseType;
            }

            PropertyInfo[] pis = t.GetProperties();
            for (int i = 0; i < pis.Length; i++)
            {

                if (
                    pis[i].Name != "Name" &&
                    pis[i].Name != "Parent" &&
                    pis[i].CanRead && pis[i].CanWrite &&
                    !pis[i].PropertyType.IsArray &&
                    !pis[i].PropertyType.IsSubclassOf(typeof(DependencyObject)) &&
                    pis[i].GetIndexParameters().Length == 0 &&
                    pis[i].GetValue(source, null) != null &&
                    pis[i].GetValue(source, null) == (object)default(int) &&
                    pis[i].GetValue(source, null) == (object)default(double) &&
                    pis[i].GetValue(source, null) == (object)default(float)
                    )
                    pis[i].SetValue(no, pis[i].GetValue(source, null), null);
                else if (pis[i].PropertyType.GetInterface("IList", true) != null)
                {
                    int cnt = (int)pis[i].PropertyType.InvokeMember("get_Count", BindingFlags.InvokeMethod, null, pis[i].GetValue(source, null), null);
                    for (int c = 0; c < cnt; c++)
                    {
                        object val = pis[i].PropertyType.InvokeMember("get_Item", BindingFlags.InvokeMethod, null, pis[i].GetValue(source, null), new object[] { c });

                        object nVal = val;
                        DependencyObject v = val as DependencyObject;
                        if (v != null)
                            nVal = v.Clone();

                        try
                        {
                            pis[i].PropertyType.InvokeMember("Add", BindingFlags.InvokeMethod, null, pis[i].GetValue(no, null), new object[] { nVal });
                        }
                        catch { }
                    }
                }
            }

            return no;
        }
    }
}
