using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using GPS.DataLayer.ValueObjects;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class Privilege
    {
        public Privilege() { }
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Value { get; set; }
        public virtual IList<Group> Groups { get; set; }
    }
}
