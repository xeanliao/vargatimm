using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using GTU.Utilities.Logging;

namespace GTU.DataLayer
{
    public class ConnectionString
    {
        /// <summary>
        /// Gets the connection string for the GPSTracking DB
        /// </summary>
        /// <value>The connect string.</value>
        public static string GPSTrackingConnectionString
        {
            get
            {
                try
                {
                    String aa = ConfigurationManager.ConnectionStrings["GTU.DataLayer.Properties.Settings.GTUTrackingConnectionString1"].ConnectionString;
                    //return ConfigurationManager.ConnectionStrings["GTUTrackingConnectionString1"].ConnectionString;
                    return aa;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                
                
            }
        }
    }
}
