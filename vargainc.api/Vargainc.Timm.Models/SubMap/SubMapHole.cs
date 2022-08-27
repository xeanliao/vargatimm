using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vargainc.Timm.Models
{
    public class SubMapHole
    {
        public int? Id { get; set; }
        public int? SubmapId { get; set; }
        public int? AreaId { get; set; }
        public string Code { get; set; }
        public int? Apt { get; set; }
        public int? Home { get; set; }
    }
}
