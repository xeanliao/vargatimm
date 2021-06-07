using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TIMM.Jobs.NonDeliverableAddresses
{
    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class ResultAddress
    {
        [DataMember]
        public Boolean IsSuccess { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
        [DataMember]
        public int Geofence { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public double[][] Locations { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
