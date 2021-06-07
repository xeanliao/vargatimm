// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PietschSoft.VE
{
    /// <summary>
    /// An enumeration of map styles.
    /// </summary>
    public enum MapStyle : int
    {
        /// <summary>
        /// The road map style
        /// </summary>
        [Description("r")]
        Road = 1,
        /// <summary>
        /// The aerial map style
        /// </summary>
        [Description("a")]
        Aerial = 2,
        /// <summary>
        /// The hybrid map style, which is an aerial map with a label overlay
        /// </summary>
        [Description("h")]
        Hybrid = 3,
        /// <summary>
        /// The bird's eye (oblique-angle) imagery map style
        /// </summary>
        [Description("o")]
        Birdseye = 4
    }
}
