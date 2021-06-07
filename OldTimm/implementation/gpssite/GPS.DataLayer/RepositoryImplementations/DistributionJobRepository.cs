using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class DistributionJobRepository : RepositoryBase, GPS.DataLayer.IDistributionJobRepository
    {
        public DistributionJobRepository() { }

        public DistributionJobRepository(ISession session) : base(session) { }

        public Boolean AuditorExisted(AuditorAssignment auditor)
        {
            const string queryFormat = "from Auditor a where a = :a";
            return null != InternalSession.CreateQuery(queryFormat).SetEntity("a", auditor).UniqueResult();
        }

        public DistributionJob GetDistributionJob(Int32 distributionJobId)
        {
            return InternalSession.Get<DistributionJob>(distributionJobId);
        }

        public IEnumerable<DistributionJob> GetDistributionJobsForCampaign(Campaign c)
        {
            string queryFormat = "select distinct dj from DistributionJob dj where dj._campaign = :c";
            return InternalSession.CreateQuery(queryFormat).SetParameter("c", c).List<DistributionJob>();
        }

        public DistributionJob AddDistributionJob(DistributionJob dj)
        {
            base.Insert(dj);
            return dj;
        }

        public void UpdateDistributionJob(DistributionJob dj)
        {
            base.Update(dj);
        }

        public void DeleteDistributionJob(DistributionJob dj)
        {
            base.Delete(dj);
        }

        public void DeleteAuditor(AuditorAssignment auditor)
        {
            base.Delete(auditor);
        }
    }
}
