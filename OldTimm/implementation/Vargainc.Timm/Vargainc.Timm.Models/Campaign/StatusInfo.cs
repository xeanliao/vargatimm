using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class StatusInfo
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0)]
        public int? Status { get; set; }
        [DefaultValue(0)]
        public int? CampaignId { get; set; }
        public virtual Campaign Campaign { get; set; }
        [DefaultValue(0)]
        public int? UserId { get; set; }
        public virtual User User { get; set; }
    }
}
