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
using GPS.DomainLayer.QuerySpecifications;
using GPS.DomainLayer.Enum;
using NetTopologySuite.Geometries;
using GPS.DomainLayer.Campaign;

namespace GPS.Website.Handler
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class CampaignMap : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            int id = Convert.ToInt32(context.Request.QueryString["cid"]);
            context.Response.Write(GetCampaignById(id));
        }

        public string GetCampaignById(int id)
        {
            ToCampaign toCampaign = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(id);
                CampaignManager.CalculateHolesInSubMap(campaign);
                toCampaign = AssemblerConfig.GetAssembler<ToCampaign, Campaign>().AssembleFrom(campaign);
            }

            string str = JsonConvert.SerializeObject(toCampaign);

            return str;
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
