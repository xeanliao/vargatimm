using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WHYTAlgorithmService.Geo
{
    public interface IDNDAreaDao
    {
        Dictionary<int, List<Coordinate>> AvailableDNDArea();
    }
}
