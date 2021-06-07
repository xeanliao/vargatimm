using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.RepositoryInterfaces.BulkRepositories
{ 

    public interface IBulkDistributionMapCoordinateRepository
    {
        void Add(DistributionMapCoordinate coordinate);
        void DeleteByDistributionMap(int distributionmapId);
    }
}


