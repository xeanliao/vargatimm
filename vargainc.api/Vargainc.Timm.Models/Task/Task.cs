using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class Task
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        [DefaultValue(0)]
        public int? Status { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        [DefaultValue(0)]
        public int DistributionMapId { get; set; }
        
        [DefaultValue(0)]
        public int? AuditorId { get; set; }
        public virtual User Auditor { get; set; }

        public virtual DistributionMap DistributionMap { get; set; }
        public virtual ICollection<TaskGtuInfoMapping> TaskGtuInfoMappings { get; set; }

        //database datetime type is datetime2(0) have some problem with entity framework
        public virtual ICollection<TaskTime> TaskTimes { get; set; }
    }
}
