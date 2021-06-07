using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Web.SessionState;
using System.Collections.Generic;
using GPS.DomainLayer.Security;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;

namespace GPS.Website {
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class LoginHandler : IHttpHandler, IRequiresSessionState {
        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            context.Response.Clear();
            context.Response.Write("error:You need clear your brower cache to login. this method is no longer support!");
            context.Response.Flush();
        }

        public bool IsReusable {
            get {
                return true;
            }
        }
    }
}
