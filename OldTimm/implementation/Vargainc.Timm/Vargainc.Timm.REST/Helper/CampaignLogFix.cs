using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.REST.Helper
{
    public static class CampaignLogoFix
    {
        public static string LogoPath(string logo)
        {
            string basePath = ConfigurationManager.AppSettings["campaignimagepath"];
            basePath = basePath.TrimEnd('/');
            if (!string.IsNullOrWhiteSpace(logo))
            {
                return string.Format("{0}/{1}", basePath, logo);
            }
            return null;

        }
    }
}