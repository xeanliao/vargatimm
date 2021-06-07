using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using log4net;
using Castle.Core.Configuration;
using Castle.ActiveRecord.Framework.Config;
using Castle.ActiveRecord;
using GPS.DomainLayer.Entities;
using Castle.ActiveRecord.Framework;
using System.Configuration;
using GPS.DomainLayer.ActiveRecordDatabase;

namespace GPS.Website
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //initialize log4net
            log4net.Config.XmlConfigurator.Configure();

            //initialize active record 
            //here only initialize ScheduleTask entity. others is still use NHibernate 
            //if more entity use another database , must add initialize here.
            var config = ActiveRecordSectionHandler.Instance;
            ActiveRecordStarter.Initialize(config, typeof(ScheduleTask), typeof(AssistantDatabase));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var logger = LogManager.GetLogger(GetType());
            logger.Error("Web Application Unhandle Error.", HttpContext.Current.Server.GetLastError());
            HttpContext.Current.Server.ClearError();
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}