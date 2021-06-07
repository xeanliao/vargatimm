using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class Task 
    {
        public Task()
        {
        }
        public virtual int Id 
        { 
            get; set; 
        }
        public virtual string Name
        {
            get;
            set;
        }
        //public virtual DateTime StartTime
        //{
        //    get;
        //    set;
        //}
        //public virtual DateTime EndTime
        //{
        //    get;
        //    set;
        //}
        public virtual DateTime Date
        {
            get;
            set;
        }
        //public virtual DateTime LunchS
        //{
        //    get;
        //    set;
        //}
        //public virtual DateTime LunchE
        //{
        //    get;
        //    set;
        //}
        //public virtual DateTime BreakS
        //{
        //    get;
        //    set;
        //}
        //public virtual DateTime BreakE
        //{
        //    get;
        //    set;
        //}
        public virtual int AuditorId
        {
            get;
            set;
        }
        public virtual int DmId
        {
            get;
            set;
        }

        public virtual int Status
        {
            get;
            set;
        }

        public virtual string Telephone
        {
            get;
            set;
        }

        public virtual string Email
        {
            get;
            set;
        }

        #region children objects

        //public virtual IList<Taskgtuinfomapping> Taskgtuinfomappings { get; set; }
        //public virtual IList<TaskTime> Tasktimes { get; set; }
        IList<Taskgtuinfomapping> list = new List<Taskgtuinfomapping>();

        public virtual IList<Taskgtuinfomapping> Taskgtuinfomappings
        {
            get { return list; }
            set { list = value; }
        }

        IList<TaskTime> list2 = new List<TaskTime>();

        public virtual IList<TaskTime> Tasktimes
        {
            get { return list2; }
            set { list2 = value; }
        }


        #endregion

        
    }
}
