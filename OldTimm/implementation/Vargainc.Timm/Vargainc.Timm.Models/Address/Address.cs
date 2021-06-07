using System.Collections.Generic;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class Address
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        public string AddressName { get; set; }
        [DefaultValue(0)]
        public int? CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        public string Color { get; set; }
        [DefaultValue(0.0)]
        public float? Latitude { get; set; }
        [DefaultValue(0.0)]
        public float? Longitude { get; set; }
        [DefaultValue(0.0)]
        public float? OriginalLatitude { get; set; }
        [DefaultValue(0.0)]
        public float? OriginalLongitude { get; set; }
        public string ZipCode { get; set; }
        public string Picture { get; set; }

        public virtual ICollection<Radiuses> Radiuses { get; set; }
    }
}
