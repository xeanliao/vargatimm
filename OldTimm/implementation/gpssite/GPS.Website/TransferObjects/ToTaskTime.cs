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
    public class ToTaskTime
    {
        [DataMember]
        public int Id
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
        public string Time
        {
            get;
            set;
        }

        [DataMember]
        public int TimeType
        {
            get;
            set;
        }  
        

        public static void Convert(ref ToTaskTime target, ref TaskTime source)
        {
            target.Time = source.Time.ToString("yyyy-MM-dd");
            
        }


        public static void ConvertBack(ref TaskTime target, ref ToTaskTime source)
        {
            target.Time = DateTime.Parse(source.Time);
            
        }
    }
}