using System.Collections.Generic;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class SubMapDetailItem
    {
        public SubMapDetailItem()
        {
            //Locations = new List<Location>();
        }
        [DefaultValue(0)]
        public int? OrderId { get; set; }
        [DefaultValue(0)]
        public int? AreaId { get; set; }
        [DefaultValue(0)]
        public int? Classification { get; set; }
        public string Name { get; set; }        
        public int? TotalHouseHold { get; set; }
        public int? TargetHouseHold { get; set; }
        public double? DisplayPenetration
        {
            get
            {
                if (TotalHouseHold.HasValue && TargetHouseHold.HasValue)
                {
                    if (TotalHouseHold == 0)//denominator is zero
                        return 0;
                    return (double)TargetHouseHold / (double)TotalHouseHold;
                }
                return null;
            }
        }

        public virtual ICollection<Location> Locations { get; set; }
    }
}
