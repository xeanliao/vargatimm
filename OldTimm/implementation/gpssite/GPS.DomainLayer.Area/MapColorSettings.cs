using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace GPS.DomainLayer.Area {
    /// <summary>
    /// Class MapColorSettings is responsible for reading the application's
    /// color settings.
    /// </summary>
    class MapColorSettings {
        /// <summary>
        /// 3-Digit ZIP Code Tabulation Areas
        /// </summary>
        public static string Z3FillColor {
            get {
                return ConfigurationSettings.AppSettings["Z3_fillColor"];
            }
        }

        /// <summary>
        /// 3-Digit ZIP Code Tabulation Areas
        /// </summary>
        public static string Z3OutlineColor {
            get {
                return ConfigurationSettings.AppSettings["Z3_outlineColor"];
            }
        }

        /// <summary>
        /// 5-Digit ZIP Code Tabulation Areas
        /// </summary>
        public static string Z5FillColor {
            get {
                return ConfigurationSettings.AppSettings["Z5_fillColor"];
            }
        }

        /// <summary>
        /// 5-Digit ZIP Code Tabulation Areas
        /// </summary>
        public static string Z5OutlineColor {
            get {
                return ConfigurationSettings.AppSettings["Z5_outlineColor"];
            }
        }

        /// <summary>
        /// TRK
        /// </summary>
        public static string TractFillColor {
            get {
                return ConfigurationSettings.AppSettings["TRK_fillColor"];
            }
        }

        /// <summary>
        /// TRK
        /// </summary>
        public static string TractOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["TRK_outlineColor"];
            }
        }

        /// <summary>
        /// Census Block Groups
        /// </summary>
        public static string BlockGroupFillColor {
            get {
                return ConfigurationSettings.AppSettings["BG_fillColor"];
            }
        }

        /// <summary>
        /// Census Block Groups
        /// </summary>
        public static string BlockGroupOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["BG_outlineColor"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string CbsaFillColor {
            get {
                return ConfigurationSettings.AppSettings["CBSA_fillColor"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string CbsaOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["CBSA_outlineColor"];
            }
        }

        /// <summary>
        /// Urban Areas
        /// </summary>
        public static string UrbanFillColor {
            get {
                return ConfigurationSettings.AppSettings["Urban_fillColor"];
            }
        }

        /// <summary>
        /// Urban Areas
        /// </summary>
        public static string UrbanOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["Urban_outlineColor"];
            }
        }

        /// <summary>
        /// County and County Equivalent Areas
        /// </summary>
        public static string CountyFillColor {
            get {
                return ConfigurationSettings.AppSettings["County_fillColor"];
            }
        }

        /// <summary>
        /// County and County Equivalent Areas
        /// </summary>
        public static string CountyOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["County_outlineColor"];
            }
        }

        /// <summary>
        /// State Legislative Districts - Upper/Senate
        /// </summary>
        public static string SldSenateFillColor {
            get {
                return ConfigurationSettings.AppSettings["SLD_Senate_fillColor"];
            }
        }

        /// <summary>
        /// State Legislative Districts - Upper/Senate
        /// </summary>
        public static string SldSenateOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["SLD_Senate_outlineColor"];
            }
        }

        /// <summary>
        /// State Legislative Districts - Lower/House
        /// </summary>
        public static string SldHouseFillColor {
            get {
                return ConfigurationSettings.AppSettings["SLD_House_fillColor"];
            }
        }

        /// <summary>
        /// State Legislative Districts - Lower/House
        /// </summary>
        public static string SldHouseOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["SLD_House_outlineColor"];
            }
        }

        /// <summary>
        /// Voting Districts
        /// </summary>
        public static string VotingDistrictFillColor {
            get {
                return ConfigurationSettings.AppSettings["Voting_District_fillColor"];
            }
        }

        /// <summary>
        /// Voting Districts
        /// </summary>
        public static string VotingDistrictOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["Voting_District_outlineColor"];
            }
        }

        /// <summary>
        /// School Districts – Elementary
        /// </summary>
        public static string SdElemFillColor {
            get {
                return ConfigurationSettings.AppSettings["SD_Elem_fillColor"];
            }
        }

        /// <summary>
        /// School Districts – Elementary
        /// </summary>
        public static string SdElemOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["SD_Elem_outlineColor"];
            }
        }

        /// <summary>
        /// School Districts – Secondary
        /// </summary>
        public static string SdSecondaryFillColor {
            get {
                return ConfigurationSettings.AppSettings["SD_Secondary_fillColor"];
            }
        }

        /// <summary>
        /// School Districts – Secondary
        /// </summary>
        public static string SdSecondaryOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["SD_Secondary_outlineColor"];
            }
        }

        /// <summary>
        /// School Districts – Unified
        /// </summary>
        public static string SdUnifiedFillColor {
            get {
                return ConfigurationSettings.AppSettings["SD_Unified_fillColor"];
            }
        }

        /// <summary>
        /// School Districts – Unified
        /// </summary>
        public static string SdUnifiedOutlineColor {
            get {
                return ConfigurationSettings.AppSettings["SD_Unified_outlineColor"];
            }
        }
    }
}
