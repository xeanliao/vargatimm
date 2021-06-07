// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

namespace PietschSoft.VE
{
    /// <summary>
    /// This enumeration is used in the route-type parameter of the VEMap.GetRoute method to specify the type of route to generate.
    /// </summary>
    public enum RouteType
    {
        /// <summary>
        /// Generates the shortest (by distance) route
        /// </summary>
        Shortest,
        /// <summary>
        /// Generates the shortest (by estimated driving time) route
        /// </summary>
        Quickest
    }
}
