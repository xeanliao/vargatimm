using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class CampaignTractImported : CampaignImportedDataItem
    {
        [DefaultValue(0)]
        public int? TractId { get; set; }
        public virtual Tract Tract { get; set; }
    }
}
