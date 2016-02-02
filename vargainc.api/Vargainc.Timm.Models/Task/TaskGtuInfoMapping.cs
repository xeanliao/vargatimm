using System.Collections.Generic;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class TaskGtuInfoMapping
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0)]
        public int? TaskId { get; set; }
        [DefaultValue(0)]
        public int? GTUId { get; set; }
        public virtual GTU GTU { get; set; }
        public string UserColor { get; set; }
        public virtual Task Task { get; set; }
        public virtual ICollection<GtuInfo> GtuInfos { get; set; }
        
    }
}
