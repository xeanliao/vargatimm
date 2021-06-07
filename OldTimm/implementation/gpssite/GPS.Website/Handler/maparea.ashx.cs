using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using Newtonsoft.Json;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.Area;
using GPS.DomainLayer.Campaign;
using GPS.DomainLayer.Interfaces;
using log4net;

namespace GPS.Website.Handler {
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class maparea : IHttpHandler, IRequiresSessionState {
        public void ProcessRequest(HttpContext context) {
            ProcessRequestImpl(context);
        }

        public bool IsReusable {
            get {
                return true;
            }
        }

        /// <summary>
        /// Process the http request.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        private void ProcessRequestImpl(HttpContext context) {
            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            Classifications classification = Classifications.Z3;
            int boxId = 0;
            try
            {
                classification = (Classifications)int.Parse(context.Request.QueryString["classification"]);
                boxId = int.Parse(context.Request.QueryString["box"]);
            }
            catch (Exception ex)
            {
                ILog logger = LogManager.GetLogger(GetType());
                logger.Error("HttpHandler Unhandle Error", ex);
            }
            if (boxId > 0) {
                context.Response.Write(GetJsonString(classification, boxId, context));
            }
        }

        /// <summary>
        /// Get json string from classification
        /// </summary>
        /// <param name="classification"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetJsonString(Classifications classification, int boxId, HttpContext context) {
            string jsonString = String.Empty;
            int campaignId = 0;
            if (context.Request.QueryString["campaign"] != null) {
                campaignId = int.Parse(context.Request.QueryString["campaign"]);
                jsonString = MapAreaManager.GetJsonString(classification, boxId, campaignId);
            } else {
                jsonString = MapAreaManager.GetJsonString(classification, boxId);
            }

            return jsonString;
        }
    }
}
