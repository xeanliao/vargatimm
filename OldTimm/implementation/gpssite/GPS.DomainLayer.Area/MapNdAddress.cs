using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Area
{
    public class MapNdAddress
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public int Geofence { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<ICoordinate> Locations { get; set; }
        public string Description { get; set; }
    }
}
