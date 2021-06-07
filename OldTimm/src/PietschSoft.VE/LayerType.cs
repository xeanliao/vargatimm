// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

namespace PietschSoft.VE
{
    /// <summary>
    /// An enumeration of layer types.
    /// </summary>
    public enum LayerType
    {
        /// <summary>
        /// The layer data is specified by a GeoRSS XML file.
        /// </summary>
        GeoRSS,
        /// <summary>
        /// The layer data is specified in an existing Live Maps (http://maps.live.com) collection file.
        /// </summary>
        VECollection,
        /// <summary>
        /// The layer comprises a custom tile set.
        /// </summary>
        TileSource
    }
}
