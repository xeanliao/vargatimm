using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class NdAddressCoordinate : Location
    {
        [DefaultValue(0)]
        public int? NdAddressId { get; set; }
    }
}
