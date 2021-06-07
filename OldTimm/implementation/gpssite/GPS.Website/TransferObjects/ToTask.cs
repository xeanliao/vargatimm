
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
    public class ToTask
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

        //[DataMember]
        //public DateTime StartTime
        //{
        //    get;
        //    set;
        //}
        //[DataMember]
        //public DateTime EndTime
        //{
        //    get;
        //    set;
        //}

        [DataMember]
        public string Date
        {
            get;
            set;
        }
        //[DataMember]
        //public DateTime LunchS
        //{
        //    get;
        //    set;
        //}
        //[DataMember]
        //public DateTime LunchE
        //{
        //    get;
        //    set;
        //}
        //[DataMember]
        //public DateTime BreakS
        //{
        //    get;
        //    set;
        //}
        //[DataMember]
        //public DateTime BreakE
        //{
        //    get;
        //    set;
        //}

        [DataMember]
        public int AuditorId
        {
            get;
            set;
        }  
        [DataMember]
        public int DmId
        {
            get;
            set;
        }

        [DataMember]
        public int Status
        {
            get;
            set;
        }

        [DataMember]
        public string Telephone
        {
            get;
            set;
        }

        [DataMember]
        public string Email
        {
            get;
            set;
        }

        [DataMember]
        public ToTaskgtuinfomapping[] Taskgtuinfomappings
        {
            get;
            set;
        }

        [DataMember]
        public ToTaskTime[] Tasktimes
        {
            get;
            set;
        }

        public static void Convert(ref ToTask target, ref Task source)
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
            //to fix datetime to string serializer
            if (source.Taskgtuinfomappings != null && source.Taskgtuinfomappings.Count > 0)
            {
                for (int i = 0; i < source.Taskgtuinfomappings.Count; i++)
                {
                    if (source.Taskgtuinfomappings[i].Gtuinfos != null && source.Taskgtuinfomappings[i].Gtuinfos.Count > 0)
                    {
                        for (int j = 0; j < source.Taskgtuinfomappings[i].Gtuinfos.Count; j++)
                        {
                            target.Taskgtuinfomappings[i].Gtuinfos[j].dtSendTime = source.Taskgtuinfomappings[i].Gtuinfos[j].dtSendTime.ToString();
                            target.Taskgtuinfomappings[i].Gtuinfos[j].dtReceivedTime = source.Taskgtuinfomappings[i].Gtuinfos[j].dtReceivedTime.ToString();
                        }
                    }
                }
            }
            target.Date = source.Date.ToString("yyyy-MM-dd");
        }


        public static void ConvertBack(ref Task target, ref ToTask source)
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
            if(source.Taskgtuinfomappings==null)
                source.Taskgtuinfomappings = new ToTaskgtuinfomapping[0];
            if (source.Tasktimes == null)
                source.Tasktimes = new ToTaskTime[0];

            int len = source.Taskgtuinfomappings.Length;
            int len2=source.Tasktimes.Length;
            if (len > 0)
            {
                for (int i = 0; i < len; i++)
                    target.Taskgtuinfomappings.Add(AssemblerConfig.GetAssembler<Taskgtuinfomapping, ToTaskgtuinfomapping>().AssembleFrom(source.Taskgtuinfomappings[i]));
            }
            if (len2 > 0)
            { 
                for(int j=0;j<len2;j++)
                 target.Tasktimes.Add(AssemblerConfig.GetAssembler<TaskTime, ToTaskTime>().AssembleFrom(source.Tasktimes[j]));
            }
            
            target.Date = DateTime.Parse(source.Date);
        }
    }
}