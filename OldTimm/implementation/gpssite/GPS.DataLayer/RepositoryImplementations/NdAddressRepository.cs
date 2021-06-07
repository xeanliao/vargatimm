using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class NdAddressRepository : RepositoryBase, GPS.DataLayer.INdAddressRepository
    {
        public NdAddressRepository() { }

        public NdAddressRepository(ISession session) : base(session) { }

        /// <summary>
        /// Insert NdAddress to database
        /// </summary>
        /// <param name="address"></param>
        public void InsertNdAddress(NdAddress address)
        {
            base.Insert(address);
        }

        /// <summary>
        /// Delete NdAddress from database
        /// </summary>
        /// <param name="address"></param>
        public void DeleteNdAddress(NdAddress address)
        {
            base.Delete(address);
        }

        /// <summary>
        /// Get NdAddresses by box id from database
        /// </summary>
        /// <param name="boxId">geo box id</param>
        /// <returns></returns>
        public IList<NdAddress> GetNdAddresses(int boxId)
        {
            const string queryFormat = "select distinct t from NdAddress t join t.NdAddressBoxMappings tbm where tbm.BoxId = :boxId";
            return InternalSession.CreateQuery(queryFormat).SetInt32("boxId", boxId).List<NdAddress>();
        }

        /// <summary>
        /// Get NdAddresses by box id from database
        /// </summary>
        /// <param name="boxId">geo box id</param>
        /// <returns></returns>
        public IList<NdAddress> GetNdAddresses()
        {
            const string queryFormat = "select distinct t from NdAddress t join t.NdAddressBoxMappings tbm ";
            return InternalSession.CreateQuery(queryFormat).List<NdAddress>();
        }

        /// <summary>
        /// Get NdAddresses by street and zip code from database
        /// </summary>
        /// <param name="boxId">geo box id</param>
        /// <returns></returns>
        public List<NdAddress> GetNdAddresses(string street, string zipCode)
        {
            List<NdAddress> results = new List<NdAddress>();

            const string queryFormat = "select distinct na from NdAddress na where na.Street = :street and na.ZipCode = :zipCode";

            IList<NdAddress> items = InternalSession.CreateQuery(queryFormat).SetString("street", street).SetString("zipCode", zipCode).List<NdAddress>();

            results.AddRange(items);

            return results;
        }

        /// <summary>
        /// Get NdAddresses id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NdAddress GetNdAddress(int id)
        {
            return InternalSession.Get<NdAddress>(id);
        }
    }
}
