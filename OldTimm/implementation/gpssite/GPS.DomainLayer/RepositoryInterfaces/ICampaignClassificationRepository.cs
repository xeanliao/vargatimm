using System;
namespace GPS.DataLayer
{
    public interface ICampaignClassificationRepository
    {
        void Delete(int campaignId);
        System.Linq.IQueryable<GPS.DomainLayer.Entities.CampaignClassification> GetEntity(int campaignId);
        void Insert(System.Collections.Generic.List<GPS.DomainLayer.Entities.CampaignClassification> entityList);
    }
}
