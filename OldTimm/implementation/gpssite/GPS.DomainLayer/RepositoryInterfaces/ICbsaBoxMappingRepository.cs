using System;
namespace GPS.DataLayer
{
    public interface ICbsaBoxMappingRepository
    {
        System.Linq.IQueryable<GPS.DomainLayer.Entities.MetropolitanCoreAreaBoxMapping> GetBoxMapping(int boxId);
    }
}
