using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.REST.ViewModel
{
    public class ViewEmplyee
    {
        public int? Id { get; set; }
        public int? CompanyId { get; set; }
        public UserRoles? Role { get; set; }
        public string FullName { get; set; }
        public string CellPhone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Notes { get; set; }
    }
}