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

namespace System.Windows.Media
{
    public class Pen
    {
        static Pen()
        {
            Type ownerType = typeof(Pen);
            //BrushProperty = Animatable.RegisterProperty("Brush", typeof(Brush), ownerType, null, new PropertyChangedCallback(Pen.BrushPropertyChanged), null, false, null);
            //ThicknessProperty = Animatable.RegisterProperty("Thickness", typeof(double), ownerType, 1.0, new PropertyChangedCallback(Pen.ThicknessPropertyChanged), null, true, null);
            //StartLineCapProperty = Animatable.RegisterProperty("StartLineCap", typeof(PenLineCap), ownerType, PenLineCap.Flat, new PropertyChangedCallback(Pen.StartLineCapPropertyChanged), new ValidateValueCallback(ValidateEnums.IsPenLineCapValid), false, null);
            //EndLineCapProperty = Animatable.RegisterProperty("EndLineCap", typeof(PenLineCap), ownerType, PenLineCap.Flat, new PropertyChangedCallback(Pen.EndLineCapPropertyChanged), new ValidateValueCallback(ValidateEnums.IsPenLineCapValid), false, null);
            //DashCapProperty = Animatable.RegisterProperty("DashCap", typeof(PenLineCap), ownerType, PenLineCap.Square, new PropertyChangedCallback(Pen.DashCapPropertyChanged), new ValidateValueCallback(ValidateEnums.IsPenLineCapValid), false, null);
            //LineJoinProperty = Animatable.RegisterProperty("LineJoin", typeof(PenLineJoin), ownerType, PenLineJoin.Miter, new PropertyChangedCallback(Pen.LineJoinPropertyChanged), new ValidateValueCallback(ValidateEnums.IsPenLineJoinValid), false, null);
            //MiterLimitProperty = Animatable.RegisterProperty("MiterLimit", typeof(double), ownerType, 10.0, new PropertyChangedCallback(Pen.MiterLimitPropertyChanged), null, false, null);
            //DashStyleProperty = Animatable.RegisterProperty("DashStyle", typeof(DashStyle), ownerType, DashStyles.Solid, new PropertyChangedCallback(Pen.DashStylePropertyChanged), null, false, null);
        }

        public Pen()
        {
            //this._duceResource = new DUCE.MultiChannelResource();
        }

        public Pen(Brush brush, double thickness)
        {
            //this._duceResource = new DUCE.MultiChannelResource();
            this.Brush = brush;
            this.Thickness = thickness;
        }

        internal Pen(Brush brush, double thickness, PenLineCap startLineCap, PenLineCap endLineCap, PenLineCap dashCap, PenLineJoin lineJoin, double miterLimit, DashStyle dashStyle)
        {
            //this._duceResource = new DUCE.MultiChannelResource();
            this.Thickness = thickness;
            this.StartLineCap = startLineCap;
            this.EndLineCap = endLineCap;
            this.DashCap = dashCap;
            this.LineJoin = lineJoin;
            this.MiterLimit = miterLimit;
            this.Brush = brush;
            this.DashStyle = dashStyle;
        }


        public Brush Brush { get; set; }
        public PenLineCap DashCap { get; set; }
        public DashStyle DashStyle { get; set; }
        //internal bool DoesNotContainGaps { get; }
        public PenLineCap EndLineCap { get; set; }
        public PenLineJoin LineJoin { get; set; }
        public double MiterLimit { get; set; }
        //internal override bool RequiresRealizationUpdates { get; }
        public PenLineCap StartLineCap { get; set; }
        public double Thickness { get; set; }


    }
}
