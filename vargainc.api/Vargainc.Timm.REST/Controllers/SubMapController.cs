using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Z.EntityFramework.Plus;

using Vargainc.Timm.Extentions;
using Vargainc.Timm.Models;
using Vargainc.Timm.REST.ViewModel;


namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("campaign")]
    public class SubMapController : BaseController
    {
        [HttpGet]
        [Route("{campaignId:int}/submap/{submapId:int}")]
        public IHttpActionResult GetSubMaps(int campaignId, int submapId)
        {
            var submap = db.SubMaps.FirstOrDefault(i => i.Id == submapId);
            if(submap == null)
            {
                return NotFound();
            }
            ConcurrentDictionary<int, List<AbstractArea>> submapLocations = new ConcurrentDictionary<int, List<AbstractArea>>();

            Parallel.ForEach(submap.SubMapRecords, i => {
                IQueryable query = null;
                switch (i.Classification)
                {
                    case 1:
                        query = db.FiveZipAreas.Where(s => s.Id == i.AreaId);
                        break;
                    case 2:
                        query = db.Tracts.Where(s => s.Id == i.AreaId);
                        break;
                    case 3:
                        query = db.BlockGroups.Where(s => s.Id == i.AreaId);
                        break;
                    case 15:
                        query = db.PremiumCRoutes.Where(s => s.Id == i.AreaId);
                        break;
                    default:
                        break;
                }
                
            });

            return NotFound();
        }

        [HttpGet]
        [Route("{campaignId:int}/submap/geojson")]
        public async Task<IHttpActionResult> GetSubmapGeoJson(int campaignId)
        {
            var subMaps = await db.SubMaps.Where(i=>i.CampaignId == campaignId).ToListAsync().ConfigureAwait(false);
            var layers = new FeatureCollection();
            foreach (var subMap in subMaps)
            {
                var points = subMap.SubMapCoordinates.OrderBy(i=>i.Id).Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToList();
                if(points.Count == 0)
                {
                    continue;
                }
                // close it
                if (!points.First().Equals2D(points.Last()))
                {
                    points.Add(points.First());
                }
                var polygon = GeometryFactory.CreatePolygon(points.ToArray());
                polygon.Normalize();

                if(polygon == null)
                {
                    continue;
                }

                layers.Add(new Feature { 
                    Geometry = polygon,
                    BoundingBox = polygon.EnvelopeInternal,
                    Attributes = new AttributesTable
                    {
                        { "type", "submap" },
                        { "id", $"submap-{subMap.Id}" },
                        { "oid", subMap.Id },
                        { "sid", subMap.Id },
                        { "name", subMap.Name },
                        { "color", $"#{subMap.ColorString}" },
                    }
                });
            }

            

            var eTag = GetETag();
            return new GeoJsonResponse(layers, eTag);
        }

        [HttpPost]
        [Route("{campaignId:int}/submap/edit")]
        public async Task<IHttpActionResult> EditSubmap(int campaignId, [FromBody] SubMap data)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId).ConfigureAwait(false);
            if (campaign == null)
            {
                throw new Exception("campaigin not exists!");
            }
            var orderId = (campaign.SubMaps.Max(i => i.OrderId) ?? 0) + 1;

            SubMap dbSubmap = null;
            if (data.Id.HasValue)
            {
                dbSubmap = await db.SubMaps.FindAsync(data.Id);
            }

            if(dbSubmap != null)
            {
                dbSubmap.Name = data.Name;
                dbSubmap.ColorString = data.ColorString.Replace("#", "");
                dbSubmap.ColorR = data.ColorR;
                dbSubmap.ColorG = data.ColorG;
                dbSubmap.ColorB = data.ColorB;
                dbSubmap.TotalAdjustment = data.TotalAdjustment ?? 0;
                dbSubmap.CountAdjustment = data.CountAdjustment ?? 0;
            }
            else
            {
                dbSubmap = new SubMap()
                {
                    Campaign = campaign,
                    CampaignId = campaignId,
                    ColorString = data.ColorString.Replace("#", ""),
                    ColorR = data.ColorR,
                    ColorG = data.ColorG,
                    ColorB = data.ColorB,
                    Name = data.Name,
                    OrderId = orderId,
                    Total = 0,
                    Penetration = 0,
                    Percentage = 0,
                    TotalAdjustment = 0,
                    CountAdjustment = 0,
                };

                campaign.SubMaps.Add(dbSubmap);
            }

            await db.SaveChangesAsync().ConfigureAwait(false);

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("{campaignId:int}/submap/{submapId:int}/merge")]
        public async Task<IHttpActionResult> MergeAreas(int campaignId, int submapId, [FromBody] List<AreaRecord> records)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId).ConfigureAwait(false);
            var submap = await db.SubMaps.Where(i => i.Id == submapId && i.CampaignId == campaignId).FirstOrDefaultAsync().ConfigureAwait(false);
            var otherSubMaps = await db.SubMaps.Where(i => i.CampaignId == campaignId && i.Id != submapId).Select(i => i.Id).ToListAsync().ConfigureAwait(false);
            if (campaign == null || submap == null)
            {
                return BadRequest();
            }

            var targetClassification = records.First().Classification;

            var subMapRecords = submap.SubMapRecords
                .Select(i => i.AreaId ?? 0)
                .ToList();

            var removeAreas = records.Where(i => i.Value == false).Select(i => i.Name).ToList();
            var addAreas = records.Where(i => i.Value == true).Select(i => i.Name).ToList();
            List<int> needRemoveRecords = new List<int>();
            List<int> needAddRecords = new List<int>();
            switch (targetClassification)
            {
                case Classifications.Z3:
                    needRemoveRecords = await db.ThreeZipAreas.Where(i => removeAreas.Contains(i.Code)).Select(i => i.Id ?? 0).Distinct().ToListAsync().ConfigureAwait(false);
                    needAddRecords = await db.ThreeZipAreas.Where(i => addAreas.Contains(i.Code)).Select(i => i.Id ?? 0).ToListAsync().ConfigureAwait(false);
                    break;
                case Classifications.Z5:
                    needRemoveRecords = await db.FiveZipAreas.Where(i => removeAreas.Contains(i.Code)).Select(i => i.Id ?? 0).ToListAsync().ConfigureAwait(false);
                    needAddRecords = await db.FiveZipAreas.Where(i => addAreas.Contains(i.Code)).Select(i => i.Id ?? 0).ToListAsync().ConfigureAwait(false);
                    break;
                case Classifications.PremiumCRoute:
                    needRemoveRecords = await db.PremiumCRoutes.Where(i => removeAreas.Contains(i.GEOCODE)).Select(i => i.Id ?? 0).ToListAsync().ConfigureAwait(false);
                    needAddRecords = await db.PremiumCRoutes.Where(i => addAreas.Contains(i.GEOCODE)).Select(i => i.Id ?? 0).ToListAsync().ConfigureAwait(false);
                    break;
            }
            if (needRemoveRecords.Count == 0 && needAddRecords.Count == 0)
            {
                return BadRequest();
            }

            var needRemoveRecordsSet = needRemoveRecords.ToHashSet();
            var newRecords = subMapRecords.Where(i => !needRemoveRecordsSet.Contains(i)).ToList();
            newRecords.AddRange(needAddRecords);

            // remove other submaps already added area
            var targetClassificationInt = (int)targetClassification;
            var otherAddedAreas = db.SubMapRecords.Where(i => otherSubMaps.Contains(i.SubMapId) && i.Classification == targetClassificationInt).Select(i => i.Id ?? 0).ToHashSet();

            newRecords = newRecords.Where(i => !otherAddedAreas.Contains(i)).ToList();

            List<AreaRecordRow> areaGeoms = null;
            switch (targetClassification)
            {
                case Classifications.Z3:
                    areaGeoms = db.ThreeZipCoordinates
                        .Where(i => newRecords.Contains(i.ThreeZipAreaId.Value))
                        .OrderBy(i => i.ThreeZipAreaId)
                        .ThenBy(i => i.Id)
                        .Select(i => new { i.ThreeZipAreaId, i.Longitude, i.Latitude })
                        .AsEnumerable()
                        .Select(i => new AreaRecordRow { Id = i.ThreeZipAreaId, Coordinate = new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0) })
                        .ToList();
                    break;
                case Classifications.Z5:
                    areaGeoms = db.FiveZipCoordinates
                        .Where(i => newRecords.Contains(i.FiveZipAreaId.Value))
                        .OrderBy(i => i.FiveZipAreaId)
                        .ThenBy(i => i.Id)
                        .Select(i => new { i.FiveZipAreaId, i.Longitude, i.Latitude })
                        .AsEnumerable()
                        .Select(i => new AreaRecordRow { Id = i.FiveZipAreaId, Coordinate = new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0) })
                        .ToList();
                    break;
                case Classifications.PremiumCRoute:
                    areaGeoms = db.PremiumCRouteCoordinates
                        .Where(i => newRecords.Contains(i.PreminumCRouteId.Value))
                        .OrderBy(i => i.PreminumCRouteId)
                        .ThenBy(i => i.Id)
                        .Select(i => new { i.PreminumCRouteId, i.Longitude, i.Latitude })
                        .AsEnumerable()
                        .Select(i => new AreaRecordRow { Id = i.PreminumCRouteId, Coordinate = new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0) })
                        .ToList();
                    break;
            }

            var data = areaGeoms.GroupBy(i => i.Id, o => o.Coordinate).ToDictionary(i => i.Key, o => o.ToArray());
            Geometry mergeGeom = null;
            Dictionary<int?, Geometry> areaGeometry = new Dictionary<int?, Geometry>();
            foreach (var item in data)
            {
                Geometry polygon = GeometryFactory.CreatePolygon(item.Value);
                if (!polygon.IsValid)
                {
                    polygon = polygon.Buffer(0);
                }
                polygon.Normalize();
                areaGeometry.Add(item.Key, polygon);
                if (mergeGeom == null)
                {
                    mergeGeom = polygon;
                }
                else
                {
                    mergeGeom = mergeGeom.Union(polygon);
                }
            }

            Polygon mergedPolygon = GeometryFactory.CreatePolygon();

            if (mergeGeom != null)
            {
                // get max area polygon when merged have more than 1 polygons
                if (mergeGeom.NumGeometries > 1)
                {
                    Polygon maxArea = null;
                    for (var i = 0; i < mergeGeom.NumGeometries; i++)
                    {
                        if (mergeGeom.GetGeometryN(i).GeometryType == "Polygon")
                        {
                            Polygon p = mergeGeom.GetGeometryN(i) as Polygon;
                            if (maxArea == null || p.Area > maxArea.Area)
                            {
                                maxArea = p;
                            }
                        }
                    }

                    mergedPolygon = maxArea;

                    foreach (var item in areaGeometry)
                    {
                        if (!maxArea.Contains(item.Value))
                        {
                            newRecords.Remove(item.Key.Value);
                        }
                    }
                }
                else
                {
                    mergedPolygon = mergeGeom as Polygon;
                    if (mergedPolygon == null)
                    {
                        throw new Exception("merge failed. the merge area is not polygon");
                    }
                }
            }

            int apt = 0;
            int home = 0;

            Dictionary<int?, string> cache = new Dictionary<int?, string>();


            switch (targetClassification)
            {
                case Classifications.Z5:
                    var resultZ5 = db.FiveZipAreas.Where(i => newRecords.Contains(i.Id.Value))
                        .GroupBy(i => i.Code)
                        .Select(g => new { APT = g.Max(i => i.APT_COUNT), HOME = g.Max(i => i.HOME_COUNT) })
                        .ToList();
                    cache = db.FiveZipAreas.Where(i => newRecords.Contains(i.Id.Value)).Select(i => new { i.Id, i.Code }).ToDictionary(i => i.Id, i => i.Code);
                    apt = resultZ5.Sum(i => i.APT ?? 0);
                    home = resultZ5.Sum(i => i.HOME ?? 0);

                    break;
                case Classifications.PremiumCRoute:
                    var resultCRoute = db.PremiumCRoutes.Where(i => newRecords.Contains(i.Id.Value))
                        .GroupBy(i => i.GEOCODE)
                        .Select(g => new { APT = g.Max(i => i.APT_COUNT), HOME = g.Max(i => i.HOME_COUNT) })
                        .ToList();
                    cache = db.PremiumCRoutes.Where(i => newRecords.Contains(i.Id.Value)).Select(i => new { i.Id, i.GEOCODE }).ToDictionary(i => i.Id, i => i.GEOCODE);
                    apt = resultCRoute.Sum(i => i.APT ?? 0);
                    home = resultCRoute.Sum(i => i.HOME ?? 0);

                    break;
            }
            int total = 0;
            switch (campaign.AreaDescription)
            {
                case "APT + HOME":
                    total = apt + home;
                    break;
                case "APT ONLY":
                    total = apt;
                    break;
                case "HOME ONLY":
                    total = home;
                    break;
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                await db.SubMapRecords.Where(i => i.SubMapId == submapId).DeleteAsync();
                var toAddRecords = newRecords.Select(record => new SubMapRecord
                {
                    Classification = (int)targetClassification,
                    SubMapId = submapId,
                    AreaId = record,
                    Value = true,
                    Code = cache[record]
                });
                db.SubMapRecords.AddRange(toAddRecords);

                await db.SubMapCoordinates.Where(i => i.SubMapId == submapId).DeleteAsync();
                var toAddCoordinates = mergedPolygon.Coordinates.Select(coordinate => new SubMapCoordinate
                {
                    SubMapId = submapId,
                    Longitude = coordinate.X,
                    Latitude = coordinate.Y,
                });
                db.SubMapCoordinates.AddRange(toAddCoordinates);

                submap.Total = total;
                await db.SaveChangesAsync();

                transaction.Commit();
            }

            return Json(new { sucess = true });
        }

        [HttpDelete]
        [Route("{campaignId:int}/submap/{submapId:int}/delete")]
        public async Task<IHttpActionResult> DeleteSubmap(int campaignId, int submapId)
        {
            var submap = await db.SubMaps.FindAsync(submapId).ConfigureAwait(false);
            if (submap == null)
            {
                throw new Exception("submap not exists!");
            }

            using (var tran = db.Database.BeginTransaction())
            {
                await db.SubMapRecords.Where(i=>i.SubMapId == submapId).DeleteAsync().ConfigureAwait(false);
                await db.SubMapCoordinates.Where(i => i.SubMapId == submapId).DeleteAsync().ConfigureAwait(false);
                await db.SubMaps.Where(i => i.Id == submapId).DeleteAsync().ConfigureAwait(false);

                tran.Commit();
            }

            return Json(new { success = true });
        }

        [HttpDelete]
        [Route("{campaignId:int}/submap/{submapId:int}/empty")]
        public async Task<IHttpActionResult> EmptySubmap(int campaignId, int submapId)
        {
            var submap = await db.SubMaps.FindAsync(submapId).ConfigureAwait(false);
            if(submap == null)
            {
                throw new Exception("submap not exists!");
            }

            using (var tran = db.Database.BeginTransaction())
            {
                await db.SubMapRecords.Where(i => i.SubMapId == submapId).DeleteAsync().ConfigureAwait(false);
                await db.SubMapCoordinates.Where(i => i.SubMapId == submapId).DeleteAsync().ConfigureAwait(false);
                
                submap.Total = 0;
                db.SaveChanges();

                tran.Commit();
            }

            return Json(new { success = true });
        }
    }
}
