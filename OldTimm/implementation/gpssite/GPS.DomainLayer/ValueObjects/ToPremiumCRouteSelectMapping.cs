using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DataLayer.ValueObjects
{
    public class ToPremiumCRouteSelectMapping
    {
        public Int32 ThreeZipId;
        public Int32 FiveZipId;
        public Int32 PremiumCRouteId;

        public ToPremiumCRouteSelectMapping(Int32 threeZipId, Int32 fiveZipId, Int32 premiumCRouteId)
        {
            ThreeZipId = threeZipId;
            FiveZipId = fiveZipId;
            PremiumCRouteId = premiumCRouteId;
        }

        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (obj.GetType() != GetType()) return false;

            ToPremiumCRouteSelectMapping m = obj as ToPremiumCRouteSelectMapping;

            return ThreeZipId.Equals(m.ThreeZipId)
                && FiveZipId.Equals(m.FiveZipId)
                && PremiumCRouteId.Equals(m.PremiumCRouteId);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
