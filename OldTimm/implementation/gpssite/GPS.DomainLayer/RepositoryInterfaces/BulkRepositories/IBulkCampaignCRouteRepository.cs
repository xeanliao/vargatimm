using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.RepositoryInterfaces.BulkRepositories
{
    public interface IBulkCampaignCRouteRepository
    {
        void Add(CampaignCRouteImported campaignCRouteImported);
        void Update(CampaignCRouteImported campaignCRouteImported);
        void DeleteByCampaign(Int32 campaignId);
    }
}
