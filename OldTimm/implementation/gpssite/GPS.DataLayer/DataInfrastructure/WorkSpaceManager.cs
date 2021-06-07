using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer.DataInfrastructure;
using GPS.DataLayer.DataInfrastructure.Implementations;

namespace GPS.DataLayer
{
    /// <summary>
    /// Manage the workspaces(Unit of Work) in the application domain.
    /// </summary>
    public sealed class WorkSpaceManager
    {
        #region WorkSpace factories
        /// <summary>
        /// Get a new workspace or an available workspace in a workspace pool.
        /// </summary>
        /// <returns>An <see cref="IWorkSpace"/>.</returns>
        public IWorkSpace NewWorkSpace()
        {
            return new NHibernateBasedWorkSpace(new NHibernateHelper().GetANewSession());
        }

        public IBulkWorkSpace NewBulkWorkSpace()
        {
            return new NHibernateBasedBulkWorkSpace(new NHibernateHelper().GetANewStatelessSession());
        }
        #endregion

        #region Single instance
        public static WorkSpaceManager Instance
        {
            get { return _instance; }
        }
        #endregion

        #region Constructors
        private WorkSpaceManager() { }

        static WorkSpaceManager()
        {
            _instance = new WorkSpaceManager();
        }
        #endregion

        #region Implementations
        private static readonly WorkSpaceManager _instance;
        #endregion
    }
}
