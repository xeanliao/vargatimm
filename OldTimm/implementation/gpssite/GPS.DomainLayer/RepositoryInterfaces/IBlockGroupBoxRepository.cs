using System;
namespace GPS.DataLayer
{
    public interface IBlockGroupBoxRepository
    {
        System.Collections.Generic.List<int> GetBoxIdList(System.Collections.Generic.List<GPS.DomainLayer.Entities.CampaignBlockGroupImported> entityList);
    }
}
