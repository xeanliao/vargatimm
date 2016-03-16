using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vargainc.Timm.Models
{
    public class GTUBag
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }

        public virtual ICollection<GTU> GTU { get; set; }
        public User Aditor { get; set; }
    }
}
