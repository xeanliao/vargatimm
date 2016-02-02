using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class CampaignClassification
    {
        [DefaultValue(0)]
        public int? Id { get; set; }

        [DefaultValue(0)]
        public int? Classification { get; set; }

        [DefaultValue(0)]
        public int CampaignId { get; set; }

        public virtual Campaign Campaign { get; set; }
    }
}
