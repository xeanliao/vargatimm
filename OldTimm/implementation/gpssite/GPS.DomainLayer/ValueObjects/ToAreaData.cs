using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer.ValueObjects
{
    [Serializable]
    public class ToAreaData
    {
        #region Interfaces
        public Int32 Id;
        public Int32 Total;
        public Int32 Count;
        public Double Penetration;
        public IEnumerable<Int32> BoxIds;
        public IEnumerable<ToBlockGroupSelectMapping> BlockGroupSelectMappings;
        public IEnumerable<ToPremiumCRouteSelectMapping> PremiumCRouteSelectMappings;
        #endregion
    }
}
