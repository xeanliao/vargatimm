using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class DistributionMap
    {
        #region basic properties
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int SubMapId { get; set; }

        public virtual int ColorB
        {
            get;
            set;
        }
        public virtual int ColorG
        {
            get;
            set;
        }
        public virtual int ColorR
        {
            get;
            set;
        }
        public virtual string ColorString
        {
            get;
            set;
        }

        public virtual int Penetration
        {
            get;
            set;
        }
        public virtual double Percentage
        {
            get;
            set;
        }
        public virtual int Total
        {
            get;
            set;
        }
        public virtual int TotalAdjustment
        {
            get;
            set;
        }
        public virtual int CountAdjustment
        {
            get;
            set;
        }
        public virtual List<double[][]> Holes { get; set; }
        #endregion

        #region children objects 
        public virtual IList<DistributionMapRecords> DistributionMapRecords { get; set; }
        public virtual IList<DistributionMapCoordinate> DistributionMapCoordinates { get; set; }
        public virtual IList<Task> Tasks { get; set; }
        #endregion
    }
}
