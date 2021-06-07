using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class Location
    {
        public Location()
        {

        }
        public Location(double? lat, double? lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
        }

        [JsonIgnore]
        public long? Id { get; set; }

        [JsonProperty("lat")]
        public double? Latitude { get; set; }

        [JsonProperty("lng")]
        public double? Longitude { get; set; }
    }
}