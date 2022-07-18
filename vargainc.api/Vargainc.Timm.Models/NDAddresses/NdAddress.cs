using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Spatial;

namespace Vargainc.Timm.Models
{
    public class NdAddress
    {
        public int? Id { get; set; }
        public string Description { get; set; }
        public int? Geofence { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public DbGeometry Polygon { get; set; }
    }
}
