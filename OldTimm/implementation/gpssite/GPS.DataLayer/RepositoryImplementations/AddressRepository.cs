using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class AddressRepository : RepositoryBase, GPS.DataLayer.IAddressRepository
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AddressRepository() { }

        public AddressRepository(ISession session) : base(session) { }

        public void Insert(Address entity)
        {
            base.Insert(entity);
        }

        public Address GetEntity(int addressId)
        {
            return InternalSession.Get<Address>(addressId);
        }

        public void Update(Address address)
        {
            InternalSession.Update(address);
            InternalSession.Flush();
        }
    }
}
