using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class CampaignClassificationRepository : RepositoryBase, GPS.DataLayer.ICampaignClassificationRepository
    {
        public CampaignClassificationRepository() { }

        public CampaignClassificationRepository(ISession session) : base(session) { }

        public void Insert(List<CampaignClassification> entityList)
        {
            foreach (CampaignClassification ccr in entityList)
            {
                InternalSession.Save(ccr);
            }

            InternalSession.Flush();
        }

        public IQueryable<CampaignClassification> GetEntity(int campaignId)
        {
            return InternalSession.Linq<CampaignClassification>().Where(c => c.Campaign.Id == campaignId);
        }

        public void Delete(int campaignId)
        {
            IEnumerable<CampaignClassification> entityList = InternalSession.Linq<CampaignClassification>().Where(c => c.Campaign.Id == campaignId);

            if (null != entityList)
            {
                foreach (CampaignClassification cc in entityList)
                {
                    InternalSession.Delete(cc);
                }

                InternalSession.Flush();
            }
        }
    }
}
