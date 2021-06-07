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

namespace TIMM.GPS.ControlCenter.UserControls.Print
{
    public class PdfPageBase : UserControl
    {
        public void Scale(double percent)
        {
            var transform = new ScaleTransform
            {
                ScaleX = percent,
                ScaleY = percent
            };
            this.RenderTransform = transform;
            this.UpdateLayout();
        }
    }
}
