using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TIMM.GPS.ControlCenter.UserControls
{
    public partial class PrintCover : UserControl
    {
        public string ClientName { get; set; }

        public PrintCover()
        {
            InitializeComponent();
        }
    }
}
