using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class CustomGTUPoint
    {
        public int? GtuId { get; set; }
        public int? TaskId { get; set; }
        public DateTime? Date { get; set;}
        public ViewModel.Location Location { get; set; }
           
    }
}