using System;
namespace GPS.DataLayer
{
    public interface ISecondarySchoolBoxMappingRepository
    {
        System.Linq.IQueryable<GPS.DomainLayer.Entities.SecondarySchoolAreaBoxMapping> GetBoxMapping(int boxId);
    }
}
