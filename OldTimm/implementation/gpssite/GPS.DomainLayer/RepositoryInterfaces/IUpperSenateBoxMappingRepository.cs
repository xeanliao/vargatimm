using System;
namespace GPS.DataLayer
{
    public interface IUpperSenateBoxMappingRepository
    {
        System.Linq.IQueryable<GPS.DomainLayer.Entities.UpperSenateAreaBoxMapping> GetBoxMapping(int boxId);
    }
}
