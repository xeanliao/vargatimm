using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class SecondarySchoolBoxMappingRepository : RepositoryBase, GPS.DataLayer.ISecondarySchoolBoxMappingRepository
    {
        public SecondarySchoolBoxMappingRepository() { }

        public SecondarySchoolBoxMappingRepository(ISession session) : base(session) { }

        /// <summary>
        /// Get Box Mappings by box'id
        /// </summary>
        /// <param name="boxIds">the box' id</param>
        /// <returns>the box mapping</returns>
        public IQueryable<SecondarySchoolAreaBoxMapping> GetBoxMapping(int boxId)
        {
            return InternalSession.Linq<SecondarySchoolAreaBoxMapping>().Where(b => b.BoxId == boxId);
        }
    }
}
