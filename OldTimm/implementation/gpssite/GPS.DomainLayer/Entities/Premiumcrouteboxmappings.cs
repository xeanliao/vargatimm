using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class PremiumCRouteBoxMapping : AbstractAreaBoxMapping
    {
        public virtual PremiumCRoute PremiumCRoute { get; set; }
    }
}
