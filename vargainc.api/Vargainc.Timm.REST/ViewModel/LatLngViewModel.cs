using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Vargainc.Timm.REST.ViewModel
{
    public class LatLngViewModel
    {
        public LatLngViewModel()
        {

        }
        public LatLngViewModel(double? lat, double? lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
        }

        [JsonProperty("lat")]
        public double? Latitude { get; set; }

        [JsonProperty("lng")]
        public double? Longitude { get; set; }
    }
}