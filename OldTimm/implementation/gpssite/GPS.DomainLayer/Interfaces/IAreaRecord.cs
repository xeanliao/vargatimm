using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;

namespace GPS.DomainLayer.Interfaces
{
    public interface IAreaRecord
    {
        Classifications Classification { get; set; }
        string ClientAreaId { get; set; }
        int AreaId { get; set; }
        bool Value { get; set; }
        List<List<string>> Relation { get; set; }
    }
}
