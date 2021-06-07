using System;
namespace GPS.DataLayer
{
    public interface IDistributionJobRepository
    {
        GPS.DomainLayer.Entities.DistributionJob AddDistributionJob(GPS.DomainLayer.Entities.DistributionJob dj);
        bool AuditorExisted(GPS.DomainLayer.Entities.AuditorAssignment auditor);
        void DeleteAuditor(GPS.DomainLayer.Entities.AuditorAssignment auditor);
        void DeleteDistributionJob(GPS.DomainLayer.Entities.DistributionJob dj);
        GPS.DomainLayer.Entities.DistributionJob GetDistributionJob(int distributionJobId);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.DistributionJob> GetDistributionJobsForCampaign(GPS.DomainLayer.Entities.Campaign c);
        void UpdateDistributionJob(GPS.DomainLayer.Entities.DistributionJob dj);
    }
}
