using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class CampaignCRouteImported : CampaignImportedDataItem
    {
        [DefaultValue(0.0)]
        public float? PartPercentage { get; set; }
        [DefaultValue(false)]

        public short _IsPartModified { get; set; }
        public bool? IsPartModified {
            get
            {
                return _IsPartModified > 0;
            }
            set
            {
                if(value.HasValue && value.Value)
                {
                    _IsPartModified = 1;
                }
                else
                {
                    _IsPartModified = 0;
                }
            }
        }
        [DefaultValue(0)]
        public int? PremiumCRouteId { get; set; }
        public virtual PremiumCRoute PremiumCRoute { get; set; }
    }
}
