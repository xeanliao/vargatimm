using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GPS.DomainLayer.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IGPSColor
    {
        [JsonProperty]
        int R { get; set; }

        [JsonProperty]
        int G { get; set; }

        [JsonProperty]
        int B { get; set; }

        [JsonProperty]
        double A { get; set; }
    }
}
