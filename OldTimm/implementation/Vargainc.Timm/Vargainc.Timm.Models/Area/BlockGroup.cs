using System.Collections.Generic;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class BlockGroup : AbstractArea
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string CountyCode { get; set; }
        public string TractCode { get; set; }
        public string LSAD { get; set; }
        public string LSADTrans { get; set; }
        [DefaultValue(0)]
        public int? OTotal { get; set; }
        public string Description { get; set; }
        public string ArbitraryUniqueCode { get; set; }

        public virtual ICollection<BlockGroupCoordinate> Locations { get; set; }
    }
}
