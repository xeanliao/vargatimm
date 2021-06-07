using System;
namespace GPS.DataLayer
{
    public interface ICountyAreaBoxMappingRepository
    {
        System.Linq.IQueryable<GPS.DomainLayer.Entities.CountyAreaBoxMapping> GetAll();
        System.Linq.IQueryable<GPS.DomainLayer.Entities.CountyAreaBoxMapping> GetBoxMapping(int boxId);
    }
}
