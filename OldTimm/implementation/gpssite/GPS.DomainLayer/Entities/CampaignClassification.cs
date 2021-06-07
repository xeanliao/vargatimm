using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class CampaignClassification
    {
        public virtual Int32 Id { get; set; }

        public virtual Int32 Classification { get; set; }

        public virtual Campaign Campaign { get; set; }
    }
}
