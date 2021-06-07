using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer.RepositoryInterfaces;
using GPS.DataLayer.DataInfrastructure;

namespace GPS.DataLayer
{
    public interface IWorkSpace : IDisposable
    {
        #region Status queries
        /// <summary>
        /// Get a value indicating whether the workspace has been closed.
        /// </summary>
        Boolean IsClosed { get; }
        /// <summary>
        /// Get a value indicating whether the workspace is connected.
        /// </summary>
        Boolean IsConnected { get; }
        #endregion

        #region Ways to commit changes
        /// <summary>
        /// Begin a new transaction.
        /// </summary>
        /// <returns></returns>
        ITransaction BeginTransaction();
        /// <summary>
        /// Commit pending changes to persistent storage if the workspace is connected.
        /// </summary>
        void Commit();
        /// <summary>
        /// Commit pending changes to persistent storage and disconnect from the workspace.
        /// </summary>
        void CommitAndDisconnect();
        #endregion

        #region Clear changes
        /// <summary>
        /// Clear all pending changes.
        /// </summary>
        void Clear();
        #endregion

        #region Connection status management
        /// <summary>
        /// Close the workspace.
        /// </summary>
        void Close();
        /// <summary>
        /// Connect to the workspace.
        /// </summary>
        void Connect();
        /// <summary>
        /// Disconnect from the workspace.
        /// </summary>
        void Disconnect();
        #endregion

        #region Obtain repositories
        IRepositoryFactory Repositories { get; }
        #endregion
    }
}
