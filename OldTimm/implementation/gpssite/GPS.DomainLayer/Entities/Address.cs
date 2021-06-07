﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class Address : IAddress
    {
        public Address()
        {
        }
        public virtual string Address1
        {
            get;
            set;
        }
        public virtual int CampaignId
        {
            get { return Campaign.Id; }
        }
        public virtual Campaign Campaign
        {
            get;
            set;
        }
        public virtual string Color
        {
            get;
            set;
        }
        public virtual int Id
        {
            get;
            set;
        }
        public virtual double Latitude
        {
            get;
            set;
        }
        public virtual double Longitude
        {
            get;
            set;
        }
        public virtual double OriginalLatitude
        {
            get;
            set;
        }
        public virtual double OriginalLongitude
        {
            get;
            set;
        }
        public virtual string ZipCode
        {
            get;
            set;
        }
        public virtual string Picture { get; set; }

        #region children objects

        public virtual IList<Radiuse> Radiuses { get; set; }

        #endregion

        #region IAddress Members
        public virtual List<IRadius> GPSRadiuses { get; set; }
        #endregion
    }
}
