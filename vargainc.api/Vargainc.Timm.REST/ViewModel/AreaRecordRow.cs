using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class AreaRecordRow
    {
        public int? Id { get; set; }
        public Coordinate Coordinate { get;set; }
    }
}