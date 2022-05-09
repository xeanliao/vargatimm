using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vargainc.Timm.Models
{
    public class TaskGtuInfoMappingHistory
    {
        public int? HistId { get; set; }
        public int? Id { get; set; }
        public int? TaskId { get; set; }
        public int? GTUId { get; set; }
        public string UserColor { get; set; }
        public DateTime? InsertedTime { get; set; }
        public virtual GTU GTU { get; set; }
        public virtual Task Task { get; set; }
        public virtual ICollection<GtuInfo> GtuInfos { get; set; }
        public int? UserId { get; set; }
        public User Auditor { get; set; }
    }
}
