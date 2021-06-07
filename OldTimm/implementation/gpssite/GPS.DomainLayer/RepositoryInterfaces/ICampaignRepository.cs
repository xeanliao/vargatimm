using System;
namespace GPS.DataLayer
{
    public interface ICampaignRepository
    {
        void Create(GPS.DomainLayer.Entities.Campaign campaign);
        void Delete(GPS.DomainLayer.Entities.Campaign camp);
        void Delete(int campaignId);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.Campaign> GetAllByUser(GPS.DomainLayer.Entities.User user);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.Campaign> GetAllEntities();
        GPS.DomainLayer.Entities.Campaign GetEntity(int id);
        int GetMaxSequence(DateTime date, string userName, string clientCode, string areaDescription);
        void Update(GPS.DomainLayer.Entities.Campaign campaign);
        void UpdateCopy(GPS.DomainLayer.Entities.Campaign campaign);
        void NewMonitorAddress(GPS.DomainLayer.Entities.MonitorAddresses ma);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.Campaign> GetAllBySubmapStatus(GPS.DomainLayer.Entities.User user);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.Campaign> GetAllByDMStatus(GPS.DomainLayer.Entities.User user);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.Campaign> GetAllCampByDMStatusWithoutUser();
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.Campaign> GetAllBySubmapStatusWithoutUser();
        GPS.DomainLayer.Entities.Campaign GetCamNameByTaskId(int tid);
        string[] GetCamNameByTasks(int[] taskids);
        string[] GetCamNameByReports(int[] taskids);
    }
}
