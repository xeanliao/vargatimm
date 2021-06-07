using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Track;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Track
{
    public static class TrackFactory
    {
        ////private static StayFilter stayFilter;
        ////private static UnvalidAddressFilter unvalidAddressFilter;
        //private static UnvalidAreaFilter unvalidAreaFilter;

        ///// <summary>
        ///// Instance StayFilter object
        ///// </summary>
        ///// <param name="stayPeriod">required</param>
        ///// <returns>StayFilter object</returns>
        //public static StayFilter StayFilterInstance(int stayPeriod)
        //{

        //    //if (null == stayFilter)
        //    //    stayFilter = new StayFilter();
        //    //stayFilter.Reset();
        //    //stayFilter.StayPeriod = stayPeriod;
        //    return stayFilter;
        //}

        ///// <summary>
        ///// Instance UnvalidAddressFilter object
        ///// </summary>
        ///// <param name="coordinate">Required</param>
        ///// <param name="radius">Required</param>
        ///// <returns>UnvalidAddressFilter object</returns>
        //public static UnvalidAddressFilter UnvalidAddressFilterInstance(ICoordinate coordinate, IRadius radius)
        //{
        //    //if (null == unvalidAddressFilter)
        //    //    unvalidAddressFilter = new UnvalidAddressFilter();
        //    //unvalidAddressFilter.Reset();
        //    //unvalidAddressFilter.AddressCoordinate = coordinate;
        //    //unvalidAddressFilter.Radius = radius;
        //    return unvalidAddressFilter;
        //}

        ///// <summary>
        ///// Instance UnvalidAreaFilter object
        ///// </summary>
        ///// <param name="coordinates">Required</param>
        ///// <returns>UnvalidAreaFilter object</returns>
        //public static UnvalidAreaFilter UnvalidAreaFilterInstance(List<ICoordinate> coordinates)
        //{
        //    //if (null == unvalidAreaFilter)
        //    //    unvalidAreaFilter = new UnvalidAreaFilter();
        //    //unvalidAreaFilter.Reset();
        //    //unvalidAreaFilter.ValidCoordinates = coordinates;
        //    return unvalidAreaFilter;
        //}
    }
}
