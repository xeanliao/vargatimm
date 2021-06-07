using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GPS.DomainLayer.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IBoxArea
    {
        [JsonProperty]
        double Latitude { get; }
        [JsonProperty]
        double Longitude { get; }
        [JsonProperty]
        double MinLongitude { get; }
        [JsonProperty]
        double MaxLongitude { get; }
        [JsonProperty]
        double MinLatitude { get; }
        [JsonProperty]
        double MaxLatitude { get; }
    }
}
