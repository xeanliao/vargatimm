using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GPS.DomainLayer.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface ICircle
    {
        [JsonProperty]
        ICoordinate Center { get; set; }

        [JsonProperty]
        Double Radius { get; set; }

        [JsonIgnore]
        List<ICoordinate> Coordinates { get; set; }

        void CalculateCoordinates();
    }
}
