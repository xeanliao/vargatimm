// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PietschSoft.VE.Converters
{
    public class GenericConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            //return base.ConvertTo(context, culture, value, destinationType);

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            jss.MaxJsonLength = int.MaxValue;
            string s = jss.Serialize(value);
            return s;
        }
    }
}
