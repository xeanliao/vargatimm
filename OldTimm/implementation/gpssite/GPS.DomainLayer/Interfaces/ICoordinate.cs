using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GPS.DomainLayer.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface ICoordinate
    {
        [JsonProperty]
        double Latitude { get; set; }

        [JsonProperty]
        double Longitude { get; set; }

        [JsonProperty]
        int ShapeId { get; }
    }
}
