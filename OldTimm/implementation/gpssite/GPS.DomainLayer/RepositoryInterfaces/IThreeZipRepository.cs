using System;
using GPS.DomainLayer.Entities;
using System.Collections.Generic;
namespace GPS.DataLayer
{
    public interface IThreeZipRepository
    {
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.ThreeZipArea> GetBoxItems(int boxId);
        System.Linq.IQueryable<GPS.DomainLayer.Entities.ThreeZipArea> GetEntityList(int boxId);
        ThreeZipArea GetItem(int id);
    }
}
