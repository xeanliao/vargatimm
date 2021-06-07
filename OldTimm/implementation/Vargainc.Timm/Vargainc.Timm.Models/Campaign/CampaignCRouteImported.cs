using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class CampaignCRouteImported : CampaignImportedDataItem
    {
        [DefaultValue(0.0)]
        public float? PartPercentage { get; set; }
        [DefaultValue(false)]
        public bool? IsPartModified { get; set; }
        [DefaultValue(0)]
        public int? PremiumCRouteId { get; set; }
        public virtual PremiumCRoute PremiumCRoute { get; set; }
    }
}
