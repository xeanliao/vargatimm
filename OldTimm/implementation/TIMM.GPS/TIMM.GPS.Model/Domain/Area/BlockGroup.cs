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
    public class BlockGroup : AbstractArea
    {
        public BlockGroup()
        {
            Locations = new List<BlockGroupCoordinate>();
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string CountyCode { get; set; }
        public string TractCode { get; set; }
        public string LSAD { get; set; }
        public string LSADTrans { get; set; }
        [DefaultValue(0)]
        public int OTotal { get; set; }
        public string Description { get; set; }
        public string ArbitraryUniqueCode { get; set; }

        public List<BlockGroupCoordinate> Locations { get; set; }
    }
}
