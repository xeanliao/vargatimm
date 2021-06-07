using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class CampaignPercentageColor
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0)]
        public int? ColorId { get; set; }
        [DefaultValue(0.0)]
        public double? Max { get; set; }
        [DefaultValue(0.0)]
        public double? Min { get; set; }

        [DefaultValue(0)]
        public int? CampaignId { get; set; }
    }
}
