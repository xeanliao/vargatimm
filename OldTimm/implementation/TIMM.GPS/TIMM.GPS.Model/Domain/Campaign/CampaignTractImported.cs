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
using System.ComponentModel;

namespace TIMM.GPS.Model
{
    public class CampaignTractImported : CampaignImportedDataItem
    {
        [DefaultValue(0)]
        public int TractId { get; set; }
        public virtual Tract Tract { get; set; }
    }
}
