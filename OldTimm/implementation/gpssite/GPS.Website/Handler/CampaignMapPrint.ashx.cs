using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using GPS.Website.TransferObjects;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.Website.AppFacilities;
using Newtonsoft.Json;
using log4net;

namespace GPS.Website.Handler
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class CampaignMapPrint : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            int id = Convert.ToInt32(context.Request.QueryString["cid"]);
            context.Response.Write(GetPrintCampaign(id));
        }

        public string GetPrintCampaign(int Id)
        {
            try
            {
                ToPrintCampaign printCampaign = null;
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(Id);

                    if (campaign != null)
                    {
                        printCampaign = AssemblerConfig.GetAssembler<ToPrintCampaign, Campaign>().AssembleFrom(campaign);
                    }

                }                

                string str = JsonConvert.SerializeObject(printCampaign);
                return str;
            }
            catch (Exception ex)
            {
                ILog logger = LogManager.GetLogger(GetType());
                logger.Error("WCF Unhandle Error", ex);
                return null;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


    }
}
