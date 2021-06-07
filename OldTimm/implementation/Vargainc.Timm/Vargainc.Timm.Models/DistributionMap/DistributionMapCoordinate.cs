using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class DistributionMapCoordinate
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0.0)]
        public double? Latitude { get; set; }
        [DefaultValue(0.0)]
        public double? Longitude { get; set; }
        [DefaultValue(0)]
        public int? DistributionMapId { get; set; }
        public virtual DistributionMap DistributionMap { get; set; }
    }
}
