using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GPS.DomainLayer.Area.Addresses
{
    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class MapAddress
    {
        public MapAddress() { }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
        [DataMember]
        public double OriginalLatitude { get; set; }
        [DataMember]
        public double OriginalLongitude { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public string Color { get; set; }
        [DataMember]
        public string Picture { get; set; }
        [DataMember]
        public List<MapAddressRadius> Radiuses { get; set; }
    }
    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class MapAddressRadius
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public double Length { get; set; }
        [DataMember]
        public int LengthMeasuresId { get; set; }
        [DataMember]
        public bool IsDisplay { get; set; }
        [DataMember]
        public Dictionary<int, Dictionary<int, string>> Relations { get; set; }
    }

}
