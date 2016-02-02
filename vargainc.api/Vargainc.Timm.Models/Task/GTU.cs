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
        public int? UserId { get; set; }
    }
}
