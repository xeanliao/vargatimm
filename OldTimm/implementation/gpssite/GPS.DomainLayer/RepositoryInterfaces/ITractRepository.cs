using System;
using GPS.DomainLayer.Entities;
using System.Collections.Generic;
namespace GPS.DataLayer
{
    public interface ITractRepository
    {
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.Tract> GetBoxItems(int boxId);
               System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.Tract> GetShapesAccordingToCodes(string[] arbitraryUniqueCodes);
        System.Collections.Generic.List<GPS.DomainLayer.Entities.Tract> GetTracts(string stateCode, string countyCode, string code);
        void SetTractEnabled(string stateCode, string countyCode, string code, int total, string description, bool enabled);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.Tract> GetTractsSatisfyingSubMap(GPS.DomainLayer.QuerySpecifications.TractsSatisfyingSubMapSpecification spec);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.Tract> GetTractsSatisfyingDistributionMap(GPS.DomainLayer.QuerySpecifications.TractsSatisfyingDistributionMapSpecification spec);
        Tract GetItem(int id);
    }
}
