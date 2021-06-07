using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.RepositoryInterfaces.BulkRepositories
{
    public interface IBulkSubMapCoordinateRepository
    {
         void Add(SubMapCoordinate coordinate);
         void DeleteBySubMap(int submapId);
    }
}
