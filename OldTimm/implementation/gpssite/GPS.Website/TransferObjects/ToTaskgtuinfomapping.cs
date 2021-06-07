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
    [DataContract(Namespace = "TIMM.Website.TaskServices")]
    public class ToTaskgtuinfomapping
    {
        [DataMember]
        public int Id
        {
            get;
            set;
        }
        [DataMember]
        public String UserColor
        {
            get;
            set;
        }
        [DataMember]
        public int TaskId
        {
            get;
            set;
        }  
        [DataMember]
        public int UserId
        {
            get;
            set;
        }
        //[DataMember]
        //public int GTUId
        //{
        //    get;
        //    set;
        //}
        [DataMember]
        public ToGtu GTU
        {
            get;
            set;
        }
        [DataMember]
        public ToGtuinfo[] Gtuinfos
        {
            get;
            set;
        }

        public static void Convert(ref ToTaskgtuinfomapping target, ref Taskgtuinfomapping source)
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


        public static void ConvertBack(ref Taskgtuinfomapping target, ref ToTaskgtuinfomapping source)
        {
            if (source.Gtuinfos == null)
                source.Gtuinfos = new ToGtuinfo[0];

            int len=source.Gtuinfos.Length;
            if (len>0)
            {
                for (int i = 0; i < len; i++)
                {
                    var gtuinfo = AssemblerConfig.GetAssembler<Gtuinfo, ToGtuinfo>().AssembleFrom(source.Gtuinfos[i]);
                    //fix by steve.yin ToGtuinfo conver back miss datetime property (the ToGtuinfo ConvertBack method do not excute)
                    gtuinfo.dtSendTime = DateTime.Parse(source.Gtuinfos[i].dtSendTime);
                    gtuinfo.dtReceivedTime = DateTime.Parse(source.Gtuinfos[i].dtReceivedTime);
                    target.Gtuinfos.Add(gtuinfo);
                }
            
            }
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