using System;
using GPS.DomainLayer.Entities;
using System.Collections.Generic;
namespace GPS.DataLayer
{
    public interface IPremiumCRouteRepository
    {
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.PremiumCRoute> GetBoxItems(System.Collections.Generic.IEnumerable<int> boxIds);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.PremiumCRoute> GetBoxItems(int boxId);
        System.Collections.Generic.List<GPS.DomainLayer.Entities.PremiumCRoute> GetPremiumCRoutes(int boxId);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.PremiumCRoute> GetShapesAccordingToCodes(string[] geocodes);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.PremiumCRoute> GetPremiumCRoutesSatisfyingSubMap(GPS.DomainLayer.QuerySpecifications.PremiumCRoutesSatisfyingSubMapSpecification spec);
        System.Collections.Generic.IEnumerable<GPS.DomainLayer.Entities.PremiumCRoute> GetPremiumCRoutesSatisfyingDistributionMap(GPS.DomainLayer.QuerySpecifications.PremiumCRoutesSatisfyingDistributionMapSpecification spec);
        PremiumCRoute GetItem(int id);
    }
}
