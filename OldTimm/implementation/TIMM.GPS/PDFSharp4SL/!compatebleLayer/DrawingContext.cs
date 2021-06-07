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
using System.Collections;
using System.Collections.Generic;

namespace System.Windows.Media
{
    /// <summary>
    /// Описывает визуальное содержимое с использованием команд рисования, проталкивания и выталкивания.
    /// </summary>
    public class DrawingContext
    {
        private List<MatrixTransform> _matrixStack = new List<MatrixTransform>();
 
        public DrawingContext()
        {
            
        }
        
        /// <summary>
        /// Закрывает DrawingContext и сбрасывает содержимое. После этого изменение DrawingContext становится невозможным.
        /// </summary>
        public void Close()
        {
            //throw new NotImplementedException();
        }
        
        /// <summary>
        /// Проталкивает заданный объект Transform в контекст рисования
        /// </summary>
        public void PushTransform(MatrixTransform transform)
        {
            _matrixStack.Add(transform);
        }
        
        /// <summary>
        /// Выталкивает последнюю маску непрозрачности, непрозрачность, клип, эффект или операцию преобразования, 
        /// которые были протолкнуты в контекст рисования. 
        /// </summary>
        public void Pop()
        {
            //PushClip
            //PushEffect            
            //PushGuidelineSet      
            //PushOpacity             
            //PushOpacityMask         
            //PushTransform  

            //throw new NotImplementedException();
        }
           
        /// <summary>
        /// Рисует заданный Geometry с использованием заданных Brush и Pen.
        /// </summary>
        public void DrawGeometry(Brush wpfBrush, Pen wpfPen, PathGeometry geo)
        {
            //throw new NotImplementedException();
        }
        
        /// <summary>
        /// Проталкивает заданную область клипа в контекст рисования.
        /// </summary>
        public void PushClip(Geometry geometry)
        {
            //throw new NotImplementedException();
        }
        
        /// <summary>
        /// Перегружен. Рисует линию заданным Pen. 
        /// </summary>
        public void DrawLine(Pen pen, Point point, Point point_3)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Перегружен. Рисует прямоугольник с закругленными углами. 
        /// </summary>
        public void DrawRoundedRectangle(Brush brush, Pen pen, Rect rect, double p, double p_5)
        {
            //throw new NotImplementedException();
        }
        
        /// <summary>
        /// Перегружен. Рисует прямоугольник. 
        /// </summary>
        public void DrawRectangle(Brush brush, Pen pen, Rect rect)
        {
            //throw new NotImplementedException();
        }
        
        /// <summary>
        /// Перегружен. Рисует изображения в области, определенной заданным Rect. 
        /// </summary>
        public void DrawImage(System.Windows.Media.Imaging.BitmapSource bitmapSource, Rect rect)
        {
            //throw new NotImplementedException();
        }
        
        /// <summary>
        /// Перегружен. Рисует эллипс. 
        /// </summary>
        public void DrawEllipse(Brush brush, Pen pen, Point point, double radiusX, double radiusY)
        {
            //throw new NotImplementedException();
        }
        
        /// <summary>
        /// Рисует текст в указанной позиции. 
        /// </summary>
        public void DrawText(FormattedText formattedText, Point point)
        {
            //throw new NotImplementedException();
        }
    }
}
