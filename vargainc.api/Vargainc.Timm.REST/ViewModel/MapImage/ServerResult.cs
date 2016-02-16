using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel.MapImage
{
    public class MapServerResult
    {
        public bool success { get; set; }
        public string tiles { get; set; }
        public string geometry { get; set; }
    }
}