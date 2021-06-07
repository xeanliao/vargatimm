using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.RepositoryInterfaces.BulkRepositories;
using GPS.DataLayer.DataInfrastructure.Implementations;

namespace GPS.DataLayer.RepositoryImplementations.BulkRepository
{
    public class BulkRepositoryBase : IBulkRepository
    {
        #region IBulkRepository Members

        protected NHibernate.IStatelessSession InternalSession { get; set; }

        #endregion

        protected BulkRepositoryBase()
        {
            InternalSession = new NHibernateHelper().GetANewStatelessSession();
        }

        protected BulkRepositoryBase(NHibernate.IStatelessSession session)
        {
            InternalSession = session;
        }

        #region IBulkRepository Members

        NHibernate.IStatelessSession IBulkRepository.Session
        {
            set { InternalSession = value; }
        }

        #endregion
    }
}
