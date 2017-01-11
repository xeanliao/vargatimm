using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Vargainc.Timm.REST.ViewModel
{
    public class NDAddressViewModel
    {
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("zip")]
        public string ZipCode { get; set;}
        [JsonProperty("center")]
        public LatLngViewModel Center { get; set; }
        [JsonProperty("boundary")]
        public List<LatLngViewModel> Boundary { get; set; }
    }
}