using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.ActiveRecordDatabase;
using Castle.ActiveRecord;

namespace GPS.DomainLayer.Entities
{
    [ActiveRecord("ScheduleTask")]
    public class ScheduleTask : AssistantDatabase
    {
        [PrimaryKey(PrimaryKeyType.Guid)]
        public Guid? Id { get; set; }
        [Property]
        public DateTime? StartDate { get; set; }
        [Property]
        public DateTime? EndDate { get; set; }
        [Property]
        public int? InUser { get; set; }
        [Property]
        public string Status { get; set; }
        [Property]
        public string AdditionalInfo { get; set; }

        public static ScheduleTask Find(Guid id)
        {
            return ScheduleTask.FindByPrimaryKey(typeof(ScheduleTask), id) as ScheduleTask;
        }
    }
}
