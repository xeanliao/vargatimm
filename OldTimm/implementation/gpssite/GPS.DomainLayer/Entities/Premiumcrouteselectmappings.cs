using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class PremiumCRouteSelectMapping
    {
        public virtual int Id
        {
            get;
            set;
        }
        public virtual int ThreeZipAreaId
        {
            get;
            set;
        }
        public virtual int FiveZipAreaId
        {
            get;
            set;
        }
        public virtual int PremiumCRouteId
        {
            get;
            set;
        }
    }
}
