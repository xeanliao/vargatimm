using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using GPS.DataLayer.ValueObjects;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class Group
    {
        public Group() { }
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<User> Users { get; set; }
        public virtual IList<Privilege> Privileges { get; set; }
    }
}
