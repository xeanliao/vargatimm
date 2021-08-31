using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Vargainc.Timm.EF;

namespace Vargainc.Timm.REST
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(WebApiApplication));
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                logger.Error(exception);
            }
        }
    }
}
