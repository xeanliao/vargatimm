using Microsoft.SqlServer.Types;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Vargainc.Timm.Extentions;
using Vargainc.Timm.Models;
using Vargainc.Timm.REST.Helper;
using Vargainc.Timm.REST.ViewModel;
using Z.EntityFramework.Plus;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("campaign")]
    public class DMapController : BaseController
    {
        [HttpGet]
        [Route("{campaignId}/dmap/geojson")]
        public async Task<IHttpActionResult> LoadDMapGeoJson(int? campaignId)
        {
            var layers = new FeatureCollection();
            var campaign = await db.Campaigns.FindAsync(campaignId).ConfigureAwait(false);

            var subMaps = await db.SubMaps.Include(i => i.SubMapRecords).Where(i => i.CampaignId == campaignId).ToListAsync().ConfigureAwait(false);
            var subMapIds = subMaps.Select(i => i.Id).ToList();
            var subMapRecords = subMaps.SelectMany(i => i.SubMapRecords.Select(j => new { j.Classification, j.AreaId, j.SubMapId })).ToList();
            Dictionary<string, Polygon> recordsInSubmap = new Dictionary<string, Polygon>();
            foreach (var item in subMaps)
            {
                if(item == null || item.SubMapCoordinates == null || item.SubMapCoordinates.Count == 0)
                {
                    continue;
                }
                var points = item.SubMapCoordinates.OrderBy(i => i.Id).Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToList();
                if (points.Count == 0)
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

                if (polygon == null)
                {
                    continue;
                }

                layers.Add(new Feature
                {
                    Geometry = polygon,
                    BoundingBox = polygon.EnvelopeInternal,
                    Attributes = new AttributesTable
                    {
                        { "type", "submap" },
                        { "id", $"submap-{item.Id}" },
                        { "oid", item.Id },
                        { "sid", item.Id },
                        { "name", item.Name },
                        { "color", $"#{item.ColorString}" },
                    }
                });

                item.SubMapRecords.ToList().ForEach(record =>
                {
                    if (!recordsInSubmap.ContainsKey(record.Code))
                    {
                        recordsInSubmap.Add(record.Code, polygon);
                    }
                });
            }


            var dMaps = await db.DistributionMaps
                .Where(i => subMapIds.Contains(i.SubMapId))
                .OrderBy(i => i.SubMapId)
                .ThenBy(i => i.Id)
                .ToListAsync()
                .ConfigureAwait(false);
            var dMapIds = dMaps.Select(i => i.Id).ToList();

            var dMapPolygon = new List<Polygon>();
            foreach (var item in dMaps)
            {
                var points = item.DistributionMapCoordinates.OrderBy(i=>i.Id).Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToList();
                if (points.Count == 0)
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

                if (polygon == null)
                {
                    continue;
                }
               
                
                dMapPolygon.Add(polygon);
                layers.Add(new Feature
                {
                    Geometry = polygon.Centroid,
                    Attributes = new AttributesTable
                    {
                        { "type", "dmap" },
                        { "id", $"dmap-centroid-{item.Id}" },
                        { "oid", item.Id },
                        { "sid", item.Id },
                        { "name", item.Name },
                        { "color", $"#{item.ColorString}" },
                        { "total", (item.Total ?? 0) + (item.TotalAdjustment ?? 0)},
                        { "count", (item.Penetration ?? 0) + (item.CountAdjustment ?? 0)},
                        { "pen", AreaHelper.CalcPercent(item.Total, item.TotalAdjustment, item.Penetration, item.CountAdjustment)}
                    }
                });
                layers.Add(new Feature
                {
                    Geometry = polygon,
                    BoundingBox = polygon.EnvelopeInternal,
                    Attributes = new AttributesTable
                    {
                        { "type", "dmap" },
                        { "id", $"dmap-{item.Id}" },
                        { "oid", item.Id },
                        { "sid", item.Id },
                        { "name", item.Name },
                        { "color", $"#{item.ColorString}" },
                        
                    }
                });
            }
            var dMapIntersectionLine = new List<LineString>();
            for (var i = 0; i < dMapPolygon.Count; i++)
            {
                var current = dMapPolygon[i];
                for(var j = i + 1; j < dMapPolygon.Count; j++)
                {
                    var intersectionLine = current.Intersection(dMapPolygon[j]);
                    if(intersectionLine is LineString)
                    {
                        dMapIntersectionLine.Add((LineString)intersectionLine);
                    }
                    else if(intersectionLine is MultiLineString)
                    {
                        dMapIntersectionLine.AddRange(((MultiLineString)intersectionLine).Geometries.Select(p => (LineString)p));
                    }
                }
            }
            Geometry dMapIntersectionMultiLine = GeometryFactory.CreateMultiLineString(dMapIntersectionLine.ToArray());
            for (var i = 0; i < dMapPolygon.Count; i++)
            {
                var boundary = GeometryFactory.CreateLineString(dMapPolygon[i].Boundary.Coordinates);
                dMapIntersectionMultiLine = dMapIntersectionMultiLine.Union(boundary);
            }

            if (!dMapIntersectionMultiLine.IsEmpty)
            {
                layers.Add(new Feature
                {
                    Geometry = dMapIntersectionMultiLine,
                    Attributes = new AttributesTable
                    {
                        { "type", "dmap-intersection" },
                        { "id", $"dmap-intersection" },
                    }
                });
            }
            

            var recordGroup = subMapRecords
                .GroupBy(i => i.Classification)
                .ToDictionary(i => i.Key, i => i.Select(j => j.AreaId).ToList());

            var recordSubMap = subMapRecords
                .GroupBy(k => $"{(Classifications)k.Classification}-{k.AreaId}", v => v.SubMapId)
                .ToDictionary(k => k.Key, v => v.FirstOrDefault());


            foreach (var item in recordGroup)
            {
                List<Record> records = new List<Record>();
                var areaIds = item.Value;

                switch (item.Key)
                {
                    case (int)Classifications.Z5:
                        records = await db.FiveZipAreas
                            .Where(i => areaIds.Contains(i.Id)).Select(i => new Record
                            {
                                Id = i.Id,
                                Code = i.Code,
                                Name = i.Name,
                                Zip = i.Zip,
                                Home = i.HOME_COUNT,
                                APT = i.APT_COUNT,
                                Geom = i.Geom
                            })
                            .ToListAsync()
                            .ConfigureAwait(false);

                        break;
                    case (int)Classifications.PremiumCRoute:
                        records = await db.PremiumCRoutes
                            .Where(i => areaIds.Contains(i.Id))
                            .Select(i => new Record
                            {
                                Id = i.Id,
                                Code = i.Code,
                                Name = i.Name,
                                Zip = i.Zip,
                                Home = i.HOME_COUNT,
                                APT = i.APT_COUNT,
                                Business = i.BUSINESS_COUNT,
                                Geom = i.Geom
                            })
                            .ToListAsync()
                            .ConfigureAwait(false);
                        break;
                }

                records.ForEach(recordItem =>
                {
                    var geom = WKTReader.Read(recordItem.Geom.AsText());
                    var submapPolygon = recordsInSubmap[recordItem.Code];
                    geom = geom.Intersection(submapPolygon);
                    var areaId = $"{((Classifications)item.Key).ToString()}-{recordItem.Id}";
                    layers.Add(new Feature
                    {
                        Geometry = geom,
                        BoundingBox = geom.EnvelopeInternal,
                        Attributes = new AttributesTable
                        {
                            { "type", "area" },
                            { "id", areaId },
                            { "oid", recordItem.Id },
                            { "vid", areaId },
                            { "sid", recordSubMap[areaId] },
                            { "name", recordItem.Name },
                            { "zip", recordItem.Zip },
                            { "home", recordItem.Home },
                            { "apt", recordItem.APT },
                            { "business", recordItem.Business },
                            { "classification", ((Classifications)item.Key).ToString() }
                        }
                    });
                    layers.Add(new Feature
                    {
                        Geometry = geom.InteriorPoint,
                        Attributes = new AttributesTable
                        {
                            { "type", "area" },
                            { "id", $"area-centroid-{areaId}" },
                            { "oid", recordItem.Id },
                            { "vid", areaId },
                            { "sid", recordSubMap[areaId] },
                            { "name", recordItem.Name },
                            { "zip", recordItem.Zip },
                            { "home", recordItem.Home },
                            { "apt", recordItem.APT },
                            { "business", recordItem.Business },
                            { "classification", ((Classifications)item.Key).ToString() }
                        }
                    });
                });
            }

            var eTag = GetETag();
            return new GeoJsonResponse(layers, eTag);
        }

        [HttpPost]
        [Route("{campaignId:int}/{subMapId:int}/dmap/edit")]
        public async Task<IHttpActionResult> EditDMap(int subMapId, [FromBody] DistributionMap data)
        {
            var subMap = await db.SubMaps.FindAsync(subMapId).ConfigureAwait(false);
            if (subMap == null)
            {
                throw new Exception("sub map not exists!");
            }

            DistributionMap dMap = null;
            if (data.Id.HasValue)
            {
                dMap = await db.DistributionMaps.FindAsync(data.Id);
            }

            if (dMap != null)
            {
                dMap.Name = data.Name;
                dMap.ColorString = data.ColorString.Replace("#", "");
                dMap.ColorR = data.ColorR;
                dMap.ColorG = data.ColorG;
                dMap.ColorB = data.ColorB;
                dMap.TotalAdjustment = data.TotalAdjustment ?? 0;
                dMap.CountAdjustment = data.CountAdjustment ?? 0;
            }
            else
            {
                dMap = new DistributionMap()
                {
                    SubMapId = data.SubMapId,
                    ColorString = data.ColorString.Replace("#", ""),
                    ColorR = data.ColorR,
                    ColorG = data.ColorG,
                    ColorB = data.ColorB,
                    Name = data.Name,
                    Total = 0,
                    Penetration = 0,
                    Percentage = 0,
                    TotalAdjustment = 0,
                    CountAdjustment = 0,
                };

                subMap.DistributionMaps.Add(dMap);
            }

            await db.SaveChangesAsync().ConfigureAwait(false);

            return Json(new { success = true });
        }

        /// <summary>
        /// Records is area Id
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="dMapId"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route("{campaignId:int}/dmap/{dMapId:int}/merge")]
        public async Task<IHttpActionResult> MergeAreas(int campaignId, int dMapId, [FromBody] List<AreaRecord> records)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId);
            var dMap = await db.DistributionMaps.Where(i => i.Id == dMapId).FirstOrDefaultAsync().ConfigureAwait(false);
            if (campaign == null || dMap == null)
            {
                return BadRequest();
            }

            var targetClassification = records.FirstOrDefault().Classification;

            var needRemoveRecords = records.Where(i => i.Value == false).Select(i => i.Id).ToList();
            var needAddRecords = records.Where(i => i.Value == true).Select(i => i.Id).ToList();

            if (needRemoveRecords.Count == 0 && needAddRecords.Count == 0)
            {
                return BadRequest();
            }

            var dMapRecords = dMap.DistributionMapRecords
                .Select(i => i.AreaId)
                .ToList();

            var needRemoveRecordsSet = needRemoveRecords.ToHashSet();
            var newRecords = dMapRecords.Where(i => !needRemoveRecordsSet.Contains(i)).ToList();
            newRecords.AddRange(needAddRecords);

            // id code apt home geom
            List<Tuple<int?, string, int?, int?, DbGeometry>> dbGeoms = new List<Tuple<int?, string, int?, int?, DbGeometry>>();
            switch (targetClassification)
            {
                case Classifications.Z5:
                    {
                        var dbData = await db.FiveZipAreas
                            .Where(i => newRecords.Contains(i.Id))
                            .Select(i => new { i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom })
                            .ToListAsync()
                            .ConfigureAwait(false);

                        dbGeoms = dbData.Select(i => new Tuple<int?, string, int?, int?, DbGeometry>(i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom)).ToList();

                    }
                    break;
                case Classifications.PremiumCRoute:
                    {
                        var dbData = await db.PremiumCRoutes
                            .Where(i => newRecords.Contains(i.Id))
                            .Select(i => new { i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom })
                            .ToListAsync()
                            .ConfigureAwait(false);

                        dbGeoms = dbData.Select(i => new Tuple<int?, string, int?, int?, DbGeometry>(i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom)).ToList();
                    }
                    break;
            }
            List<Geometry> cachedGeometry = new List<Geometry>();
            Geometry mergedPolygon = null;

            dbGeoms.ForEach(i =>
            {
                var geom = WKTReader.Read(i.Item5.AsText());
                geom = geom.Buffer(0);
                geom.Normalize();

                geom.UserData = new Record
                {
                    Id = i.Item1,
                    Code = i.Item2,
                    APT = i.Item3,
                    Home = i.Item4,
                };
                cachedGeometry.Add(geom);
                if (mergedPolygon == null)
                {
                    mergedPolygon = geom;
                }
                else
                {
                    mergedPolygon = mergedPolygon.Union(geom);
                }
            });

            // find max area polygon
            Polygon finalPolygon = null;
            if (mergedPolygon.GeometryType == "MultiPolygon")
            {
                for (var i = 0; i < mergedPolygon.NumGeometries; i++)
                {
                    Polygon geom = (Polygon)mergedPolygon.GetGeometryN(i);
                    if (geom.IsValid && !geom.IsEmpty)
                    {
                        if (finalPolygon == null)
                        {
                            finalPolygon = geom;
                        }
                        else
                        {
                            finalPolygon = finalPolygon.Area > geom.Area ? finalPolygon : geom;
                        }
                    }
                }
            }
            else if (mergedPolygon.GeometryType == "Polygon")
            {
                finalPolygon = (Polygon)mergedPolygon;
            }

            // dmap should in submap
            var submapId = dMap.SubMapId;
            var submapCoordinates = db.SubMapCoordinates
                .Where(i => i.SubMapId == submapId)
                .OrderBy(i => i.Id)
                .Select(i => new { i.Longitude, i.Latitude })
                .ToList()
                .Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0))
                .ToArray();

            var dmapPolygon = GeometryFactory.CreatePolygon(submapCoordinates);

            finalPolygon = (Polygon)dmapPolygon.Intersection(finalPolygon);

            // fill holes
            finalPolygon = GeometryFactory.CreatePolygon(finalPolygon.Shell);

            if (finalPolygon == null)
            {
                throw new Exception("merge failed. the merge area is not polygon");
            }

            // calc apt home
            float? apt = 0;
            float? home = 0;
            List<Tuple<int?, string>> fixedRecords = new List<Tuple<int?, string>>();
            cachedGeometry.ForEach(i =>
            {
                if (finalPolygon.Intersects(i))
                {
                    Record userData = (Record)i.UserData;
                    apt += userData.APT;
                    home += userData.Home;
                    fixedRecords.Add(new Tuple<int?, string>(userData.Id, userData.Code));
                }
            });

            float? total = 0;
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
                await db.DistributionMapRecords.Where(i=>i.DistributionMapId == dMapId).DeleteAsync().ConfigureAwait(false);
                await db.DistributionMapCoordinates.Where(i => i.DistributionMapId == dMapId).DeleteAsync().ConfigureAwait(false);

                var toAddRecords = fixedRecords.Select(record => new DistributionMapRecord
                {
                    Classification = (int)targetClassification,
                    DistributionMapId = dMapId,
                    AreaId = record.Item1,
                    Code = record.Item2,
                    Value = true,
                });

                db.DistributionMapRecords.AddRange(toAddRecords);

                var toAddCoordinate = finalPolygon.Coordinates.Select(coordinate => new DistributionMapCoordinate
                {
                    DistributionMapId = dMapId,
                    Longitude = coordinate.X,
                    Latitude = coordinate.Y
                });
                db.DistributionMapCoordinates.AddRange(toAddCoordinate);

                dMap.Total = (int)total;

                await db.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();
            }

            return Json(new { sucess = true });
        }

        [HttpDelete]
        [Route("{campaignId:int}/dmap/{dMapId:int}/delete")]
        public async Task<IHttpActionResult> DeleteDMap(int campaignId, int dMapId)
        {
            var dMap = await db.DistributionMaps.FindAsync(dMapId).ConfigureAwait(false);
            if (dMap == null)
            {
                throw new Exception("submap not exists!");
            }

            using (var tran = db.Database.BeginTransaction())
            {
                await db.DistributionMapRecords.Where(i => i.DistributionMapId == dMapId).DeleteAsync().ConfigureAwait(false);
                await db.DistributionMapCoordinates.Where(i => i.DistributionMapId == dMapId).DeleteAsync().ConfigureAwait(false);
                await db.DistributionMaps.Where(i => i.Id == dMapId).DeleteAsync().ConfigureAwait(false);

                tran.Commit();
            }

            return Json(new { success = true });
        }
    }
}