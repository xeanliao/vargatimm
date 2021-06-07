using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.Website.TransferObjects;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TransferObjects")]
    public class ToDistributionJobMap
    {
        #region properties
        [DataMember]
        public int Id
        {
            get;
            set;
        }
        [DataMember]
        public ToDistributionJob DistributionJob
        {
            get;
            set;
        }
        [DataMember]
        public ToDistributionMap DistributionMap
        {
            get;
            set;
        }
        #endregion
    }
}