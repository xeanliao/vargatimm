using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class CampaignBlockGroupImported : CampaignImportedDataItem
    {
        [DefaultValue(0)]
        public int? BlockGroupId { get; set; }
        public virtual BlockGroup BlockGroup { get; set; }
    }
}
