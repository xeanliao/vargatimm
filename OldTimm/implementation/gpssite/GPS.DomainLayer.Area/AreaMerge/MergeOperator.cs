using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.QuerySpecifications;
using GPS.DomainLayer.Entities;
using GPS.DataLayer;
using GPS.DataLayer.ValueObjects;
using GPS.DomainLayer.Area.Import;
using System.Collections;
using NetTopologySuite.Geometries;
using log4net;

namespace GPS.DomainLayer.Area.AreaMerge
{
    public class MergeOperator
    {
        private static ILog m_Logger = LogManager.GetLogger(typeof(MergeOperator));

        /// <summary>
        /// use 
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public MergeResult MergeAreasAPI(int campaignId, double[][] basePolygonLocations, ref IEnumerable<AreaRecord> records, bool isAddAllShapesToSubMap = false)
        {
            #region load records source
            Classifications c = Classifications.Z3;

            foreach (AreaRecord record in records)
            {
                if (record.Value)
                {
                    c = record.Classification;
                    break;
                }
            }
            MergeResult result = null;
            if (c == Classifications.Z5)
            {
                result = GetSoureForFiveZipMapping(campaignId, records);
            }
            else if (c == Classifications.PremiumCRoute)
            {
                result = GetSoureForCRouteMapping(campaignId, records);
            }
            else if (c == Classifications.TRK)
            {
                result = GetSoureForTractMapping(campaignId, records);
            }
            else if (c == Classifications.BG)
            {
                result = GetSoureForBlockGroupMapping(campaignId, records);
            }

            if (result.Locs.Count == 0)
            {
                return result;
            }
            #endregion


            #region load submap polygon
            Polygon submapPolygon = null;
            if (basePolygonLocations != null && basePolygonLocations.Length > 0)
            {
                CoordinateList baseLocations = new CoordinateList();
                for (int i = 0; i < basePolygonLocations.Length; i++)
                {
                    baseLocations.Add(new GeoAPI.Geometries.Coordinate(basePolygonLocations[i][1], basePolygonLocations[i][0]));
                }
                baseLocations.Add(new GeoAPI.Geometries.Coordinate(basePolygonLocations[0][1], basePolygonLocations[0][0]));
                LinearRing baseRing = new LinearRing(baseLocations.ToArray());
                submapPolygon = new Polygon(baseRing);
                submapPolygon = submapPolygon.Buffer(0) as Polygon;
            }
            #endregion


            #region load all records polygon
            Dictionary<Polygon, int> allPolygon = new Dictionary<Polygon, int>();
            foreach (var record in result.RecordLocations)
            {
                CoordinateList coordinate = new CoordinateList();
                for (int i = 0; i < record.Value.Length; i++)
                {
                    coordinate.Add(new GeoAPI.Geometries.Coordinate(record.Value[i][1], record.Value[i][0]));
                }

                LineString ring = new LineString(coordinate.ToArray());
                if (!ring.IsClosed)
                {
                    //try to find the closed LinerRing
                    StringBuilder message = new StringBuilder();
                    message.AppendFormat("The {0} id={1} is not closed LinerRing!\r\n try to fix\r\n", c.ToString(), record.Key);
                    
                    HashSet<GeoAPI.Geometries.Coordinate> checkPoints = new HashSet<GeoAPI.Geometries.Coordinate>();
                    List<GeoAPI.Geometries.Coordinate> coincidePoints = new List<GeoAPI.Geometries.Coordinate>();
                    for (int i = ring.Coordinates.Length - 1; i >= 0 ; i--)
                    {
                        if(!checkPoints.Contains(ring.Coordinates[i]))
                        {
                            checkPoints.Add(ring.Coordinates[i]);
                        }
                        else
                        {
                            coincidePoints.Add(ring.Coordinates[i]);
                        }
                    }

                    message.AppendFormat("this area have {0} coincide points:\r\n", coincidePoints.Count);
                    coincidePoints.ForEach(i => {
                        message.AppendFormat("({0}, {1})\r\n ", i.X, i.Y);
                    });
                    message.AppendLine();
                    List<Polygon> fixMultiPolygon = new List<Polygon>();
                    foreach (var point in coincidePoints)
                    {
                        int startPointIndex = 0, endPointIndex = ring.Coordinates.Length - 1;
                        for (int i = 0; i < ring.Coordinates.Length; i++)
                        {
                            if (ring.Coordinates[i].Equals2D(point))
                            {
                                startPointIndex = i;
                                break;
                            }
                        }
                        for (int i = ring.Coordinates.Length - 1; i >= 0; i--)
                        {
                            if (ring.Coordinates[i].Equals2D(point))
                            {
                                endPointIndex = i;
                                break;
                            }
                        }
                        GeoAPI.Geometries.Coordinate[] fixedRing = new GeoAPI.Geometries.Coordinate[endPointIndex - startPointIndex + 1];
                        Array.Copy(ring.Coordinates, startPointIndex, fixedRing, 0, fixedRing.Length);
                        fixMultiPolygon.Add(new Polygon(new LinearRing(fixedRing)).Buffer(0) as Polygon);
                    }

                    var fixedPolygon = new MultiPolygon(fixMultiPolygon.ToArray()).Union();
                    if (fixedPolygon is Polygon && fixedPolygon.IsValid)
                    {
                        ring = new LineString(fixedPolygon.Coordinates);
                        message.AppendLine("fix successed!");
                    }
                    else
                    {
                        var fixClosedRing = ring.Coordinates.ToList();
                        fixClosedRing.Add(new GeoAPI.Geometries.Coordinate(ring.Coordinates[0].CoordinateValue));
                        ring = new LineString(fixClosedRing.ToArray());
                        message.AppendLine("fix failed!");
                    }
                    m_Logger.Warn(message.ToString());
                }

                Polygon area = new Polygon(new LinearRing(ring.Coordinates));
                //area = area.Buffer(0) as Polygon;
                var fixedArea = area.Buffer(0);
                if (fixedArea is Polygon)
                {
                    area = fixedArea as Polygon;
                }
                else
                {
                    //the polygon is invalid then log it
                    m_Logger.Warn(string.Format("The {0} id={1} is an invalid polygon", c.ToString(), record.Key));
                }
                allPolygon.Add(area, record.Key);
            }
            #endregion


            List<double[][]> coordinates = new List<double[][]>();
            List<double[][]> holes = new List<double[][]>();
            

            var unionPolygon = new MultiPolygon(allPolygon.Keys.ToArray()).Union();
            if (unionPolygon.NumGeometries == 1)//all area are connected
            {
                submapPolygon = unionPolygon.GetGeometryN(0) as Polygon;//set submap to merged area
                HashSet<int> needRemoveRecord = new HashSet<int>();
                //remove the records in holes
                if (submapPolygon.Holes.Length > 0)
                {
                    foreach (var shapes in allPolygon)
                    {
                        foreach (var hole in submapPolygon.Holes)
                        {
                            if (shapes.Key.Within(hole))
                            {
                                if (!needRemoveRecord.Contains(shapes.Value))
                                {
                                    needRemoveRecord.Add(shapes.Value);
                                }
                            }
                        }
                    }
                }
                
                if (needRemoveRecord.Count > 0)
                {
                    List<AreaRecord> resultRecords = new List<AreaRecord>();
                    foreach (var item in records)
                    {
                        if (needRemoveRecord.Contains(item.AreaId))
                        {
                            continue;
                        }
                        resultRecords.Add(item);
                    }
                    records = resultRecords;

                    var fixResult = RecountTotal(campaignId, resultRecords);

                    result.Total = fixResult.Total;
                    result.Count = fixResult.Count;
                }

                LoadPolygon(submapPolygon, coordinates, holes);
            }
            #region remove auto merge logical
            //else if (isAddAllShapesToSubMap && unionPolygon.NumGeometries > 1)
            //{
            //    //first add area to submap. try to find the the biger area as submap
            //    if (submapPolygon == null)
            //    {
            //        Polygon bestFitPolygon = null;
            //        for (var i = 0; i < unionPolygon.NumGeometries; i++)
            //        {
            //            var polygon = unionPolygon.GetGeometryN(i) as Polygon;
            //            submapPolygon = bestFitPolygon == null || bestFitPolygon.Area < polygon.Area ? polygon : bestFitPolygon;
            //        }
            //    }
            //    else //get new merged submap
            //    {
            //        for (var i = 0; i < unionPolygon.NumGeometries; i++)
            //        {
            //            var polygon = unionPolygon.GetGeometryN(i) as Polygon;
            //            if (submapPolygon.Within(polygon))
            //            {
            //                submapPolygon = polygon;
            //                break;
            //            }
            //        }
            //    }

            //    HashSet<int> needRemoveRecord = new HashSet<int>();
            //    //remove the records in holes
            //    foreach(var shapes in allPolygon)
            //    {
            //        foreach (var hole in submapPolygon.Holes)
            //        {
            //            if (shapes.Key.Within(hole))
            //            {
            //                if (!needRemoveRecord.Contains(shapes.Value))
            //                {
            //                    needRemoveRecord.Add(shapes.Value);
            //                }
            //            }
            //        }
            //    }
            //    //remove the records not in merged submap 
            //    for (var i = 0; i < unionPolygon.NumGeometries; i++)
            //    {
            //        var polygon = unionPolygon.GetGeometryN(i) as Polygon;
            //        if (!submapPolygon.Within(polygon))
            //        {
            //            foreach (var shapes in allPolygon)
            //            {
            //                if (shapes.Key.Within(polygon))
            //                {
            //                    if (!needRemoveRecord.Contains(shapes.Value))
            //                    {
            //                        needRemoveRecord.Add(shapes.Value);
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    if (needRemoveRecord.Count > 0)
            //    {
            //        List<AreaRecord> resultRecords = new List<AreaRecord>();
            //        foreach (var item in records)
            //        {
            //            if (needRemoveRecord.Contains(item.AreaId))
            //            {
            //                continue;
            //            }
            //            resultRecords.Add(item);
            //        }
            //        records = resultRecords;

            //        var fixResult = RecountTotal(campaignId, resultRecords);

            //        result.Total = fixResult.Total;
            //        result.Count = fixResult.Count;
            //    }

            //    LoadPolygon(submapPolygon, coordinates, holes);
            //}
            #endregion
            else//this means merge failed. keep the old logical return more than one shapes.  
            {
                for (var i = 0; i < unionPolygon.NumGeometries; i++)
                {
                    var polygon = unionPolygon.GetGeometryN(i) as Polygon;
                    LoadPolygon(polygon, coordinates, holes);
                }
            }

            result.Locs = coordinates;
            result.Holes = holes;

            return result;
        }

        public IEnumerable<AreaRecord> FilterNotIncludedInSubMapRecord(int campaignId, int submapId, IEnumerable<AreaRecord> records)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign == null)
                {
                    throw new Exception("bad campaignId");
                }

                var subMap = campaign.SubMaps.FirstOrDefault(i => i.Id == submapId);
                if (subMap == null || subMap.SubMapRecords == null || subMap.SubMapRecords.Count == 0)
                {
                    throw new Exception("bad submapId");
                }
                if (subMap.SubMapRecords.Count == 0)
                {
                    throw new Exception("bad submap");
                }
                //filter records. dmap records must under submap and have same Classifications
                var currentSubMapClassifications = subMap.SubMapRecords.First().Classification;
                //var subMapAreaId = subMap.SubMapRecords.ToDictionary(i=>i.AreaId);

                var validRecords = new List<AreaRecord>();
                foreach (var record in records)
                {
                    
                    if (record.Classification != currentSubMapClassifications)
                    {
                        continue;
                    }
                    //filter records. dmap records must under submap and have same Classifications
                    bool inSubMap = subMap.SubMapRecords.Any(i => i.AreaId == record.AreaId && i.Classification == record.Classification);
                    if (!inSubMap)
                    {
                        continue;
                    }
                    // filter records. dmap records must not used by other dmaps.
                    bool needAdd = true;
                    foreach (var currentDmap in subMap.DistributionMaps)
                    {
                        bool inOhterDmap = currentDmap.DistributionMapRecords.Any(i => i.AreaId == record.AreaId && i.Classification == record.Classification && i.Value == true);
                        if (inOhterDmap)
                        {
                            needAdd = false;
                            break;
                        }
                    }
                    if(needAdd)
                    {
                        validRecords.Add(record);
                    }
                }
                

                return validRecords as IEnumerable<AreaRecord>;
            }
        }
        private MergeResult RecountTotal(int campaignId, IEnumerable<AreaRecord> records)
        {
            //recount total
            Classifications c = Classifications.Z3;
            foreach (AreaRecord record in records)
            {
                if (record.Value)
                {
                    c = record.Classification;
                    break;
                }
            }
            MergeResult result = null;
            switch(c)
            {
                case Classifications.Z5:
                    result = GetSoureForFiveZipMapping(campaignId, records);
                    break;
                case Classifications.PremiumCRoute:
                    result = GetSoureForCRouteMapping(campaignId, records);
                    break;
                case Classifications.TRK:
                    result = GetSoureForTractMapping(campaignId, records);
                    break;
                case Classifications.BG:
                    result = GetSoureForBlockGroupMapping(campaignId, records);
                    break;
                default:
                    break;
            }
            return result;
        }

        private void LoadPolygon(Polygon polygon, List<double[][]> coordinates, List<double[][]> holes)
        {
            if (polygon != null)
            {
                var locations = polygon.ExteriorRing.Coordinates;
                double[][] boundary = new double[locations.Length][];
                for (int j = 0; j < boundary.Length; j++)
                {
                    boundary[j] = new double[] { locations[j].Y, locations[j].X };
                }
                coordinates.Add(boundary);

                if (polygon.Holes.Length > 0)
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
        }

        [Obsolete("replaced by GeoAPI, please use MergeAreasAPI method")]
        public MergeResult MergeAreas(int campaignId, ref IEnumerable<AreaRecord> records)
        {
            Classifications c = Classifications.Z3;

            foreach (AreaRecord record in records)
            {
                if (record.Value)
                {
                    c = record.Classification;
                    break;
                }
            }
            MergeResult result = null;
            if (c == Classifications.Z5 )
            {
                result = GetSoureForFiveZipMapping(campaignId, records);
            }
            else if (c == Classifications.PremiumCRoute)
            {
                result = GetSoureForCRouteMapping(campaignId, records);
            }
            else if(c == Classifications.TRK)
            {
                result = GetSoureForTractMapping(campaignId, records);
            }
            else if (c == Classifications.BG)
            {
                result = GetSoureForBlockGroupMapping(campaignId, records);
            }

            if (result.Locs.Count > 0)
            {
                List<double[][]> coordinates = new List<double[][]>();
                List<List<GPS.Utilities.Boundary.Types.Loc>> locs = GPS.Utilities.Boundary.BoundaryFinder.find2(result.Locs);
                foreach (List<GPS.Utilities.Boundary.Types.Loc> loc in locs)
                {
                    var size = loc.Count;
                    double[][] ps = new double[size][];
                    for (int i = 0; i < size; i++)
                    {
                        ps[i] = new double[2] { loc[i].getX(), loc[i].getY() };
                    }
                    coordinates.Add(ps);
                }
                result.Locs = coordinates;

            }

            return result;
        }

        private MergeResult GetSoureForFiveZipMapping(int campaignId, IEnumerable<AreaRecord> records)
        {
            MergeResult result = new MergeResult();
            List<double[][]> pss = result.Locs = new List<double[][]>();
            result.RecordLocations = new Dictionary<int, double[][]>();
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    Dictionary<Classifications, List<Int32>> areaIds = GetAreaIds(records, true);
                    Dictionary<Classifications, List<Int32>> nonIds = GetAreaIds(records, false);
                    List<Int32> fiveZipIds = areaIds.ContainsKey(Classifications.Z5) ? areaIds[Classifications.Z5] : null;
                    List<Int32> nonFiveZipIds = new List<int>();
                    List<Int32> cRouteIds = areaIds.ContainsKey(Classifications.PremiumCRoute) ? areaIds[Classifications.PremiumCRoute] : null;
                    List<Int32> nonCRouteIds = nonIds.ContainsKey(Classifications.PremiumCRoute) ? nonIds[Classifications.PremiumCRoute] : null;

                    FiveZipsSatisfyingSubMapSpecification fSpec = new FiveZipsSatisfyingSubMapSpecification(fiveZipIds, cRouteIds, nonCRouteIds);
                    IEnumerable<FiveZipArea> fiveZips = fSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.FiveZipRepository);
                    AppendFiveZipData(ref result, ref nonFiveZipIds, fiveZips, campaign);

                    //PremiumCRoutesSatisfyingSubMapSpecification cSpec = new PremiumCRoutesSatisfyingSubMapSpecification(
                    //    fiveZipIds, nonFiveZipIds, cRouteIds, nonCRouteIds);
                    //IEnumerable<PremiumCRoute> cRoutes = cSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.PremiumCRouteRepository);
                    //AppendCRouteData(ref result, cRoutes, campaign);

                    IEnumerable<PremiumCRoute> cRoutes = new List<PremiumCRoute>();

                    if (fiveZips.Count() == 1 && cRoutes.Count() == 0 && pss.Count == 0)
                    {
                        foreach (FiveZipArea fiveZip in fiveZips)
                        {
                            int size = fiveZip.FiveZipAreaCoordinates.Count;
                            var coordinates = fiveZip.FiveZipAreaCoordinates.OrderBy(i => i.Id).ToList();
                            double[][] ps = new double[size][];
                            for (int i = 0; i < size; i++)
                            {
                                ps[i] = new double[] { coordinates[i].Latitude, coordinates[i].Longitude };
                            }
                            pss.Add(ps);
                            result.RecordLocations.Add(fiveZip.Id, ps);
                            break;
                        }
                    }
                    //else if (fiveZips.Count() == 0 && cRoutes.Count() == 1 && pss.Count == 0)
                    //{
                    //    foreach (PremiumCRoute cRoute in cRoutes)
                    //    {
                    //        int size = cRoute.PremiumCRouteCoordinates.Count;
                    //        double[][] ps = new double[size][];
                    //        for (int i = 0; i < size; i++)
                    //        {
                    //            ps[i] = new double[] { cRoute.PremiumCRouteCoordinates[i].Latitude, cRoute.PremiumCRouteCoordinates[i].Longitude };
                    //        }
                    //        pss.Add(ps);
                    //        break;
                    //    }
                    //}
                }

            }

            return result;
        }

        private MergeResult GetSoureForCRouteMapping(int campaignId, IEnumerable<AreaRecord> records)
        {
            MergeResult result = new MergeResult();
            List<double[][]> pss = result.Locs = new List<double[][]>();
            result.RecordLocations = new Dictionary<int, double[][]>();
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    Dictionary<Classifications, List<Int32>> areaIds = GetAreaIds(records, true);
                    Dictionary<Classifications, List<Int32>> nonIds = GetAreaIds(records, false);
                    List<Int32> fiveZipIds = areaIds.ContainsKey(Classifications.Z5) ? areaIds[Classifications.Z5] : null;
                    List<Int32> nonFiveZipIds = new List<int>();
                    List<Int32> cRouteIds = areaIds.ContainsKey(Classifications.PremiumCRoute) ? areaIds[Classifications.PremiumCRoute] : null;
                    List<Int32> nonCRouteIds = nonIds.ContainsKey(Classifications.PremiumCRoute) ? nonIds[Classifications.PremiumCRoute] : null;

                    //FiveZipsSatisfyingSubMapSpecification fSpec = new FiveZipsSatisfyingSubMapSpecification(fiveZipIds, cRouteIds, nonCRouteIds);
                    //IEnumerable<FiveZipArea> fiveZips = fSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.FiveZipRepository);
                    //AppendFiveZipData(ref result, ref nonFiveZipIds, fiveZips, campaign);

                    IEnumerable<FiveZipArea> fiveZips = new List<FiveZipArea>();

                    PremiumCRoutesSatisfyingSubMapSpecification cSpec = new PremiumCRoutesSatisfyingSubMapSpecification(
                        fiveZipIds, nonFiveZipIds, cRouteIds, nonCRouteIds);
                    IEnumerable<PremiumCRoute> cRoutes = cSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.PremiumCRouteRepository);
                    AppendCRouteData(ref result, cRoutes, campaign);

                    //if (fiveZips.Count() == 1 && cRoutes.Count() == 0 && pss.Count == 0)
                    //{
                    //    foreach (FiveZipArea fiveZip in fiveZips)
                    //    {
                    //        int size = fiveZip.FiveZipAreaCoordinates.Count;
                    //        double[][] ps = new double[size][];
                    //        for (int i = 0; i < size; i++)
                    //        {
                    //            ps[i] = new double[] { fiveZip.FiveZipAreaCoordinates[i].Latitude, fiveZip.FiveZipAreaCoordinates[i].Longitude };
                    //        }
                    //        pss.Add(ps);
                    //        break;
                    //    }
                    //}
                    //else 
                    if (fiveZips.Count() == 0 && cRoutes.Count() == 1 && pss.Count == 0)
                    {
                        foreach (PremiumCRoute cRoute in cRoutes)
                        {
                            int size = cRoute.PremiumCRouteCoordinates.Count;
                            var coordinates = cRoute.PremiumCRouteCoordinates.OrderBy(i => i.Id).ToList();
                            double[][] ps = new double[size][];
                            for (int i = 0; i < size; i++)
                            {
                                ps[i] = new double[] { coordinates[i].Latitude, coordinates[i].Longitude };
                            }
                            pss.Add(ps);
                            result.RecordLocations.Add(cRoute.Id, ps);
                            break;
                        }
                    }
                }

            }

            return result;
        }

        private MergeResult GetSoureForTractMapping(int campaignId, IEnumerable<AreaRecord> records)
        {
            MergeResult result = new MergeResult();
            List<double[][]> pss = result.Locs = new List<double[][]>();
            result.RecordLocations = new Dictionary<int, double[][]>();
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    Dictionary<Classifications, List<Int32>> areaIds = GetAreaIds(records, true);
                    Dictionary<Classifications, List<Int32>> nonIds = GetAreaIds(records, false);
                    List<Int32> tractIds = areaIds.ContainsKey(Classifications.TRK) ? areaIds[Classifications.TRK] : null;
                    List<Int32> nonTractIds = new List<int>();
                    List<Int32> bgIds = areaIds.ContainsKey(Classifications.BG) ? areaIds[Classifications.BG] : null;
                    List<Int32> nonBgIds = nonIds.ContainsKey(Classifications.BG) ? nonIds[Classifications.BG] : null;

                    TractsSatisfyingSubMapSpecification tSpec = new TractsSatisfyingSubMapSpecification(tractIds, bgIds, nonBgIds);
                    IEnumerable<Tract> tracts = tSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.TractRepository);
                    AppendTractData(ref result, ref nonTractIds, tracts, campaign);

                    //BlockGroupsSatisfyingSubMapSpecification cSpec = new BlockGroupsSatisfyingSubMapSpecification(
                    //    tractIds, nonTractIds, bgIds, nonBgIds);
                    //IEnumerable<BlockGroup> bgs = cSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.BlockGroupRepository);
                    //AppendBlockGroupData(ref result, bgs, campaign);
                }
            }
            return result;
        }

        private MergeResult GetSoureForBlockGroupMapping(int campaignId, IEnumerable<AreaRecord> records)
        {
            MergeResult result = new MergeResult();
            List<double[][]> pss = result.Locs = new List<double[][]>();
            result.RecordLocations = new Dictionary<int, double[][]>();
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    Dictionary<Classifications, List<Int32>> areaIds = GetAreaIds(records, true);
                    Dictionary<Classifications, List<Int32>> nonIds = GetAreaIds(records, false);
                    List<Int32> tractIds = areaIds.ContainsKey(Classifications.TRK) ? areaIds[Classifications.TRK] : null;
                    List<Int32> nonTractIds = new List<int>();
                    List<Int32> bgIds = areaIds.ContainsKey(Classifications.BG) ? areaIds[Classifications.BG] : null;
                    List<Int32> nonBgIds = nonIds.ContainsKey(Classifications.BG) ? nonIds[Classifications.BG] : null;

                    //TractsSatisfyingSubMapSpecification tSpec = new TractsSatisfyingSubMapSpecification(tractIds, bgIds, nonBgIds);
                    //IEnumerable<Tract> tracts = tSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.TractRepository);
                    //AppendTractData(ref result, ref nonTractIds, tracts, campaign);

                    BlockGroupsSatisfyingSubMapSpecification cSpec = new BlockGroupsSatisfyingSubMapSpecification(
                        tractIds, nonTractIds, bgIds, nonBgIds);
                    IEnumerable<BlockGroup> bgs = cSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.BlockGroupRepository);
                    AppendBlockGroupData(ref result, bgs, campaign);
                }
            }
            return result;
        }

        private void AppendFiveZipData(ref MergeResult result, ref List<Int32> nonFiveZipIds, IEnumerable<FiveZipArea> fiveZips, Campaign campaign)
        {
            foreach (FiveZipArea fiveZip in fiveZips)
            {
                var areaData = campaign.CampaignFiveZipImporteds.Where(t => t.FiveZipArea.Id == fiveZip.Id).SingleOrDefault();
                //load shape coordinates
                int size = fiveZip.FiveZipAreaCoordinates.Count;
                var coordinates = fiveZip.FiveZipAreaCoordinates.OrderBy(i => i.Id).ToList();
                double[][] ps = new double[size][];
                for (int i = 0; i < size; i++)
                {
                    ps[i] = new double[] { coordinates[i].Latitude, coordinates[i].Longitude };
                }
                result.Locs.Add(ps);
                nonFiveZipIds.Add(fiveZip.Id);
                result.RecordLocations.Add(fiveZip.Id, ps);


                if (fiveZip.IsEnabled)
                {
                    if (areaData != null)
                    {
                        result.Total += (areaData.PartPercentage == 0) ? areaData.Total : areaData.Total * areaData.PartPercentage;
                        result.Count += (areaData.PartPercentage == 0) ? areaData.Penetration : areaData.Penetration * areaData.PartPercentage;
                    }
                    else
                    {
                        switch (campaign.AreaDescription)
                        {
                            case "APT ONLY":
                                result.Total += fiveZip.APT_COUNT;
                                break;
                            case "HOME ONLY":
                                result.Total += fiveZip.HOME_COUNT;
                                break;
                            case "APT + HOME":
                            default:
                                result.Total += fiveZip.HOME_COUNT + fiveZip.APT_COUNT;
                                break;
                        }
                    }
                }                
            }
            result.Total = Math.Round(result.Total, 0);
            result.Count = Math.Round(result.Count, 0);
        }

        private void AppendCRouteData(ref MergeResult result, IEnumerable<PremiumCRoute> cRoutes, Campaign campaign)
        {
            foreach (PremiumCRoute cRoute in cRoutes)
            {
                var areaData = campaign.CampaignCRouteImporteds.Where(t => t.PremiumCRoute.Id == cRoute.Id).SingleOrDefault();
                //load shape coordinates
                int size = cRoute.PremiumCRouteCoordinates.Count;
                var coordinates = cRoute.PremiumCRouteCoordinates.OrderBy(i => i.Id).ToList();
                double[][] ps = new double[size][];
                for (int i = 0; i < size; i++)
                {
                    ps[i] = new double[] { coordinates[i].Latitude, coordinates[i].Longitude };
                }
                result.Locs.Add(ps);
                result.RecordLocations.Add(cRoute.Id, ps);

                if (cRoute.IsEnabled)
                {
                    if (areaData != null)
                    {
                        result.Total += (areaData.PartPercentage == 0) ? areaData.Total : areaData.Total * areaData.PartPercentage;
                        result.Count += (areaData.PartPercentage == 0) ? areaData.Penetration : areaData.Penetration * areaData.PartPercentage;
                    }
                    else
                    {
                        switch (campaign.AreaDescription)
                        {
                            case "APT ONLY":
                                result.Total += cRoute.APT_COUNT;
                                break;
                            case "HOME ONLY":
                                result.Total += cRoute.HOME_COUNT;
                                break;
                            case "APT + HOME":
                            default:
                                result.Total += cRoute.HOME_COUNT + cRoute.APT_COUNT;
                                break;
                        }
                    }
                }

            }
            result.Total = Math.Round(result.Total, 0);
            result.Count = Math.Round(result.Count, 0);
        }

        private void AppendTractData(ref MergeResult result, ref List<Int32> nonTractIds, IEnumerable<Tract> tracts, Campaign campaign)
        {
            foreach (Tract tract in tracts)
            {
                var areaData = campaign.CampaignTractImporteds.Where(t => t.Tract.Id == tract.Id).SingleOrDefault();
                if (areaData != null)
                {
                    if (tract.IsEnabled)
                    {
                        result.Total += areaData.Total;
                        result.Count += areaData.Penetration;
                    }
                }

                int size = tract.TractCoordinates.Count;
                var coordinates = tract.TractCoordinates.OrderBy(i => i.Id).ToList();
                double[][] ps = new double[size][];
                for (int i = 0; i < size; i++)
                {
                    ps[i] = new double[] { coordinates[i].Latitude, coordinates[i].Longitude };
                }
                result.Locs.Add(ps);
                result.RecordLocations.Add(tract.Id, ps);
                nonTractIds.Add(tract.Id);

            }
        }

        private void AppendBlockGroupData(ref MergeResult result, IEnumerable<BlockGroup> bgs, Campaign campaign)
        {
            foreach (BlockGroup bg in bgs)
            {
                var areaData = campaign.CampaignBlockGroupImporteds.Where(t => t.BlockGroup.Id == bg.Id).SingleOrDefault();
                if (areaData != null && bg.IsEnabled)
                {
                    result.Total += areaData.Total;
                    result.Count += areaData.Penetration;
                }

                int size = bg.BlockGroupCoordinates.Count;
                var coordinates = bg.BlockGroupCoordinates.OrderBy(i => i.Id).ToList();
                double[][] ps = new double[size][];
                for (int i = 0; i < size; i++)
                {
                    ps[i] = new double[] { coordinates[i].Latitude, coordinates[i].Longitude };
                }
                result.Locs.Add(ps);
                result.RecordLocations.Add(bg.Id, ps);
            }
        }

        private Dictionary<Classifications, List<Int32>> GetAreaIds(IEnumerable<AreaRecord> records, bool value)
        {
            Dictionary<Classifications, List<Int32>> cIds = new Dictionary<Classifications, List<int>>();
            foreach (AreaRecord record in records)
            {
                if (record.Value == value)
                {
                    if (!cIds.ContainsKey(record.Classification))
                    {
                        cIds.Add(record.Classification, new List<int>());
                    }
                    cIds[record.Classification].Add(record.AreaId);
                }
            }
            return cIds;
        }

    }

    public class MergeResult
    {
        private double _total;
        private double _count;
        private List<double[][]> _locs;
        private List<double[][]> _Holes;
        private Dictionary<int, double[][]> _RecordLocations;
        private bool _HaveNotMergedArea;

        public double Total
        {
            get { return _total; }
            set { _total = value; }
        }

        public double Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public List<double[][]> Locs
        {
            get { return _locs; }
            set { _locs = value; }
        }

        public List<double[][]> Holes
        {
            get { return _Holes; }
            set { _Holes = value; }
        }

        public Dictionary<int, double[][]> RecordLocations
        {
            get { return _RecordLocations; }
            set { _RecordLocations = value; }
        }

        public bool HaveNotMergedArea
        {
            get { return _HaveNotMergedArea; }
            set { _HaveNotMergedArea = value; }
        }

        public List<AreaRecord> Areas { get; set; }
    }
}
