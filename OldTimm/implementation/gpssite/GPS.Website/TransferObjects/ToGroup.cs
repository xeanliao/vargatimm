using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.Website.TransferObjects;
using GPS.Website.AppFacilities;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.GroupServices")]
    public class ToGroup
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
        public ToPrivilege[] Privileges
        {
            get;
            set;
        }

        public static void Convert(ref ToGroup target, ref Group source)
        {
            //target.Name = source.Name;

            //if (source.Privileges != null)
            //{
            //    foreach (Privilege p in source.Privileges)
            //    {
            //        ToPrivilege tp = AssemblerConfig.GetAssembler<ToPrivilege, Privilege>().AssembleFrom(p);
            //        target.Privileges.Add(tp);
            //    }
            //}
        }


        public static void ConvertBack(ref Group target, ref ToGroup source)
        {
            //target.Name = source.Name;

            //if (source.Privileges != null)
            //{
            //    foreach (ToPrivilege p in source.Privileges)
            //    {
            //        Privilege tp = AssemblerConfig.GetAssembler<Privilege, ToPrivilege>().AssembleFrom(p);
            //        target.Privileges.Add(tp);
            //    }
            //}
        }
    }
}