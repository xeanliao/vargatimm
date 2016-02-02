using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel.ControlCenter
{
    public class TaskViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public int? Status { get; set; }
        /// <summary>
        /// Database Telephone = Telephone @ Operator
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// Database Telephone = Telephone @ Operator
        /// </summary>
        public string Operator { get; set; }
        public string Email { get; set; }
        public int? AuditorId { get; set; }
    }
}