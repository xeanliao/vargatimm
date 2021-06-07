using System;
namespace GPS.DataLayer
{
    public interface ILowerHouseBoxMappingRepository
    {
        System.Linq.IQueryable<GPS.DomainLayer.Entities.LowerHouseAreaBoxMapping> GetBoxMapping(int boxId);
    }
}
