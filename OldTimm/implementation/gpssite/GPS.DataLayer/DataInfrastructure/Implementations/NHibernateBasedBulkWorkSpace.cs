using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DataLayer.DataInfrastructure.Implementations
{
    internal class NHibernateBasedBulkWorkSpace : IBulkWorkSpace
    {
        #region Constructors
        public NHibernateBasedBulkWorkSpace(NHibernate.IStatelessSession session)
        {
            _session = session;
        }
        #endregion

        #region IBulkWorkSpace Members

        ITransaction IBulkWorkSpace.BeginTransaction()
        {
            return new NHibernateBasedTransaction(_session.BeginTransaction());
        }

        IBulkRepositoryFactory IBulkWorkSpace.Repositories
        {
            get { return new BulkRepositoryFactory(_session); }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            _session.Dispose();
        }

        #endregion

        #region Implementations
        private NHibernate.IStatelessSession _session;
        #endregion
    }
}
