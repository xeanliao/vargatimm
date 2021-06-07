using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class ToPrintAddress
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Address1 { get; set; }
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
        public ToPrintAddressRadius[] Radiuses { get; set; }

    }
    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class ToPrintAddressRadius
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public double Length { get; set; }
        [DataMember]
        public int LengthMeasuresId { get; set; }
        [DataMember]
        public bool IsDisplay { get; set; }
    }
}
