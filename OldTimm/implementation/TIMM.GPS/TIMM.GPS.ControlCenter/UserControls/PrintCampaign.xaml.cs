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
using System.Windows.Media.Imaging;

namespace TIMM.GPS.ControlCenter.UserControls
{
    public partial class PrintCampaign : UserControl
    {
        public PrintCampaign(ImageSource source)
        {
            InitializeComponent();
            CampaignImage.Source = source;
        }
    }
}
