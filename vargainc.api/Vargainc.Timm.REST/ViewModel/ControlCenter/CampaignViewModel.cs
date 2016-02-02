using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel.ControlCenter
{
    public class CampaignViewModel
    {
        public int? Id { get; set; }
        public string AreaDescription { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ContactName { get; set; }
        public string CreatorName { get; set; }
        public string CustemerName { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int? Sequence { get; set; }
        public string UserName { get; set; }
    }
}