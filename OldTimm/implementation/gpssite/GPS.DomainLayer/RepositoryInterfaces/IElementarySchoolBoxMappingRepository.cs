using System;
namespace GPS.DataLayer
{
    public interface IElementarySchoolBoxMappingRepository
    {
        System.Linq.IQueryable<GPS.DomainLayer.Entities.ElementarySchoolAreaBoxMapping> GetBoxMapping(int boxId);
    }
}
