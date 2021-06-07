using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class Gtuinfo
    {
        public Gtuinfo()
        {
        }
        public virtual int Id
        {
            get;
            set;
        }

        public virtual string Code
        {
            get;
            set;
        }

        public virtual double dwSpeed
        {
            get;
            set;
        }
        public virtual int nHeading
        {
            get;
            set;
        }
        public virtual DateTime dtSendTime
        {
            get;
            set;
        }
        public virtual DateTime dtReceivedTime
        {
            get;
            set;
        }
        public virtual string sIPAddress
        {
            get;
            set;
        }
        public virtual int nAreaCode
        {
            get;
            set;
        }
        public virtual int nNetworkCode
        {
            get;
            set;
        }
        public virtual int nCellID
        {
            get;
            set;
        }
       public virtual long nGPSFix
        {
            get;
            set;
        }
        public virtual int nAccuracy
        {
            get;
            set;
        }
        public virtual int nCount
        {
            get;
            set;
        }
        public virtual int nLocationID
        {
            get;
            set;
        }
        public virtual string sVersion
        {
            get;
            set;
        }
        public virtual double dwAltitude
        {
            get;
            set;
        }
        public virtual double dwLatitude
        {
            get;
            set;
        }
        public virtual double dwLongitude
        {
            get;
            set;
        }
        public virtual int PowerInfo
        {
            get;
            set;
        }
        public virtual int TaskgtuinfoId
        {
            get;
            set;
        }
        public virtual int Status
        {
            get;
            set;
        }
        public virtual double Distance
        {
            get;
            set;
        }

        
    }
}
