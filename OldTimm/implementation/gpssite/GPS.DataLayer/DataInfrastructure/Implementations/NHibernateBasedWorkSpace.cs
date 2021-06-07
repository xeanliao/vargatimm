using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer.RepositoryInterfaces;

namespace GPS.DataLayer.DataInfrastructure.Implementations
{
    internal class NHibernateBasedWorkSpace : IWorkSpace
    {
        #region Constructors
        public NHibernateBasedWorkSpace(NHibernate.ISession session)
        {
            _session = session;
        }
        #endregion

        #region IWorkSpace Members

        bool IWorkSpace.IsClosed
        {
            get { return !_session.IsOpen; }
        }

        bool IWorkSpace.IsConnected
        {
            get { return _session.IsConnected; }
        }

        ITransaction IWorkSpace.BeginTransaction()
        {
            return new NHibernateBasedTransaction(_session.BeginTransaction());
        }

        void IWorkSpace.Commit()
        {
            _session.Flush();
        }

        void IWorkSpace.CommitAndDisconnect()
        {
            _session.Flush();
            _session.Disconnect();
        }

        void IWorkSpace.Close()
        {
            _session.Close();
        }

        void IWorkSpace.Connect()
        {
            _session.Reconnect();
        }

        void IWorkSpace.Disconnect()
        {
            _session.Disconnect();
        }

        IRepositoryFactory IWorkSpace.Repositories
        {
            get 
            {
                return new RepositoryFactory(_session);
            }
        }
        #endregion

        #region Implementations
        private NHibernate.ISession _session;
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_session.IsOpen)
            {
                _session.Close();
            }
            _session.Dispose();
        }

        #endregion

        #region IWorkSpace Members


        void IWorkSpace.Clear()
        {
            _session.Clear();
        }

        #endregion
    }
}
