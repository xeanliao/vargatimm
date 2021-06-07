using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Enum
{
    /// <summary>
    /// http://www.census.gov/geo/www/cob/bdy_files.html
    /// </summary>
    public enum Classifications
    {
        /// <summary>
        /// 3-Digit ZIP Code Tabulation Areas
        /// </summary>
        Z3 = 0,
        /// <summary>
        /// 5-Digit ZIP Code Tabulation Areas
        /// </summary>
        Z5 = 1,
        /// <summary>
        /// TRK
        /// </summary>
        TRK = 2,
        /// <summary>
        /// Census Block Groups
        /// </summary>
        BG = 3,
        /// <summary>
        /// Metropolitan and Micropolitan Statistical Areas
        /// </summary>
        CBSA = 4,
        /// <summary>
        /// Urban Areas
        /// </summary>
        Urban = 5,
        /// <summary>
        /// County and County Equivalent Areas
        /// </summary>
        County = 6,
        /// <summary>
        /// State Legislative Districts - Upper/Senate
        /// </summary>
        SLD_Senate = 7,
        /// <summary>
        /// State Legislative Districts - Lower/House
        /// </summary>
        SLD_House = 8,
        /// <summary>
        /// Voting Districts
        /// </summary>
        Voting_District = 9,
        /// <summary>
        /// School Districts – Elementary
        /// </summary>
        SD_Elem = 10,
        /// <summary>
        /// School Districts – Secondary
        /// </summary>
        SD_Secondary = 11,
        /// <summary>
        /// School Districts – Unified
        /// </summary>
        SD_Unified = 12,
        /// <summary>
        /// Custom Area
        /// </summary>
        Custom = 13,
        /// <summary>
        /// Non-Deliverable Address
        /// </summary>
        Address = 14,
        /// <summary>
        /// Premium CRoute
        /// </summary>
        PremiumCRoute = 15
    }
}
