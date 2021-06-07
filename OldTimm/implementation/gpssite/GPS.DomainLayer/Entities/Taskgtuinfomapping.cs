using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class Taskgtuinfomapping 
    {
        public Taskgtuinfomapping()
        {
        }
        public virtual int Id
        {
            get;
            set;
        }
        public virtual string UserColor
        {
            get;
            set;
        }
        public virtual int TaskId
        {
            get;
            set;
        }
     
        public virtual int UserId
        {
            get;
            set;
        }
        public virtual Gtu GTU
        {
            get;
            set;
        }

        #region children objects

        IList<Gtuinfo> list = new List<Gtuinfo>();

        public virtual IList<Gtuinfo> Gtuinfos { 
            get { return list; } 
            set { list = value; } 
        }

        #endregion

        
    }
}
