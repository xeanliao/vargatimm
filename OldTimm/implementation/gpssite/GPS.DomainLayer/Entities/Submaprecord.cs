using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Enum;

namespace GPS.DomainLayer.Entities
{
    public class SubMapRecord : IAreaRecord
    {
        public SubMapRecord()
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
        public virtual int SubMapId
        {
            get { return SubMap.Id; }
        }
        public virtual SubMap SubMap { get; set; }
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
