using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GPS.Website.Handler
{
    /// <summary>
    /// Summary description for PhantomjsPrintHandler
    /// </summary>
    public class PhantomjsPrintHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            var campaignId = context.Request["campaignId"];
            var dmapId = context.Request["dmapId"];
            var printType = context.Request["type"];
            var phantomjsUrl = ConfigurationManager.AppSettings["PhantomjsSite"];
            var campaignPrintUrl = ConfigurationManager.AppSettings["CampaignPrintUrl"];
            var distributionPrintUrl = ConfigurationManager.AppSettings["DistributionPrintUrl"];
            var reportPrintUrl = ConfigurationManager.AppSettings["ReportPrintUrl"];

            var url = string.Empty;
            switch (printType)
            {
                case "campaign":
                    if (string.IsNullOrWhiteSpace(campaignPrintUrl))
                    {
                        url = string.Format("{0}/print.html?campaign={1}&type=campaign",
                            phantomjsUrl,
                            campaignId);
                    }
                    else
                    {
                        url = string.Format(campaignPrintUrl, campaignId);
                    }
                    break;
                case "dmap":
                    if (string.IsNullOrWhiteSpace(distributionPrintUrl))
                    {
                        url = string.Format("{0}/print.html?campaign={1}&type=dmap",
                            phantomjsUrl,
                            campaignId);
                    }
                    else
                    {
                        url = string.Format(distributionPrintUrl, campaignId);
                    }
                    break;
                default:
                    if (string.IsNullOrWhiteSpace(reportPrintUrl))
                    {
                        url = string.Format("{0}/print.html?campaign={1}&type=report",
                            phantomjsUrl,
                            campaignId);
                    }
                    else
                    {
                        url = string.Format(reportPrintUrl, campaignId);
                    }
                    break;
            }
            context.Response.Redirect(url, true);
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}