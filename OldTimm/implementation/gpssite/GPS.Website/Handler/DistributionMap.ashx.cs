using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using GPS.Website.TransferObjects;
using GPS.DomainLayer.Entities;
using GPS.DataLayer;
using GPS.Website.AppFacilities;
using Newtonsoft.Json;
using GPS.DomainLayer.Campaign;
using GPS.Website.DistributionMapServices;

namespace GPS.Website.Handler
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DistributionMap : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            int id = Convert.ToInt32(context.Request.QueryString["cid"]);
            context.Response.Write(GetCampaignByIdForDM(id));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


     


        public string GetCampaignByIdForDM(int id)
        {
            ToCampaign toCampaign = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(id);
                campaign.Users = null;
                if (campaign.SubMaps != null)
                {
                    foreach (SubMap sm in campaign.SubMaps)
                    {
                        if (sm.DistributionMaps != null)
                        {
                            foreach (GPS.DomainLayer.Entities.DistributionMap dm in sm.DistributionMaps)
                            {
                                dm.Tasks = null;
                            }

                        }
                    }
                }
                CampaignManager.CalculateHolesInDMap(campaign);
                toCampaign = AssemblerConfig.GetAssembler<ToCampaign, Campaign>().AssembleFrom(campaign);
                toCampaign.NotIncludeInSubMapArea = DMWriterService.CalcDmapNoIncludeArea(campaign.Id);
            }

            //return toCampaign;
            string str = JsonConvert.SerializeObject(toCampaign);           

            return str;
        }
    }
}
