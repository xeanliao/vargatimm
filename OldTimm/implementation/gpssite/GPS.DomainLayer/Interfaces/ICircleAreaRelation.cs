using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using GPS.DomainLayer.Enum;

namespace GPS.DomainLayer.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface ICircleAreaRelation
    {
        [JsonProperty]
        string Id { get; set; }

        [JsonProperty]
        int AddressId { get; set; }

        [JsonProperty]
        Classifications classifiction { get; set; }

        [JsonProperty]
        ICircle Circle { get; set; }

        [JsonProperty]
        List<IRelations> Relations { get; set; }
    }
}
