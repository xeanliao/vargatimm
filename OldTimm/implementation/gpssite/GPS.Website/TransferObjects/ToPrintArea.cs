using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TransferObjects")]
    public class ToPrintArea
    {
        [DataMember]
        public Int32 Id
        {
            get;
            set;
        }
        [DataMember]
        public String Code
        {
            get;
            set;
        }
        [DataMember]
        public Boolean IsEnabled
        {
            get;
            set;
        }
        [DataMember]
        public ToCoordinate[] Coordinates
        {
            get;
            set;
        }
        [DataMember]
        public Int32 Total
        {
            get;
            set;
        }
        [DataMember]
        public Int32 Count
        {
            get;
            set;
        }
        [DataMember]
        public Int32 PartPercentage
        {
            get;
            set;
        }
        
    }
}
