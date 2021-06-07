using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using GPS.DataLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using GPS.DomainLayer.Area;
using System.Web;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.RepositoryInterfaces;
using GPS.DomainLayer.Enum;
using System.Data.Linq;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.QuerySpecifications;
using NetTopologySuite.Geometries;

namespace GPS.DomainLayer.Campaign
{
    public class CampaignManager
    {


        public static void CreateNewCampaign(GPS.DomainLayer.Entities.Campaign campaign, ICampaignRepository rep)
        {
            campaign.Id = System.Guid.NewGuid().GetHashCode();
            campaign.Sequence = GenerateCampaignSequence(campaign.Date, campaign.CreatorName, campaign.ClientCode, campaign.AreaDescription, rep);
            rep.Create(campaign);
        }

        /// <summary>
        /// Return the Sequence value that should be assigned to a new campaign 
        /// with specified user name, for specified customer, and in the 
        /// specified date.
        /// </summary>
        /// <param name="date">The date of the new campaign. Only year, month, 
        /// and date counts; hours, minutes, and seconds will be ignored.</param>
        /// <param name="userName">The user name of the new campaign.</param>
        /// <param name="clientCode">The client code of the new campaign.</param>
        /// <param name="areaDescription">The area description of the new campaign.</param>
        /// <returns>The Sequence value of the new campaign.</returns>
        public static int GenerateCampaignSequence(DateTime date, String userName, String clientCode, String areaDescription)
        {    
            ICampaignRepository rep = WorkSpaceManager.Instance.NewWorkSpace().Repositories.CampaignRepository;
            return GenerateCampaignSequence(date, userName, clientCode, areaDescription, rep);
        }

        public static int GenerateCampaignSequence(DateTime date, String userName, String clientCode, String areaDescription, ICampaignRepository rep)
        {
            return rep.GetMaxSequence(date, userName, clientCode, areaDescription) + 1;
        }

        /// <summary>
        /// Get All Campaign
        /// </summary>
        /// <returns>the campaign list</returns>
        public static List<GPS.DomainLayer.Entities.Campaign> GetAllCampaign()
        {
            List<GPS.DomainLayer.Entities.Campaign> campaigns = new List<GPS.DomainLayer.Entities.Campaign>();
            CampaignRepository rep = new CampaignRepository();
            campaigns = rep.GetAllEntities().ToList();

            return campaigns;
        }


        public static void CalculateHolesInSubMap(GPS.DomainLayer.Entities.Campaign campaign)
        {
            foreach (var subMap in campaign.SubMaps)
            {
                if (subMap.SubMapRecords != null && subMap.SubMapRecords.Count > 0)
                {

                    List<int> fiveZipIds = new List<int>();
                    List<int> cRouteIds = new List<int>();
                    Classifications? currentType = null;
                    foreach (var record in subMap.SubMapRecords)
                    {
                        switch (record.Classification)
                        {
                            case Classifications.Z5:
                                if (record.Value)
                                {
                                    fiveZipIds.Add(record.AreaId);
                                }
                                if (!currentType.HasValue)
                                {
                                    currentType = Classifications.Z5;
                                }
                                break;
                            case Classifications.PremiumCRoute:
                                if (record.Value)
                                {
                                    cRouteIds.Add(record.AreaId);
                                }
                                if (!currentType.HasValue)
                                {
                                    currentType = Classifications.PremiumCRoute;
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    switch (currentType)
                    {
                        case Classifications.Z5:
                            subMap.Holes = FindFiveZipAreaHoles(fiveZipIds);
                            break;
                        case Classifications.PremiumCRoute:
                            subMap.Holes = FindCRouteAreaHoles(cRouteIds);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static void CalculateHolesInDMap(GPS.DomainLayer.Entities.Campaign campaign)
        {
            foreach (var subMap in campaign.SubMaps)
            {
                foreach (var dMap in subMap.DistributionMaps)
                {
                    if (dMap.DistributionMapRecords != null && dMap.DistributionMapRecords.Count > 0)
                    {

                        List<int> fiveZipIds = new List<int>();
                        List<int> cRouteIds = new List<int>();
                        Classifications? currentType = null;
                        foreach (var record in dMap.DistributionMapRecords)
                        {
                            switch (record.Classification)
                            {
                                case Classifications.Z5:
                                    if (record.Value)
                                    {
                                        fiveZipIds.Add(record.AreaId);
                                    }
                                    
                                    if (!currentType.HasValue)
                                    {
                                        currentType = Classifications.Z5;
                                    }
                                    break;
                                case Classifications.PremiumCRoute:
                                    if (record.Value)
                                    {
                                        cRouteIds.Add(record.AreaId);
                                    }
                                    
                                    if (!currentType.HasValue)
                                    {
                                        currentType = Classifications.PremiumCRoute;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        switch (currentType)
                        {
                            case Classifications.Z5:
                                dMap.Holes = FindFiveZipAreaHoles(fiveZipIds);
                                break;
                            case Classifications.PremiumCRoute:
                                dMap.Holes = FindCRouteAreaHoles(cRouteIds);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private static List<double[][]> FindFiveZipAreaHoles(List<int> fiveZipIds)
        {
            //load Five Zips
            FiveZipsSatisfyingSubMapSpecification fSpec = new FiveZipsSatisfyingSubMapSpecification(fiveZipIds, null, null);
            IEnumerable<FiveZipArea> fiveZips = fSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.FiveZipRepository);
            List<Polygon> subMapAreas = new List<Polygon>();
            foreach (var area in fiveZips)
            {
                CoordinateList locations = new CoordinateList();
                foreach (var point in area.FiveZipAreaCoordinates)
                {
                    locations.Add(new GeoAPI.Geometries.Coordinate(point.Longitude, point.Latitude));
                }
                locations.Add(new GeoAPI.Geometries.Coordinate(locations[0].X, locations[0].Y));

                Polygon one = new Polygon(new LinearRing(locations.ToArray()));
                one = one.Buffer(0) as Polygon;
                subMapAreas.Add(one);
            }
            var unionPolygon = new MultiPolygon(subMapAreas.ToArray()).Union();

            List<double[][]> holes = new List<double[][]>();
            for (var i = 0; i < unionPolygon.NumGeometries; i++)
            {
                var polygon = unionPolygon.GetGeometryN(i) as Polygon;
                if (polygon != null && polygon.Holes.Length > 0)
                {
                    foreach (var hole in polygon.Holes)
                    {
                        var holeLocations = hole.Coordinates;
                        double[][] holeBoundary = new double[holeLocations.Length][];
                        for (int j = 0; j < holeLocations.Length; j++)
                        {
                            holeBoundary[j] = new double[] { holeLocations[j].Y, holeLocations[j].X };
                        }
                        holes.Add(holeBoundary);
                    }
                }
            }
            return holes;
        }

        private static List<double[][]> FindCRouteAreaHoles(List<int> cRouteIds)
        {
            //load Five Zips
            PremiumCRoutesSatisfyingSubMapSpecification cSpec = new PremiumCRoutesSatisfyingSubMapSpecification(
                        null, null, cRouteIds, null);
            IEnumerable<PremiumCRoute> cRoutes = cSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.PremiumCRouteRepository);
            List<Polygon> subMapAreas = new List<Polygon>();
            foreach (var area in cRoutes)
            {
                CoordinateList locations = new CoordinateList();
                foreach (var point in area.PremiumCRouteCoordinates)
                {
                    locations.Add(new GeoAPI.Geometries.Coordinate(point.Longitude, point.Latitude));
                }
                locations.Add(new GeoAPI.Geometries.Coordinate(locations[0].X, locations[0].Y));

                Polygon one = new Polygon(new LinearRing(locations.ToArray()));
                one = one.Buffer(0) as Polygon;
                subMapAreas.Add(one);
            }
            var unionPolygon = new MultiPolygon(subMapAreas.ToArray()).Union();

            List<double[][]> holes = new List<double[][]>();
            for (var i = 0; i < unionPolygon.NumGeometries; i++)
            {
                var polygon = unionPolygon.GetGeometryN(i) as Polygon;
                if (polygon != null && polygon.Holes.Length > 0)
                {
                    foreach (var hole in polygon.Holes)
                    {
                        var holeLocations = hole.Coordinates;
                        double[][] holeBoundary = new double[holeLocations.Length][];
                        for (int j = 0; j < holeLocations.Length; j++)
                        {
                            holeBoundary[j] = new double[] { holeLocations[j].Y, holeLocations[j].X };
                        }
                        holes.Add(holeBoundary);
                    }
                }
            }
            return holes;
        }
    }
}
