using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class PremiumCRouteCoordinate : Location
    {
        [DefaultValue(0)]
        public int? PreminumCRouteId { get; set; }
    }
}
