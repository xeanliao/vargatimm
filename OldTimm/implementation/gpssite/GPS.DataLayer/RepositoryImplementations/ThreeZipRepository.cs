using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class ThreeZipRepository : RepositoryBase, GPS.DataLayer.IThreeZipRepository
    {
        public ThreeZipRepository() { }

        public ThreeZipRepository(ISession session) : base(session) { }

        /// <summary>
        /// Get ThreeZip Area by box'id
        /// </summary>
        /// <param name="boxIds">the box' id</param>
        /// <returns>the box mapping</returns>
        public IQueryable<ThreeZipArea> GetEntityList(int boxId)
        {
            const string queryFormat = "select distinct tza from ThreeZipArea tza join tza.ThreeZipBoxMappings tzbm where tzbm.BoxId = :boxId";

            IList<ThreeZipArea> items = InternalSession.CreateQuery(queryFormat).SetInt32("boxId", boxId).List<ThreeZipArea>();

            return items.AsQueryable();
        }

        public IEnumerable<ThreeZipArea> GetBoxItems(int boxId)
        {
            const string queryFormat = "select distinct tza from ThreeZipArea tza join tza.ThreeZipBoxMappings tzbm where tzbm.BoxId = :boxId";
            return InternalSession.CreateQuery(queryFormat).SetInt32("boxId", boxId).List<ThreeZipArea>();
        }

        public ThreeZipArea GetItem(int id)
        {
            return InternalSession.Get<ThreeZipArea>(id);
        }

    }
}
