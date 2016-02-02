using System.Collections.Generic;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class Group
    {
        public Group()
        {
            //Users = new List<User>();
        }
        [DefaultValue(0)]
        public int? Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
