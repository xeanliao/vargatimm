using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class ImportArea
    {
        public string Name { get; set; }
        public int? APT { get; set; }
        public int? HOME { get; set; }
        public int? Total { get; set; }
        public int? Penetration { get; set; }
    }
}