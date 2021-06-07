using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.RepositoryInterfaces.BulkRepositories;

namespace GPS.DataLayer.RepositoryImplementations.BulkRepository
{
    public class BulkCampaignBGRepository : BulkRepositoryBase, IBulkCampaignBGRepository
    {
        public BulkCampaignBGRepository() { }

        public BulkCampaignBGRepository(NHibernate.IStatelessSession session) : base(session) { }

        #region IBulkCampaignBGRepository Members

        void IBulkCampaignBGRepository.Add(CampaignBlockGroupImported campaignBlockGroupImported)
        {
            InternalSession.Insert(campaignBlockGroupImported);
        }

        void IBulkCampaignBGRepository.DeleteAllCampaignBlockGroupImportedsOfCampaign(int campaignId)
        {
            InternalSession.GetNamedQuery("CampaignBlockGroupImported.DeleteAllForCampaign")
                .SetInt32("campaignId", campaignId)
                .ExecuteUpdate();
        }

        void IBulkCampaignBGRepository.Update(CampaignBlockGroupImported campaignBlockGroupImported)
        {
            InternalSession.Update(campaignBlockGroupImported);
        }

        #endregion
    }
}
