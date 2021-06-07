using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.RepositoryInterfaces.BulkRepositories;

namespace GPS.DataLayer.RepositoryImplementations.BulkRepository
{
    class BulkCampaignCRouteRepository : BulkRepositoryBase, IBulkCampaignCRouteRepository
    {

        #region IBulkCampaignCRouteRepository Members

        public void Add(GPS.DomainLayer.Entities.CampaignCRouteImported campaignCRouteImported)
        {
            InternalSession.Insert(campaignCRouteImported);
        }

        public void Update(GPS.DomainLayer.Entities.CampaignCRouteImported campaignCRouteImported)
        {
            InternalSession.Update(campaignCRouteImported);
        }

        public void DeleteByCampaign(int campaignId)
        {
            InternalSession.CreateQuery("CampaignCRouteImported.DeleteByCampaign")
                .SetInt32("campaignId", campaignId)
                .ExecuteUpdate();
        }

        #endregion
    }
}
