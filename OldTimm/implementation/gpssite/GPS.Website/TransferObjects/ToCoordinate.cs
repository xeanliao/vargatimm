using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TransferObjects")]
    public class ToCoordinate
    {
        [DataMember]
        public double Latitude
        {
            get;
            set;
        }
        [DataMember]
        public double Longitude
        {
            get;
            set;
        }
    }
}
