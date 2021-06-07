using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace GPS.DataLayer.DataInfrastructure.Implementations
{
    internal class NHibernateBasedTransaction : ITransaction
    {
        #region Constructors
        public NHibernateBasedTransaction(NHibernate.ITransaction nhibernateTransaction)
        {
            _nhibernateTransaction = nhibernateTransaction;
        }
        #endregion

        #region ITransaction Members

        void ITransaction.Commit()
        {
            _nhibernateTransaction.Commit();
        }

        void ITransaction.Rollback()
        {
            _nhibernateTransaction.Rollback();
        }

        #endregion


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            _nhibernateTransaction.Dispose();
        }

        #endregion

        #region Implementations
        private NHibernate.ITransaction _nhibernateTransaction;
        #endregion
    }
}
