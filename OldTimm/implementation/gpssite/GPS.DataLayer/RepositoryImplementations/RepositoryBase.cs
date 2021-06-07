using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.RepositoryInterfaces;
using GPS.DataLayer.DataInfrastructure.Implementations;

namespace GPS.DataLayer
{
    /// <summary>
    /// Provided a base for easier building of repository classes.
    /// </summary>
    public class RepositoryBase : IRepository
    {
        protected ISession InternalSession { get; set; }

        protected RepositoryBase()
        {
            InternalSession = new NHibernateHelper().GetANewSession();
        }

        protected RepositoryBase(ISession session)
        {
            InternalSession = session;
        }

        /// <summary>
        /// Insert the specified persistent object to database.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        protected void Insert(Object entity)
        {
            if (null != entity)
            {
                InternalSession.Save(entity);
                InternalSession.Flush();
            }
        }

        /// <summary>
        /// Update the specified persistent object to database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        protected void Update(Object entity)
        {
            if (null != entity)
            {
                InternalSession.SaveOrUpdate(entity);
                InternalSession.Flush();
            }
        }

        protected void UpdateCopy(Object entity)
        {
            if (null != entity)
            {
                InternalSession.SaveOrUpdateCopy(entity);
                InternalSession.Flush();
            }
        }

        /// <summary>
        /// Delete the specified persistent object from database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        protected void Delete(Object entity)
        {
            if (null != entity)
            {
                InternalSession.Delete(entity);
                InternalSession.Flush();
            }
        }

        #region IRepository Members

        ISession IRepository.Session
        {
            set { InternalSession = value; }
        }

        #endregion
    }
}
