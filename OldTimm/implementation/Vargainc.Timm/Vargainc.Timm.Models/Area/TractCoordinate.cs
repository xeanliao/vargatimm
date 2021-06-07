using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class TractCoordinate : Location
    {
        [DefaultValue(0)]
        public int? TractId { get; set; }
    }
}
