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
using System.Globalization;
using System.ComponentModel;
using MS.Internal.FontCache;

namespace System.Windows.Media
{
    public enum NumberCultureSource
    {
        Text,
        User,
        Override
    }

    public enum NumberSubstitutionMethod
    {
        AsCulture,
        Context,
        European,
        NativeNational,
        Traditional
    }

    public class NumberSubstitution
    {
        private CultureInfo _cultureOverride;
        private NumberCultureSource _source;
        private NumberSubstitutionMethod _substitution;
        public static readonly DependencyProperty CultureOverrideProperty;// = DependencyProperty.RegisterAttached("CultureOverride", typeof(CultureInfo), typeof(NumberSubstitution), null, new ValidateValueCallback(NumberSubstitution.IsValidCultureOverrideValue));
        public static readonly DependencyProperty CultureSourceProperty;// = DependencyProperty.RegisterAttached("CultureSource", typeof(NumberCultureSource), typeof(NumberSubstitution));
        public static readonly DependencyProperty SubstitutionProperty;// = DependencyProperty.RegisterAttached("Substitution", typeof(NumberSubstitutionMethod), typeof(NumberSubstitution));

        public NumberSubstitution()
        {
            this._source = NumberCultureSource.Text;
            this._cultureOverride = null;
            this._substitution = NumberSubstitutionMethod.AsCulture;
        }

        public NumberSubstitution(NumberCultureSource source, CultureInfo cultureOverride, NumberSubstitutionMethod substitution)
        {
            this._source = source;
            this._cultureOverride = ThrowIfInvalidCultureOverride(cultureOverride);
            this._substitution = substitution;
        }

        public override bool Equals(object obj)
        {
            NumberSubstitution substitution = obj as NumberSubstitution;
            if (((substitution == null) || (this._source != substitution._source)) || (this._substitution != substitution._substitution))
            {
                return false;
            }
            if (this._cultureOverride != null)
            {
                return this._cultureOverride.Equals(substitution._cultureOverride);
            }
            return (substitution._cultureOverride == null);
        }

        //[AttachedPropertyBrowsableForType(typeof(DependencyObject)), TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public static CultureInfo GetCultureOverride(DependencyObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            return (CultureInfo)target.GetValue(CultureOverrideProperty);
        }

        //[AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static NumberCultureSource GetCultureSource(DependencyObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            return (NumberCultureSource)target.GetValue(CultureSourceProperty);
        }

        public override int GetHashCode()
        {
            int hash = HashFn.HashMultiply((int)this._source) + (int)this._substitution;
            if (this._cultureOverride != null)
            {
                hash = HashFn.HashMultiply(hash) + this._cultureOverride.GetHashCode();
            }
            return HashFn.HashScramble(hash);
        }

        //[AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static NumberSubstitutionMethod GetSubstitution(DependencyObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            return (NumberSubstitutionMethod)target.GetValue(SubstitutionProperty);
        }

        private static bool IsValidCultureOverride(CultureInfo culture)
        {
            return ((culture == null) || (!culture.IsNeutralCulture && !culture.Equals(CultureInfo.InvariantCulture)));
        }

        private static bool IsValidCultureOverrideValue(object value)
        {
            return IsValidCultureOverride((CultureInfo)value);
        }

        public static void SetCultureOverride(DependencyObject target, CultureInfo value)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            target.SetValue(CultureOverrideProperty, value);
        }

        public static void SetCultureSource(DependencyObject target, NumberCultureSource value)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            target.SetValue(CultureSourceProperty, value);
        }

        public static void SetSubstitution(DependencyObject target, NumberSubstitutionMethod value)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            target.SetValue(SubstitutionProperty, value);
        }

        private static CultureInfo ThrowIfInvalidCultureOverride(CultureInfo culture)
        {
            if (!IsValidCultureOverride(culture))
            {
                throw new ArgumentException(SR.GetString("SpecificNumberCultureRequired"));
            }
            return culture;
        }

        //[TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo CultureOverride
        {
            get
            {
                return this._cultureOverride;
            }
            set
            {
                this._cultureOverride = ThrowIfInvalidCultureOverride(value);
            }
        }

        public NumberCultureSource CultureSource
        {
            get
            {
                return this._source;
            }
            set
            {
                if (value > NumberCultureSource.Override)
                {
                    throw new InvalidEnumArgumentException("CultureSource", (int)value, typeof(NumberCultureSource));
                }
                this._source = value;
            }
        }

        public NumberSubstitutionMethod Substitution
        {
            get
            {
                return this._substitution;
            }
            set
            {
                if (value > NumberSubstitutionMethod.Traditional)
                {
                    throw new InvalidEnumArgumentException("Substitution", (int)value, typeof(NumberSubstitutionMethod));
                }
                this._substitution = value;
            }
        }
    }

 

}
