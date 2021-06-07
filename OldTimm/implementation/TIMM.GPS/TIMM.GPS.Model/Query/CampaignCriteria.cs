using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TIMM.GPS.Model
{
    public class CampaignCriteria : Criteria
    {
        public string Name { get; set; }
        public int status { get; set; }
    }
}
