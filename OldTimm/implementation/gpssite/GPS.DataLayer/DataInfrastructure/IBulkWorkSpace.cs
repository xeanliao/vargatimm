using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DataLayer.DataInfrastructure
{
    public interface IBulkWorkSpace : IDisposable
    {
        #region Ways to commit changes
        /// <summary>
        /// Begin a new transaction.
        /// </summary>
        /// <returns></returns>
        ITransaction BeginTransaction();
        #endregion

        #region Obtain repositories
        IBulkRepositoryFactory Repositories { get; }
        #endregion
    }
}
