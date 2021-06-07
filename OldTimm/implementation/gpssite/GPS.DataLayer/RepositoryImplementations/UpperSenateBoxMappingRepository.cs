using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class UpperSenateBoxMappingRepository : RepositoryBase, GPS.DataLayer.IUpperSenateBoxMappingRepository
    {
        public UpperSenateBoxMappingRepository() { }

        public UpperSenateBoxMappingRepository(ISession session) : base(session) { }

        /// <summary>
        /// Get Box Mappings by box'id
        /// </summary>
        /// <param name="boxIds">the box' id</param>
        /// <returns>the box mapping</returns>
        public IQueryable<UpperSenateAreaBoxMapping> GetBoxMapping(int boxId)
        {
            return InternalSession.Linq<UpperSenateAreaBoxMapping>().Where(b => b.BoxId == boxId);
        }
    }
}
