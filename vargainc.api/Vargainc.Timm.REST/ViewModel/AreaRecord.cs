using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class AreaRecord
    {
        public Classifications Classification { get;set;  }
        public string Name { get; set;  }
        public int? Id { get; set; }
        public bool? Value { get; set; }
    }
}