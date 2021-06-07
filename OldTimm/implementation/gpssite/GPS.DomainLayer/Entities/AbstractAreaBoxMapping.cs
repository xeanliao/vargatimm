using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public abstract class AbstractAreaBoxMapping
    {
        public virtual int Id { get; set; }
        public virtual int BoxId { get; set; }
    }
}
