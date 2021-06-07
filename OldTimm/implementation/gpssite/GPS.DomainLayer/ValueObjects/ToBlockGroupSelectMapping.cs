using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DataLayer.ValueObjects
{
    public class ToBlockGroupSelectMapping
    {
        public ToBlockGroupSelectMapping(Int32 threeZipId, Int32 fiveZipId, Int32 tractId, Int32 blockGroupId)
        {
            ThreeZipId = threeZipId;
            FiveZipId = fiveZipId;
            TractId = tractId;
            BlockGroupId = blockGroupId;
        }

        #region Interfaces
        public Int32 ThreeZipId;
        public Int32 FiveZipId;
        public Int32 TractId;
        public Int32 BlockGroupId;
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (obj.GetType() != GetType()) return false;

            ToBlockGroupSelectMapping m = obj as ToBlockGroupSelectMapping;

            return ThreeZipId.Equals(m.ThreeZipId)
                && FiveZipId.Equals(m.FiveZipId)
                && TractId.Equals(m.TractId)
                && BlockGroupId.Equals(m.BlockGroupId);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
