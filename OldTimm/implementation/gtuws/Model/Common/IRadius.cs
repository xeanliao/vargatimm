using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.ModelLayer.Common
{
    public interface IRadius
    {
        Int32 Id
        {
            get;
            set;
        }

        Double Length
        {
            get;
            set;
        }

        //Int32 LengthMeasuresId
        //{
        //    get;
        //    set;
        //}

        //Int32 AddressId
        //{
        //    get;
        //    set;
        //}

        //Boolean IsDisplay
        //{
        //    get;
        //    set;
        //}
    }
}
