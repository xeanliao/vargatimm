using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vargainc.Timm.Models
{
    public class NdArea
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public bool? IsEnabled { get; set; }
        public int? Total { get; set; }
        public string Description { get; set; }
        public DbGeometry Polygon { get; set; }
    }
}
