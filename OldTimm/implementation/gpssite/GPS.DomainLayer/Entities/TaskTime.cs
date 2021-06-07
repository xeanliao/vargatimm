using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using GPS.DataLayer.ValueObjects;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class TaskTime
    {
        public TaskTime() { }
        public virtual int Id { get; set; }
        public virtual int TaskId { get; set; }
        public virtual DateTime Time { get; set; }
        public virtual int TimeType { get; set; }
    }
}
