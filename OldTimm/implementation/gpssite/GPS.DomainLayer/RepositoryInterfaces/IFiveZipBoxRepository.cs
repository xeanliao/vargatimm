using System;
namespace GPS.DataLayer
{
    public interface IFiveZipBoxRepository
    {
        System.Collections.Generic.List<int> GetBoxIdList(System.Collections.Generic.List<GPS.DomainLayer.Entities.CampaignFiveZipImported> entityList);
    }
}
