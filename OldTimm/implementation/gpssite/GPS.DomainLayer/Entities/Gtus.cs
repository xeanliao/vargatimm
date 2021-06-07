using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class Gtu
    {
        public Gtu() { }
        public virtual int Id { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual string Model { get; set; }
        public virtual string UniqueID { get; set; }
        public virtual User User { get; set; }
        //public virtual IList<Taskgtuinfomapping> Taskgtuinfomappings { get; set; }
    }
}
