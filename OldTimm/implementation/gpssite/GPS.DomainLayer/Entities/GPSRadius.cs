using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;
using System.Runtime.Serialization;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    class GPSRadius : IRadius
    {
        #region IRadius Members

        public int Id
        {
            get;
            set;
        }

        public double Length
        {
            get;
            set;
        }

        public int LengthMeasuresId
        {
            get;
            set;
        }

        public int AddressId
        {
            get;
            set;
        }

        public bool IsDisplay
        {
            get;
            set;
        }

        #endregion
    }
}
