using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Interfaces {

    public interface IRadius {
        Int32 Id {
            get;
            set;
        }

        Double Length {
            get;
            set;
        }

        Int32 LengthMeasuresId {
            get;
            set;
        }

        Int32 AddressId {
            get;
        }

        Boolean IsDisplay {
            get;
            set;
        }
    }
}
