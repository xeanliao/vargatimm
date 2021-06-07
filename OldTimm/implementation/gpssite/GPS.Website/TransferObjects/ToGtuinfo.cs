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
using System.Web.Script.Serialization;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TaskServices")]
    public class ToGtuinfo
    {
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public string Code
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public double dwSpeed
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public int nHeading
        {
            get;
            set;
        }
        [DataMember]
        public string dtSendTime
        {
            get;
            set;
        }
        [DataMember]
        public string dtReceivedTime
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public string sIPAddress
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public int nAccuracy
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public int nCount
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public int nLocationID
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public string sVersion
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public double dwAltitude
        {
            get;
            set;
        }

        [DataMember]
        public double dwLatitude
        {
            get;
            set;
        }
        [DataMember]
        public double dwLongitude
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public int nAreaCode
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public int nNetworkCode
        {
            get;
            set;
        }
        //434 [IgnoreDataMember]
        [DataMember]
        public int nCellID
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public long nGPSFix
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public int PowerInfo
        {
            get;
            set;
        }
        [DataMember]
        public int TaskgtuinfoId
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public int Status
        {
            get;
            set;
        }
        [IgnoreDataMember]
        public double Distance
        {
            get;
            set;
        }

        public ToGtuinfo()
        {
            Code = "";
            sIPAddress = "";
            sVersion = "";
        }

        public static void Convert(ref ToGtuinfo target, ref Gtuinfo source)
        {
            target.dtReceivedTime = source.dtReceivedTime.ToString();
            target.dtSendTime = source.dtSendTime.ToString();
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


        public static void ConvertBack(ref Gtuinfo target, ref ToGtuinfo source)
        {
            target.dtReceivedTime = DateTime.Parse(source.dtReceivedTime);
            target.dtSendTime = DateTime.Parse(source.dtSendTime);
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