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
    [DataContract(Namespace = "TIMM.Website.UserServices")]
    public class ToUserRole
    {
        [DataMember]
        public String RoleName
        {
            get;
            set;
        }
        [DataMember]
        public int RoleValue
        {
            get;
            set;
        }
    }
}
