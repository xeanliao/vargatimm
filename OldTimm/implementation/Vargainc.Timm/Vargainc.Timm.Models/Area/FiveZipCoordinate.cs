using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class FiveZipCoordinate : Location
    {
        [DefaultValue(0)]
        public int? FiveZipAreaId { get; set; }
    }
}
