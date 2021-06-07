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
    public class ToDistributionJob
    {
        #region properties
        [DataMember]
        public int Id
        {
            get;
            set;
        }
        [DataMember]
        public String Name
        {
            get;
            set;
        }
        [DataMember]
        public ToCampaign Campaign
        {
            get;
            set;
        }

        [DataMember]
        public ToDistributionMap[] DistributionMaps
        {
            get;
            set;
        }

        [DataMember]
        public ToDriver[] DriverAssignments
        {
            get;
            set;
        }

        [DataMember]
        public ToAuditor AuditorAssignment
        {
            get;
            set;
        }

        [DataMember]
        public ToWalker[] WalkerAssignments
        {
            get;
            set;
        }

        [DataMember]
        public int CampaignID
        {
            get;
            set;
        }

        #endregion

    }
}

