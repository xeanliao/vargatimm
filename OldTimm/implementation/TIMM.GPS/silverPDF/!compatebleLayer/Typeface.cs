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
using silverPDF._compatebleLayer;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a combination of <see cref="T:System.Windows.Media.FontFamily" />, <see cref="T:System.Windows.FontWeight" />, <see cref="T:System.Windows.FontStyle" />, and <see cref="T:System.Windows.FontStretch" />.
    /// </summary>
    public class Typeface
    {
        public Typeface(FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch)
        {
            FontFamily = fontFamily;
            FontStyle = style;
            FontWeight = weight;
            FontStretch = stretch;
        }
        public Typeface(FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch, FontFamily fallbackFontFamily)
        {
            FontFamily = fontFamily;
            FontStyle = style;
            FontWeight = weight;
            FontStretch = stretch;
        }

        public FontFamily FontFamily { get; set; }
        public FontStyle FontStyle { get; set; }
        public FontWeight FontWeight { get; set; }
        public FontStretch FontStretch { get; set; }
        public bool IsBoldSimulated { get; set; }
        public bool IsObliqueSimulated { get; set; }


        internal bool TryGetGlyphTypeface(out GlyphTypeface glyphTypeface)
        {
            glyphTypeface = new GlyphTypeface();
            glyphTypeface.CurrentFont = FontFamily.Source;

            return true;
        }
    }

    //public class TypefaceOld
    //{
    //    private CachedTypeface _cachedTypeface;
    //    private FontFamily _fallbackFontFamily;
    //    private FontFamily _fontFamily;
    //    private FontStretch _stretch;
    //    private FontStyle _style;
    //    private FontWeight _weight;

    //    public Typeface(string typefaceName)
    //        : this(new FontFamily(typefaceName), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal)
    //    {
    //    }

    //    public Typeface(FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch)
    //        : this(fontFamily, style, weight, stretch, FontFamily.FontFamilyGlobalUI)
    //    {
    //    }

    //    public Typeface(FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch, FontFamily fallbackFontFamily)
    //    {
    //        if (fontFamily == null)
    //        {
    //            throw new ArgumentNullException("fontFamily");
    //        }
    //        this._fontFamily = fontFamily;
    //        this._style = style;
    //        this._weight = weight;
    //        this._stretch = stretch;
    //        this._fallbackFontFamily = fallbackFontFamily;
    //    }

    //    internal bool CheckFastPathNominalGlyphs(CharacterBufferRange charBufferRange, double emSize, double widthMax, bool keepAWord, bool numberSubstitution, CultureInfo cultureInfo, out int stringLengthFit)
    //    {
    //        stringLengthFit = 0;
    //        if (this.CachedTypeface.NullFont)
    //        {
    //            return false;
    //        }
    //        GlyphTypeface typeface = this.TryGetGlyphTypeface();
    //        if (typeface == null)
    //        {
    //            return false;
    //        }
    //        stringLengthFit = 0;
    //        IDictionary<int, ushort> characterToGlyphMap = typeface.CharacterToGlyphMap;
    //        double num5 = 0.0;
    //        int num = 0;
    //        ushort blankGlyphIndex = typeface.BlankGlyphIndex;
    //        ushort num4 = blankGlyphIndex;
    //        ushort num6 = numberSubstitution ? ((ushort)0x101) : ((ushort)1);
    //        ushort flags = 0;
    //        ushort num3 = 0x30;
    //        bool symbol = typeface.Symbol;
    //        if (symbol)
    //        {
    //            num6 = 0;
    //        }
    //        if (keepAWord)
    //        {
    //            do
    //            {
    //                char codepoint = charBufferRange[num++];
    //                flags = Classification.CharAttributeOf(Classification.GetUnicodeClassUTF16(codepoint)).Flags;
    //                num3 = (ushort)(num3 & flags);
    //                characterToGlyphMap.TryGetValue(codepoint, out num4);
    //                num5 += emSize * typeface.GetAdvanceWidth(num4);
    //            }
    //            while ((((num < charBufferRange.Length) && ((flags & num6) == 0)) && ((num4 != 0) || symbol)) && (num4 != blankGlyphIndex));
    //        }
    //        while (((num < charBufferRange.Length) && (num5 <= widthMax)) && (((flags & num6) == 0) && ((num4 != 0) || symbol)))
    //        {
    //            char ch = charBufferRange[num++];
    //            flags = Classification.CharAttributeOf(Classification.GetUnicodeClassUTF16(ch)).Flags;
    //            num3 = (ushort)(num3 & flags);
    //            characterToGlyphMap.TryGetValue(ch, out num4);
    //            num5 += emSize * typeface.GetAdvanceWidth(num4);
    //        }
    //        if (symbol)
    //        {
    //            stringLengthFit = num;
    //            return true;
    //        }
    //        if (num4 == 0)
    //        {
    //            return false;
    //        }
    //        if (((flags & num6) != 0) && (--num <= 0))
    //        {
    //            return false;
    //        }
    //        stringLengthFit = num;
    //        TypographyAvailabilities typographyAvailabilities = typeface.FontFaceLayoutInfo.TypographyAvailabilities;
    //        if ((num3 & 0x10) != 0)
    //        {
    //            if ((typographyAvailabilities & (TypographyAvailabilities.FastTextMajorLanguageLocalizedFormAvailable | TypographyAvailabilities.FastTextTypographyAvailable)) != TypographyAvailabilities.None)
    //            {
    //                return false;
    //            }
    //            if ((typographyAvailabilities & TypographyAvailabilities.FastTextExtraLanguageLocalizedFormAvailable) != TypographyAvailabilities.None)
    //            {
    //                return MajorLanguages.Contains(cultureInfo);
    //            }
    //            return true;
    //        }
    //        if ((num3 & 0x20) != 0)
    //        {
    //            return ((typographyAvailabilities & TypographyAvailabilities.IdeoTypographyAvailable) == TypographyAvailabilities.None);
    //        }
    //        return ((typographyAvailabilities & TypographyAvailabilities.Available) == TypographyAvailabilities.None);
    //    }

    //    internal bool CompareFallbackFontFamily(FontFamily fallbackFontFamily)
    //    {
    //        if ((fallbackFontFamily != null) && (this._fallbackFontFamily != null))
    //        {
    //            return this._fallbackFontFamily.Equals(fallbackFontFamily);
    //        }
    //        return (fallbackFontFamily == this._fallbackFontFamily);
    //    }

    //    private CachedTypeface ConstructCachedTypeface()
    //    {
    //        FontStyle style = this._style;
    //        FontWeight weight = this._weight;
    //        FontStretch stretch = this._stretch;
    //        FontFamily fontFamily = this.FontFamily;
    //        IFontFamily family2 = fontFamily.FindFirstFontFamilyAndFace(ref style, ref weight, ref stretch);
    //        if (family2 == null)
    //        {
    //            if (this.FallbackFontFamily != null)
    //            {
    //                fontFamily = this.FallbackFontFamily;
    //                family2 = fontFamily.FindFirstFontFamilyAndFace(ref style, ref weight, ref stretch);
    //            }
    //            if (family2 == null)
    //            {
    //                fontFamily = null;
    //                family2 = FontFamily.LookupFontFamily(FontFamily.NullFontFamilyCanonicalName);
    //            }
    //        }
    //        if ((fontFamily != null) && (fontFamily.Source != null))
    //        {
    //            IFontFamily family3 = TypefaceMetricsCache.ReadonlyLookup(fontFamily.FamilyIdentifier) as IFontFamily;
    //            if (family3 != null)
    //            {
    //                family2 = family3;
    //            }
    //            else
    //            {
    //                TypefaceMetricsCache.Add(fontFamily.FamilyIdentifier, family2);
    //            }
    //        }
    //        return new CachedTypeface(style, weight, stretch, family2, family2.GetTypefaceMetrics(style, weight, stretch), fontFamily == null);
    //    }

    //    public override bool Equals(object o)
    //    {
    //        Typeface typeface = o as Typeface;
    //        if (typeface == null)
    //        {
    //            return false;
    //        }
    //        return ((((this._style == typeface._style) && (this._weight == typeface._weight)) && ((this._stretch == typeface._stretch) && this._fontFamily.Equals(typeface._fontFamily))) && this.CompareFallbackFontFamily(typeface._fallbackFontFamily));
    //    }

    //    internal void GetCharacterNominalWidthsAndIdealWidth(CharacterBufferRange charBufferRange, double emSize, double toIdeal, out int[] nominalWidths, out int idealWidth)
    //    {
    //        GlyphTypeface typeface = this.TryGetGlyphTypeface();
    //        Invariant.Assert(typeface != null);
    //        IDictionary<int, ushort> characterToGlyphMap = typeface.CharacterToGlyphMap;
    //        nominalWidths = new int[charBufferRange.Length];
    //        idealWidth = 0;
    //        for (int i = 0; i < charBufferRange.Length; i++)
    //        {
    //            ushort num3;
    //            characterToGlyphMap.TryGetValue(charBufferRange[i], out num3);
    //            double num2 = emSize * typeface.GetAdvanceWidth(num3);
    //            nominalWidths[i] = (int)Math.Round((double)(num2 * toIdeal));
    //            idealWidth += nominalWidths[i];
    //        }
    //    }

    //    public override int GetHashCode()
    //    {
    //        int hashCode = this._fontFamily.GetHashCode();
    //        if (this._fallbackFontFamily != null)
    //        {
    //            hashCode = HashFn.HashMultiply(hashCode) + this._fallbackFontFamily.GetHashCode();
    //        }
    //        hashCode = HashFn.HashMultiply(hashCode) + this._style.GetHashCode();
    //        hashCode = HashFn.HashMultiply(hashCode) + this._weight.GetHashCode();
    //        hashCode = HashFn.HashMultiply(hashCode) + this._stretch.GetHashCode();
    //        return HashFn.HashScramble(hashCode);
    //    }

    //    internal GlyphTypeface TryGetGlyphTypeface()
    //    {
    //        return (this.CachedTypeface.TypefaceMetrics as GlyphTypeface);
    //    }

    //    public bool TryGetGlyphTypeface(out GlyphTypeface glyphTypeface)
    //    {
    //        glyphTypeface = this.CachedTypeface.TypefaceMetrics as GlyphTypeface;
    //        return (glyphTypeface != null);
    //    }

    //    internal double Baseline
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.FirstFontFamily.Baseline;
    //        }
    //    }

    //    private CachedTypeface CachedTypeface
    //    {
    //        get
    //        {
    //            if (this._cachedTypeface == null)
    //            {
    //                CachedTypeface typeface = TypefaceMetricsCache.ReadonlyLookup(this) as CachedTypeface;
    //                if (typeface == null)
    //                {
    //                    typeface = this.ConstructCachedTypeface();
    //                    TypefaceMetricsCache.Add(this, typeface);
    //                }
    //                this._cachedTypeface = typeface;
    //            }
    //            return this._cachedTypeface;
    //        }
    //    }

    //    internal FontStretch CanonicalStretch
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.CanonicalStretch;
    //        }
    //    }

    //    internal FontStyle CanonicalStyle
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.CanonicalStyle;
    //        }
    //    }

    //    internal FontWeight CanonicalWeight
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.CanonicalWeight;
    //        }
    //    }

    //    public double CapsHeight
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.TypefaceMetrics.CapsHeight;
    //        }
    //    }

    //    public LanguageSpecificStringDictionary FaceNames
    //    {
    //        get
    //        {
    //            return new LanguageSpecificStringDictionary(this.CachedTypeface.TypefaceMetrics.AdjustedFaceNames);
    //        }
    //    }

    //    internal FontFamily FallbackFontFamily
    //    {
    //        get
    //        {
    //            return this._fallbackFontFamily;
    //        }
    //    }

    //    public FontFamily FontFamily
    //    {
    //        get
    //        {
    //            return this._fontFamily;
    //        }
    //    }

    //    public bool IsBoldSimulated
    //    {
    //        get
    //        {
    //            return ((this.CachedTypeface.TypefaceMetrics.StyleSimulations & StyleSimulations.BoldSimulation) != StyleSimulations.None);
    //        }
    //    }

    //    public bool IsObliqueSimulated
    //    {
    //        get
    //        {
    //            return ((this.CachedTypeface.TypefaceMetrics.StyleSimulations & StyleSimulations.ItalicSimulation) != StyleSimulations.None);
    //        }
    //    }

    //    internal double LineSpacing
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.FirstFontFamily.LineSpacing;
    //        }
    //    }

    //    internal bool NullFont
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.NullFont;
    //        }
    //    }

    //    public FontStretch Stretch
    //    {
    //        get
    //        {
    //            return this._stretch;
    //        }
    //    }

    //    public double StrikethroughPosition
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.TypefaceMetrics.StrikethroughPosition;
    //        }
    //    }

    //    public double StrikethroughThickness
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.TypefaceMetrics.StrikethroughThickness;
    //        }
    //    }

    //    public FontStyle Style
    //    {
    //        get
    //        {
    //            return this._style;
    //        }
    //    }

    //    internal bool Symbol
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.TypefaceMetrics.Symbol;
    //        }
    //    }

    //    public double UnderlinePosition
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.TypefaceMetrics.UnderlinePosition;
    //        }
    //    }

    //    public double UnderlineThickness
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.TypefaceMetrics.UnderlineThickness;
    //        }
    //    }

    //    public FontWeight Weight
    //    {
    //        get
    //        {
    //            return this._weight;
    //        }
    //    }

    //    public double XHeight
    //    {
    //        get
    //        {
    //            return this.CachedTypeface.TypefaceMetrics.XHeight;
    //        }
    //    }
    //}

 



}
