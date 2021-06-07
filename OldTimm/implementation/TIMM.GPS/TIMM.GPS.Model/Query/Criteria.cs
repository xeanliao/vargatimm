using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TIMM.GPS.Model
{
    public class Criteria
    {
        public Criteria()
        {
            PageSize = 50;
            PageIndex = 0;
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string SortField { get; set; }
        public int TotalRecord { get; set; }
    }
}
