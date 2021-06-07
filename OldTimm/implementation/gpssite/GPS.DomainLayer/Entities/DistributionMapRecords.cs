using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class DistributionMapRecords : IAreaRecord
    {
        public DistributionMapRecords()
        {
        }
        public virtual int AreaId
        {
            get;
            set;
        }
        public virtual Classifications Classification
        {
            get;
            set;
        }
        public virtual int Id
        {
            get;
            set;
        }
        public virtual int DistributionMapId
        {
            get;
            set;
        }
        public virtual bool Value
        {
            get;
            set;
        }

        #region IAreaRecord Members

        public virtual string ClientAreaId { get; set; }

        public virtual List<List<string>> Relation { get; set; }

        #endregion
    }
}
