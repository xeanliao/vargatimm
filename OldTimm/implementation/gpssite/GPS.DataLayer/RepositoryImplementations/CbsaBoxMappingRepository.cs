using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class CbsaBoxMappingRepository : RepositoryBase, GPS.DataLayer.ICbsaBoxMappingRepository
    {
        public CbsaBoxMappingRepository() { }

        public CbsaBoxMappingRepository(ISession session) : base(session) { }

        /// <summary>
        /// Get Box Mappings by box'id
        /// </summary>
        /// <param name="boxIds">the box' id</param>
        /// <returns>the box mapping</returns>
        public IQueryable<MetropolitanCoreAreaBoxMapping> GetBoxMapping(int boxId)
        {
            return InternalSession.Linq<MetropolitanCoreAreaBoxMapping>().Where(b => b.BoxId == boxId);
        }
    }
}
