using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class CampaignPercentageColorRepository : RepositoryBase, GPS.DataLayer.ICampaignPercentageColorRepository
    {
        public CampaignPercentageColorRepository() { }

        public CampaignPercentageColorRepository(ISession session) : base(session) { }

        public void ReplaceCampaignColorsWith(int campaignId, IEnumerable<CampaignPercentageColor> entities)
        {
            Campaign c = InternalSession.Get<Campaign>(campaignId);

            if (null != c)
            {
                // Clear existing colors
                foreach (CampaignPercentageColor cpc in c.CampaignPercentageColors)
                {
                    InternalSession.Delete(cpc);
                }

                // Add new colors
                foreach (CampaignPercentageColor cpc in entities)
                {
                    InternalSession.Save(cpc);
                }

                InternalSession.Flush();
            }
        }
    }
}
