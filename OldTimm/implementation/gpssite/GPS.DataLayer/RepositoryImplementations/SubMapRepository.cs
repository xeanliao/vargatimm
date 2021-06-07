using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;
using System.Data.Linq;
using GPS.DomainLayer.Interfaces;
using GPS.DataLayer.ValueObjects;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class SubMapRepository : RepositoryBase, GPS.DataLayer.ISubMapRepository
    {         /// <summary>
        /// Default constructor
        /// </summary>
        public SubMapRepository() { }

        public SubMapRepository(ISession session) : base(session) { }

       
        /// <summary>
        /// Return the specified campaign entity.
        /// </summary>
        /// <param name="id">The Id of the campaign to return.</param>
        /// <returns>The campaign entity</returns>
        public SubMap GetEntity(int id)
        {
            return InternalSession.Get<SubMap>(id);
        }
    }
}
