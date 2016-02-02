using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class CampaignRecord
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0)]
        public int? AreaId { get; set; }
        [DefaultValue(0)]
        public int? Classification { get; set; }
        [DefaultValue(false)]
        public bool? Value { get; set; }
        [DefaultValue(0)]
        public int CampaignId { get; set; }

        public virtual Campaign Campaign { get; set; }
    }
}
