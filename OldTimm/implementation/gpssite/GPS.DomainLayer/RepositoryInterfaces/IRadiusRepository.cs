using System;
using GPS.DomainLayer.Entities;
namespace GPS.DataLayer
{
    public interface IRadiusRepository
    {
        void DeleteByCampaign(int campaignId);
        void Insert(Radiuse radiuse);
        void InsertEntityList(System.Collections.Generic.List<Radiuse> entityList);
        Radiuse GetEntity(int id);
        void Update(Radiuse radiuse);
    }
}
