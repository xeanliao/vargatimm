using System;
namespace GPS.DataLayer
{
    public interface ICampaignPercentageColorRepository
    {
        void ReplaceCampaignColorsWith(int campaignId, System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.CampaignPercentageColor> entities);
    }
}
