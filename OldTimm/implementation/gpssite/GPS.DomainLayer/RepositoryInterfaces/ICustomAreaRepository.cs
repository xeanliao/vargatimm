using System;
namespace GPS.DataLayer
{
    public interface ICustomAreaRepository
    {
        void DeleteCustomArea(GPS.DomainLayer.Entities.CustomArea area);
        GPS.DomainLayer.Entities.CustomArea GetCustomArea(string name);
        System.Collections.Generic.List<GPS.DomainLayer.Entities.CustomArea> GetCustomAreas(int boxId);
        System.Collections.Generic.List<GPS.DomainLayer.Entities.CustomArea> GetCustomAreas();
        void InsertCustomArea(GPS.DomainLayer.Entities.CustomArea area);
    }
}
