using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TransferObjects")]
    public class ToAdjustData
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Total { get; set; }
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public float PartPercentage { get; set; }
        [DataMember]
        public bool IsPartModified { get; set; }
    }
}
