using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class UrbanBoxMappingRepository : RepositoryBase, GPS.DataLayer.IUrbanBoxMappingRepository
    {
        public UrbanBoxMappingRepository() { }

        public UrbanBoxMappingRepository(ISession session) : base(session) { }

        /// <summary>
        /// Get Box Mappings by box'id
        /// </summary>
        /// <param name="boxIds">the box' id</param>
        /// <returns>the box mapping</returns>
        public IQueryable<UrbanAreaBoxMapping> GetBoxMapping(int boxId)
        {
            return InternalSession.Linq<UrbanAreaBoxMapping>().Where(b => b.BoxId == boxId);
        }
    }
}
