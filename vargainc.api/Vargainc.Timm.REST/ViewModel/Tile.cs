using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class Tile
    {
        public string Layer { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ZIP { get; set; }
        public int? HOME_COUNT { get; set; }
        public int? BUSINESS_COUNT { get; set; }
        public int? APT_COUNT { get; set; }
        public bool IsMaster { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        //public DbGeometry Geom { get; set; }
        public SqlGeometry Geom { get; set; }
        public double[] Center { get; set; }
        
    }
}