using System;
namespace GPS.DataLayer
{
    public interface ICampaignRecordRepository
    {
        void DeleteEntityListById(int campaignId);
        System.Linq.IQueryable<GPS.DomainLayer.Entities.CampaignRecord> GetEntitiesByCampaign(int campaignId);
        GPS.DomainLayer.Entities.CampaignRecord GetEntity(int id);
        void InsertEntity(GPS.DomainLayer.Entities.CampaignRecord entity);
        void InsertEntityList(System.Collections.Generic.List<GPS.DomainLayer.Entities.CampaignRecord> entityList);
        void UpdateEntity(GPS.DomainLayer.Entities.CampaignRecord entity);
    }
}
