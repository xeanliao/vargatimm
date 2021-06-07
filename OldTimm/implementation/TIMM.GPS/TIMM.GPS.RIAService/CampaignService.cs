using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.DomainServices.EntityFramework;
using System.ServiceModel.DomainServices.Hosting;
using TIMM.GPS.Model;
using System.ServiceModel.DomainServices.Server;

namespace TIMM.GPS.RIAService
{
    [EnableClientAccess]
    public class CampaignService : DbDomainService<TIMMContext>
    {
        [Query]
        public IQueryable<Campaign> Query()
        {
            return new TIMMContext().Campaigns;
        }
    }
}
