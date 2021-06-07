using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.ModelLayer.Common
{
    public interface ICoordinate
    {
        Double Latitude { get; set; }

        Double Longitude { get; set; }
    }
}
