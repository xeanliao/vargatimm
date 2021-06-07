using System;
namespace GPS.DataLayer
{
    public interface IUnifiedSchoolBoxMappingRepository
    {
        System.Linq.IQueryable<GPS.DomainLayer.Entities.UnifiedSchoolAreaBoxMapping> GetBoxMapping(int boxId);
    }
}
