using System;
using GPS.DomainLayer.Entities;
namespace GPS.DataLayer
{
    public interface IAddressRepository
    {
        Address GetEntity(int addressId);
        void Update(Address address);
        void Insert(Address address);
    }
}
