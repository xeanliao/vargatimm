using System;
using System.Collections.Generic;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public interface IDistributionMapRepository
    {
        IEnumerable<DistributionMap> GetDistributionMaps(int submapId);
        void AddDistributionMap(DistributionMap distributionMap);
        void UpdateDistributionMap(DistributionMap distributionMap);
        void AddDistributionMaps(IList<DistributionMap> distributionMaps);
        void DeleteDistributionMap(DistributionMap distributionMap);
        DistributionMap GetEntity(int id);
      
    }
}
