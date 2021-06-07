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
using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.ControlCenter.Interface;
using TIMM.GPS.Net.Http;

namespace TIMM.GPS.ControlCenter.UserControls
{
    public partial class PrintFooter : UserControl
    {
    
        public CampaignUI Campaign {get; set;}

        public PrintFooter(CampaignUI campaign)
        {
            InitializeComponent();

            Campaign = campaign;
            this.DataContext = Campaign;
        }
    }
}
