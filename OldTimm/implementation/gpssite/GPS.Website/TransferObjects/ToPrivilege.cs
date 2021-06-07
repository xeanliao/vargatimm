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
    [DataContract(Namespace = "TIMM.Website.GroupServices")]
    public class ToPrivilege
    {
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
        public int Value
        {
            get;
            set;
        }
    }
}