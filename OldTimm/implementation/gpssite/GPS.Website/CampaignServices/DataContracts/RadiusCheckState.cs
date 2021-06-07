using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GPS.Website.CampaignServices.DataContracts {
    [DataContract(Namespace = "TIMM.Website.CampaignServices.DataContracts")]
    public struct RadiusCheckState {
        [DataMember]
        public Int32 AddressId;
        [DataMember]
        public Int32 RadiusLevel;
        [DataMember]
        public Boolean IsChecked;
    }
}
