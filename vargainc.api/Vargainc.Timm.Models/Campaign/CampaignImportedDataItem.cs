using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public abstract class CampaignImportedDataItem
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0)]
        public int? Penetration { get; set; }
        [DefaultValue(0)]
        public int? Total { get; set; }
        public int CampaignId { get; set; }
        public string Code { get; set; }
    }
}
