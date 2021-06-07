using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public abstract class CampaignImportedDataItem
    {
        public virtual int Id
        {
            get;
            set;
        }
        public virtual int Penetration
        {
            get;
            set;
        }
        public virtual int Total
        {
            get;
            set;
        }
        public virtual Campaign Campaign
        {
            get;
            set;
        }
    }
}
