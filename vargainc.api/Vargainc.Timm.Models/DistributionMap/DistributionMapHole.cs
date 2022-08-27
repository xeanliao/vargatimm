using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vargainc.Timm.Models
{
    public class DistributionMapHole
    {
        public int? Id { get; set; }
        public int? DistributionMapId { get; set; }
        public int? AreaId { get; set; }
        public string Code { get; set; }
        public int? Apt { get; set; }
        public int? Home { get; set; }
    }
}
