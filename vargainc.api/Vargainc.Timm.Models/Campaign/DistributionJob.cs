using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vargainc.Timm.Models
{
    public class DistributionJob
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? CampaignId { get; set; }
        public virtual Campaign Campaign { get; set; }
    }
}
