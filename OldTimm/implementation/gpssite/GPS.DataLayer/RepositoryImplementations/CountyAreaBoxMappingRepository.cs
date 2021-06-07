using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class CountyAreaBoxMappingRepository : RepositoryBase, GPS.DataLayer.ICountyAreaBoxMappingRepository
    {
        public CountyAreaBoxMappingRepository() { }

        public CountyAreaBoxMappingRepository(ISession session) : base(session) { }

        /// <summary>
        /// Get All mapping for county area and box 
        /// </summary>
        /// <returns>the CountyAreaBoxMapping area</returns>
        public IQueryable<CountyAreaBoxMapping> GetAll()
        {
            return InternalSession.Linq<CountyAreaBoxMapping>();
        }

        /// <summary>
        /// Get Box Mappings by box'id
        /// </summary>
        /// <param name="boxIds">the box' id</param>
        /// <returns>the box mapping</returns>
        public IQueryable<CountyAreaBoxMapping> GetBoxMapping(int boxId)
        {
            return InternalSession.Linq<CountyAreaBoxMapping>().Where(b => b.BoxId == boxId);
        }
    }
}
