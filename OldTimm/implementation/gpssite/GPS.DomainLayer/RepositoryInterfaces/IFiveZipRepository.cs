using System;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Entities;
using System.Collections.Generic;

namespace GPS.DataLayer
{
    public interface IFiveZipRepository : IExportSourceRepository<FiveZipArea>
    {
        List<FiveZipArea> GetAreaByZipCode(string zipCode);
        IEnumerable<FiveZipArea> GetBoxItems(int boxId);
        //IEnumerable<FiveZipArea> GetExportSource(IExportSourceSpecification spec);
        IEnumerable<FiveZipArea> GetShapesAccordingToCodes(string[] codes);
        void SetFiveZipEnabled(string code, int total, string desription, bool enabled);
        System.Collections.Generic.IEnumerable<FiveZipArea> GetFiveZipsSatisfyingSubMap(GPS.DomainLayer.QuerySpecifications.FiveZipsSatisfyingSubMapSpecification spec);
        System.Collections.Generic.IEnumerable<FiveZipArea> GetFiveZipsSatisfyingDistributionMap(GPS.DomainLayer.QuerySpecifications.FiveZipsSatisfyingDistributionMapSpecification spec);
        FiveZipArea GetItem(int id);

        IEnumerable<BlockGroup> GetBGByTract(int tractId);
        IEnumerable<BlockGroup> GetBGByFiveZip(int fivezipId);
        IEnumerable<BlockGroup> GetBGByPremiumCRoute(int croutId);
    }
}
