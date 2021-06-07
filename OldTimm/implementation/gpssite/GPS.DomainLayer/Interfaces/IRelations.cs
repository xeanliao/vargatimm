using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;
using Newtonsoft.Json;

namespace GPS.DomainLayer.Interfaces
{    
    [JsonObject(MemberSerialization.OptIn)]
    public interface IRelations
    {
        [JsonProperty]
        Classifications Classification { get; set; }

        [JsonProperty]
        int CurrentAreaId { get; set; }

        [JsonProperty]
        bool IsExistsMediumCircle { get; set; }

        [JsonProperty]
        bool IsExistsLeastCircle { get; set; }

        [JsonProperty]
        List<List<string>> Relation { get; }

        [JsonProperty]
        string ClientAreaId { get;}
    }
}
