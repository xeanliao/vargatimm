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
#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange (mailto:Stefan.Lange@pdfsharp.com)
//
// Copyright (c) 2005-2008 empira Software GmbH, Cologne (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
using System.IO;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
#endif
#if WPF
using System.Windows.Media;
#endif
using PdfSharp.Pdf;
using PdfSharp.Fonts.TrueType;

// WPFHACK
#pragma warning disable 162

namespace PdfSharp.Drawing
{
  /// <summary>
  /// Defines a group of type faces having a similar basic design and certain variations in styles.
  /// </summary>
  public sealed class XFontFamily
  {
    internal XFontFamily() { }

#if GDI
    internal XFontFamily(System.Drawing.FontFamily family)
    {
      this.name = family.Name;
      this.gdiFamily = family;
#if WPF
      this.wpfFamily = new System.Windows.Media.FontFamily(family.Name);
#endif
    }
#endif

    internal XFontFamily(System.Windows.Media.FontFamily family)
    {
      this.name = family.Source;
      this.wpfFamily = family;
#if GDI
      this.gdiFamily = new System.Drawing.FontFamily(family.Source);
#endif
    }


    //internal FontFamily();
    //public FontFamily(GenericFontFamilies genericFamily);
    //internal FontFamily(IntPtr family);

    /// <summary>
    /// Initializes a new instance of the <see cref="XFontFamily"/> class.
    /// </summary>
    /// <param name="name">The family name of a font.</param>
    public XFontFamily(string name)
    {
      this.name = name;
#if GDI
      this.gdiFamily = new System.Drawing.FontFamily(name);
#endif
      this.wpfFamily = new System.Windows.Media.FontFamily(name);
    }

    //public FontFamily(string name, FontCollection fontCollection);

    //public override bool Equals(object obj);


    /// <summary>
    /// Gets the name of the font family.
    /// </summary>
    public string Name
    {
      get { return this.name; }
    }
    string name;

    /// <summary>
    /// Returns the cell ascent, in design units, of the XFontFamily object of the specified style.
    /// </summary>
    public int GetCellAscent(XFontStyle style)
    {
#if GDI && !WPF
      return this.gdiFamily.GetCellAscent((FontStyle)style);
#endif

      return FontHelper.GetWpfValue(this, style, GWV.GetCellAscent);

#if WPF && GDI
#if DEBUG
      int gdiResult = this.gdiFamily.GetCellAscent((FontStyle)style);
      int wpfResult = FontHelper.GetWpfValue(this, style, GWV.GetCellAscent);
      Debug.Assert(gdiResult == wpfResult, "GDI+ and WPF provides different values.");
#endif
      return FontHelper.GetWpfValue(this, style, GWV.GetCellAscent);
#endif
    }

    /// <summary>
    /// Returns the cell descent, in design units, of the XFontFamily object of the specified style.
    /// </summary>
    public int GetCellDescent(XFontStyle style)
    {
#if GDI && !WPF
      return this.gdiFamily.GetCellDescent((FontStyle)style);
#endif

      return FontHelper.GetWpfValue(this, style, GWV.GetCellDescent);

#if WPF && GDI
#if DEBUG
      int gdiResult = this.gdiFamily.GetCellDescent((FontStyle)style);
      int wpfResult = FontHelper.GetWpfValue(this, style, GWV.GetCellDescent);
      Debug.Assert(gdiResult == wpfResult, "GDI+ and WPF provides different values.");
#endif
      return FontHelper.GetWpfValue(this, style, GWV.GetCellDescent);
#endif
    }

    /// <summary>
    /// Gets the height, in font design units, of the em square for the specified style.
    /// </summary>
    public int GetEmHeight(XFontStyle style)
    {
#if GDI && !WPF
      return this.gdiFamily.GetEmHeight((FontStyle)style);
#endif

      return FontHelper.GetWpfValue(this, style, GWV.GetEmHeight);

#if WPF && GDI
#if DEBUG
      int gdiResult = this.gdiFamily.GetEmHeight((FontStyle)style);
      int wpfResult = FontHelper.GetWpfValue(this, style, GWV.GetEmHeight);
      Debug.Assert(gdiResult == wpfResult, "GDI+ and WPF provides different values.");
#endif
      return FontHelper.GetWpfValue(this, style, GWV.GetEmHeight);
#endif
    }

    //public override int GetHashCode();

    /// <summary>
    /// Returns the line spacing, in design units, of the FontFamily object of the specified style.
    /// The line spacing is the vertical distance between the base lines of two consecutive lines of text.
    /// </summary>
    public int GetLineSpacing(XFontStyle style)
    {
#if GDI && !WPF
      return this.gdiFamily.GetLineSpacing((FontStyle)style);
#endif

      return FontHelper.GetWpfValue(this, style, GWV.GetLineSpacing);

#if WPF && GDI
#if DEBUG
      int gdiResult = this.gdiFamily.GetLineSpacing((FontStyle)style);
      int wpfResult = FontHelper.GetWpfValue(this, style, GWV.GetLineSpacing);
      //Debug.Assert(gdiResult == wpfResult, "GDI+ and WPF provides different values.");
#endif
      return FontHelper.GetWpfValue(this, style, GWV.GetLineSpacing);
#endif
    }

    //public string GetName(int language);

    /// <summary>
    /// Indicates whether the specified FontStyle enumeration is available.
    /// </summary>
    public bool IsStyleAvailable(XFontStyle style)
    {
#if GDI && !WPF
      return this.gdiFamily.IsStyleAvailable((FontStyle)style);
#endif

      return FontHelper.GetWpfValue(this, style, GWV.IsStyleAvailable) == 1;

#if WPF && GDI
#if DEBUG
      bool gdiResult = this.gdiFamily.IsStyleAvailable((FontStyle)style);
      bool wpfResult = FontHelper.GetWpfValue(this, style, GWV.IsStyleAvailable) == 1;
      // TODOWPF
      //Debug.Assert(gdiResult == wpfResult, "GDI+ and WPF provides different values.");
#endif
      return FontHelper.GetWpfValue(this, style, GWV.IsStyleAvailable) == 1;
#endif
    }

    //internal void SetNative(IntPtr native);
    //public override string ToString();
    //
    //// Properties
    //private static int CurrentLanguage { get; }

    /// <summary>
    /// Returns an array that contains all the FontFamily objects associated with the current graphics context.
    /// </summary>
    public static XFontFamily[] Families
    {
      get
      {
        //System.Windows.Media.Fonts.GetFontFamilies(
        // TODOWPF
        return null;
      }
    }

    /// <summary>
    /// Returns an array that contains all the FontFamily objects available for the specified 
    /// graphics context.
    /// </summary>
    public static XFontFamily[] GetFamilies(XGraphics graphics)
    {
      XFontFamily[] result;
      result = null;
      return result;
    }

    //public static FontFamily GenericMonospace { get; }
    //public static FontFamily GenericSansSerif { get; }
    //public static FontFamily GenericSerif { get; }
    //public string Name { get; }


    /// <summary>
    /// WPF object.
    /// </summary>
    internal System.Windows.Media.FontFamily wpfFamily;
  }
}