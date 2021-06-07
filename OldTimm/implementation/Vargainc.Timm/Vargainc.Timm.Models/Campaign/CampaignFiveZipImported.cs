using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class CampaignFiveZipImported : CampaignImportedDataItem
    {
        [DefaultValue(0.0)]
        public float? PartPercentage { get; set; }
        [DefaultValue(false)]
        public bool? IsPartModified { get; set; }
        [DefaultValue(0)]
        public int? FiveZipAreaId { get; set; }
        public virtual FiveZipArea FiveZipArea { get; set; }
    }
}
