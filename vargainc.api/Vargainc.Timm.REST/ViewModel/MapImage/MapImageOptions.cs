using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel.MapImage
{
    public class MapImageOptions
    {
        public string baseUrl { get; set; }
        public int? campaignId { get; set; }
        public int? submapId { get; set; }
        public int? dmapId { get; set; }
        public bool suppressNDAInCampaign { get; set; }
        public bool suppressGTU { get; set; }
        public bool suppressNDAInSubMap { get; set; }
        public bool suppressNDAInDMap { get; set; }
        public bool suppressLocations { get; set; }
        public bool suppressRadii { get; set; }
        public bool showPenetrationColors { get; set; }
        public List<int> penetrationColors { get; set; }
        public int zoom { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public MapTypeEnum? mapType { get;set; }
        public string type { get; set; }
        public double? topRightLat { get; set; }
        public double? topRightLng { get; set; }
        public double? bottomLeftLat { get; set; }
        public double? bottomLeftLng { get; set; }
    }
}