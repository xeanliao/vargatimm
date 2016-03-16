using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class ViewGTU
    {
        public int? Id { get; set; }
        public int? TaskId { get; set; }
        public string UserColor { get; set; }
        public int? CompanyId { get; set; }
        public string Company { get; set; }
        public int? AuditorId { get; set; }
        public string Auditor { get; set; }
        public Models.UserRoles? Role { get; set; }
    }
}