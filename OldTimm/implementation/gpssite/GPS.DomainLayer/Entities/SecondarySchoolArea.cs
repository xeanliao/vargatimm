using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class SecondarySchoolArea : IBoxArea
    {
        public SecondarySchoolArea()
        {
        }
        public virtual string Code
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
        public virtual string LSAD
        {
            get;
            set;
        }
        public virtual string LSAD_TRAN
        {
            get;
            set;
        }
        public virtual double MaxLatitude
        {
            get;
            set;
        }
        public virtual double MaxLongitude
        {
            get;
            set;
        }
        public virtual double MinLatitude
        {
            get;
            set;
        }
        public virtual double MinLongitude
        {
            get;
            set;
        }
        public virtual string Name
        {
            get;
            set;
        }
        public virtual string StateCode
        {
            get;
            set;
        }

        #region children objects

        public virtual IList<SecondarySchoolAreaCoordinate> SecondarySchoolAreaCoordinates { get; set; }

        #endregion
    }
}
