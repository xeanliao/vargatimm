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
using System.Collections.Generic;
using System.ComponentModel;

namespace TIMM.GPS.Model
{
    public class CampaignBlockGroupImported : CampaignImportedDataItem
    {
        [DefaultValue(0)]
        public int BlockGroupId { get; set; }
        public virtual BlockGroup BlockGroup { get; set; }
    }
}
