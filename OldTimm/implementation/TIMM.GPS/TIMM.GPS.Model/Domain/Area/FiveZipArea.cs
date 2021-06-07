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
    public class FiveZipArea : AbstractArea
    {
        public FiveZipArea()
        {
            Locations = new List<FiveZipCoordinate>();
        }

        public string Code { get; set; }
        public string Description { get; set; }
        public string LSAD { get; set; }
        public string LSADTrans { get; set; }
        public string Name { get; set; }
        [DefaultValue(0)]
        public int OTotal { get; set; }
        public string StateCode { get; set; }
        [DefaultValue(0)]
        public int APT_COUNT { get; set; }
        [DefaultValue(0)]
        public int BUSINESS_COUNT { get; set; }
        [DefaultValue(0)]
        public int HOME_COUNT { get; set; }
        [DefaultValue(0)]
        public int TOTAL_COUNT { get; set; }

        public List<FiveZipCoordinate> Locations { get; set; }
    }
}
