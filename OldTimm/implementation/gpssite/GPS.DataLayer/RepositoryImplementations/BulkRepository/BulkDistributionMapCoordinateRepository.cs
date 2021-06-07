using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.RepositoryInterfaces.BulkRepositories;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer.RepositoryImplementations.BulkRepository
{
   

    class BulkDistributionMapCoordinateRepository : BulkRepositoryBase, IBulkDistributionMapCoordinateRepository
    {
        #region IBulkDistributionMapCoordinateRepository Members

        public void Add(GPS.DomainLayer.Entities.DistributionMapCoordinate coordinate)
        {
            InternalSession.Insert(coordinate);
        }

        public void DeleteByDistributionMap(int distributionmapId)
        {
            InternalSession.CreateQuery("delete from DistributionMapCoordinate sc where sc.DistributionMapId = :distributionMapId")
                .SetInt32("distributionMapId", distributionmapId)
                .ExecuteUpdate();
        }

        #endregion
    }
}
