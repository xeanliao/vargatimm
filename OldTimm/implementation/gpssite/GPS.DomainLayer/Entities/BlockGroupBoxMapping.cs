using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class BlockGroupBoxMapping : AbstractAreaBoxMapping
    {
        public virtual BlockGroup BlockGroup { get; set; }
    }
}
