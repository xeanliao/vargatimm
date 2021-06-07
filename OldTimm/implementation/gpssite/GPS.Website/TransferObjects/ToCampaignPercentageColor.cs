using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TransferObjects")]
    public class ToCampaignPercentageColor
    {
        [DataMember]
        public int CampaignId
        {
            get;
            set;
        }
        [DataMember]
        public int ColorId
        {
            get;
            set;
        }
        [DataMember]
        public int Id
        {
            get;
            set;
        }
        [DataMember]
        public double Max
        {
            get;
            set;
        }
        [DataMember]
        public double Min
        {
            get;
            set;
        }
    }
}
