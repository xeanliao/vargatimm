using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{

    public partial class MetropolitanCoreArea : IBoxArea
    {
        public MetropolitanCoreArea()
        {
        }
        public virtual string Code
        {
            get;
            set;
        }
        public virtual string GEOCODE
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
        public virtual string Status
        {
            get;
            set;
        }
        public virtual string Type
        {
            get;
            set;
        }

        #region children objects

        public virtual IList<MetropolitanCoreAreaCoordinate> MetropolitanCoreAreaCoordinates { get; set; }

        #endregion
    }
}
