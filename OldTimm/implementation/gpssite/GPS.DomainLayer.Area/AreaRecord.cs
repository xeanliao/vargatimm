using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;

namespace GPS.DomainLayer.Area
{
    public class AreaRecord
    {
        public Classifications Classification { get; set; }
        public Int32 AreaId { get; set; }
        public Boolean Value { get; set; }
        public Dictionary<int, Dictionary<int, bool>> Relations { get; set; }
    }
}
