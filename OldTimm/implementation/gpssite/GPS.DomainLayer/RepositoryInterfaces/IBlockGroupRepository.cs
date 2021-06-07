using System;
using GPS.DomainLayer.Entities;
using System.Collections.Generic;
namespace GPS.DataLayer
{
    public interface IBlockGroupRepository
    {
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.BlockGroup> GetBoxItems(System.Collections.Generic.IEnumerable<int> boxIds);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.BlockGroup> GetBoxItems(int boxId);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.BlockGroup> GetShapesAccordingToCodes(string[] arbitraryUniqueCodes);
        void SetBlockGroupEnabled(string stateCode, string countyCode, string tractCode, string code, int total, string description, bool enabled);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.BlockGroup> GetBlockGroupsSatisfyingSubMap(GPS.DomainLayer.QuerySpecifications.BlockGroupsSatisfyingSubMapSpecification spec);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.BlockGroup> GetBlockGroupsSatisfyingDistributionMap(GPS.DomainLayer.QuerySpecifications.BlockGroupsSatisfyingDistributionMapSpecification spec);
        BlockGroup GetItem(int id);
        List<BlockGroup> GetAreaByBGArbitraryUniqueCode(string arbitraryUniqueCode);
    }
}
