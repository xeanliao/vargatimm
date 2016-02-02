using Newtonsoft.Json;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class Location
    {
        [DefaultValue(0)]
        [JsonIgnore]
        public long? Id { get; set; }

        [JsonProperty("lat")]
        [DefaultValue(0.0)]
        public double? Latitude { get; set; }

        [JsonProperty("lng")]
        [DefaultValue(0.0)]
        public double? Longitude { get; set; }
    }
}
