// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

namespace PietschSoft.VE
{
    /// <summary>
    /// This enumeration is used in the units parameter of the VEMap.GetRoute method to specify the distance units to use when generating the route.
    /// This enumeration is also used as the VERouteItinerary.DistanceUnit property.
    /// </summary>
    public enum DistanceUnit
    {
        /// <summary>
        /// Generates route information in miles
        /// </summary>
        Miles,
        /// <summary>
        /// Generates route information in kilometers
        /// </summary>
        Kilometers
    }
}
