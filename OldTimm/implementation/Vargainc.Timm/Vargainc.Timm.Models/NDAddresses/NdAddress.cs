using System.Collections.Generic;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class NdAddress
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        public string Description { get; set; }
        [DefaultValue(0)]
        public int? Geofence { get; set; }
        [DefaultValue(0.0)]
        public double? Latitude { get; set; }
        [DefaultValue(0.0)]
        public double? Longitude { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }

        public NdAddress()
        {
            //NdAddressCoordinates = new List<NdAddressCoordinate>();
            //Locations = new List<Location>();
        }

        public virtual ICollection<NdAddressCoordinate> NdAddressCoordinates { get; set; }

        public virtual ICollection<Location> Locations { get; set; }
    }
}
