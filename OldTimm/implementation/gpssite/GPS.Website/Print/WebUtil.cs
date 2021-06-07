using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPS.Website.Print
{
    public class WebUtil
    {
        public static string GetAbsoluteUrl(System.Web.HttpRequest request, string sFile)
        {
            string sUrl = request.Url.ToString();
            if (sFile.StartsWith("/") == false)
                sFile = "/" + sFile;

            string sCurrentPage = request.Path.ToLower();
            if(request.ApplicationPath != "/")
                sCurrentPage = sCurrentPage.Replace(request.ApplicationPath.ToLower(), "");
            string sNewUrl = sUrl.ToLower().Replace(sCurrentPage, sFile);

            if (sFile.ToLower().IndexOf("footer.aspx") > 0)
            {
                GPS.Utilities.DBLog.LogInfo(request.Path);
            }

            return sNewUrl;
        }

    }
}