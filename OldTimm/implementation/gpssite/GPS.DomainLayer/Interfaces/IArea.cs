using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace GPS.DomainLayer.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IArea
    {
        [JsonProperty]
        int Id { get; set; }

        [JsonProperty]
        string Name { get; set; }

        [JsonProperty]
        Classifications Classification { get; set; }

        [JsonProperty]
        string State { get; set; }

        [JsonProperty]
        Dictionary<string, string> Attributes { get; set; }

        [JsonProperty]
        List<ICoordinate> Locations { get; set; }

        [JsonProperty]
        IGPSColor FillColor { get; set; }

        [JsonProperty]
        IGPSColor LineColor { get; set; }

        [JsonProperty]
        Dictionary<int, Dictionary<int, bool>> Relations { get; set; }

        double Latitude { get; set; }
        double Longitude { get; set; }

        [JsonProperty]
        bool IsEnabled { get; set; }

        [JsonProperty]
        string Description { get; set; }
    }
}
