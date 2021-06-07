using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.RepositoryInterfaces.BulkRepositories;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer.RepositoryImplementations.BulkRepository
{
    class BulkSubMapCoordinateRepository : BulkRepositoryBase, IBulkSubMapCoordinateRepository
    {
        #region IBulkSubMapCoordinateRepository Members

        public void Add(GPS.DomainLayer.Entities.SubMapCoordinate coordinate)
        {
            InternalSession.Insert(coordinate);
        }

        public void DeleteBySubMap(int submapId)
        {
            
            //InternalSession.CreateQuery("delete from SubMapCoordinate sc where sc.SubMapId = :subMapId")
            //    .SetInt32("subMapId",submapId)
            //    .ExecuteUpdate();
            var targets = InternalSession.CreateQuery("delete from SubMapCoordinate a where a.SubMap.Id = :submapId")
                .SetInt32("submapId", submapId)
                .ExecuteUpdate();
        }

        #endregion
    }
}
