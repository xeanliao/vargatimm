using System;
namespace GPS.DataLayer
{
    public interface IUrbanBoxMappingRepository
    {
        System.Linq.IQueryable<GPS.DomainLayer.Entities.UrbanAreaBoxMapping> GetBoxMapping(int boxId);
    }
}
