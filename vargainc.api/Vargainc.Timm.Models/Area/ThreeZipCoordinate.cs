using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class ThreeZipCoordinate : Location
    {
        [DefaultValue(0)]
        public int? ThreeZipAreaId { get; set; }
    }
}
