using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vargainc.Timm.Models
{
    public class GTU
    {
        public int? Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Model { get; set; }
        public string UniqueID { get; set; }
        public string ShortUniqueID { get; set; }
        public int? UserId { get; set; }
        public int? BagId { get; set; }

        public GTUBag Bag { get; set; }
        public User User { get; set; }
    }
}
