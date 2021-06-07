using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace GPS.Website.DAL
{
    public class ConfigUtils
    {
        public static string GetConfiguration(string sKey)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[sKey] == null)
            {
                return "";
            }
            return System.Configuration.ConfigurationManager.AppSettings[sKey];
        }
    }
}