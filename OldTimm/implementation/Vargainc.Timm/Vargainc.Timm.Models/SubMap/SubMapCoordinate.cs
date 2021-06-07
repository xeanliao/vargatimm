using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class SubMapCoordinate
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0.0)]
        public double? Latitude { get; set; }
        [DefaultValue(0.0)]
        public double? Longitude { get; set; }
        [DefaultValue(0)]
        public int? SubMapId { get; set; }
        public virtual SubMap SubMap { get; set; }
        
    }
}
