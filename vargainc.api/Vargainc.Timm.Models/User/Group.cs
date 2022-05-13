using System.Collections.Generic;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class Group
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
