using System;
namespace GPS.DataLayer
{
    public interface INdAddressRepository
    {
        void DeleteNdAddress(GPS.DomainLayer.Entities.NdAddress address);
        GPS.DomainLayer.Entities.NdAddress GetNdAddress(int id);
        System.Collections.Generic.List<GPS.DomainLayer.Entities.NdAddress> GetNdAddresses(string street, string zipCode);
        System.Collections.Generic.IList<GPS.DomainLayer.Entities.NdAddress> GetNdAddresses(int boxId);
        System.Collections.Generic.IList<GPS.DomainLayer.Entities.NdAddress> GetNdAddresses();
        void InsertNdAddress(GPS.DomainLayer.Entities.NdAddress address);
    }
}
