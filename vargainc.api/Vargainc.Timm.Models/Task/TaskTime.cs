using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vargainc.Timm.Models
{
    public class TaskTime
    {
        public int? Id { get; set; }
        public int TaskId { get; set; }
        public DateTime? Time { get; set; }
        public int TimeType { get; set; }

        public virtual Task Task { get; set; }
    }
}
