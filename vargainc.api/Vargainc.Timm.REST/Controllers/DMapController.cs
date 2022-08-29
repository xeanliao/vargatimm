﻿using Microsoft.SqlServer.Types;
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
            //var subMapRecords = subMaps.SelectMany(i => i.SubMapRecords.Select(j => new { j.Classification, j.AreaId, j.SubMapId })).ToList();
            List<Tuple<int?, Polygon, int?, List<int?>>> subMapRecords = new List<Tuple<int?, Polygon, int?, List<int?>>>();
            foreach (var subMap in subMaps)
            {
                if(subMap == null || subMap.SubMapCoordinates == null || subMap.SubMapCoordinates.Count == 0)
                {
                    continue;
                }
                var points = subMap.SubMapCoordinates.OrderBy(i => i.Id).Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToList();
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
                        { "id", $"submap-{subMap.Id}" },
                        { "oid", subMap.Id },
                        { "sid", subMap.Id },
                        { "name", subMap.Name },
                        { "color", $"#{subMap.ColorString}" },
                    }
                });

                var records = subMap.SubMapRecords.Select(i => i.AreaId).ToList();
                var recordsType = subMap.SubMapRecords.FirstOrDefault()?.Classification;

                subMapRecords.Add(new Tuple<int?, Polygon, int?, List<int?>>(subMap.Id, polygon, recordsType, records));

                // holes
                var holes = subMap.Holes.Select(i => i.AreaId).ToHashSet();
                if (holes.Count > 0)
                {
                    List<Tuple<int?, string, int?, int?, DbGeometry>> dbGeoms = new List<Tuple<int?, string, int?, int?, DbGeometry>>();

                    var classification = subMap.SubMapRecords.FirstOrDefault()?.Classification;
                    if (classification != null && classification.HasValue)
                    {
                        var targetClassification = (Classifications)classification;
                        switch (targetClassification)
                        {
                            case Classifications.Z5:
                                {
                                    var dbData = await db.FiveZipAreas
                                        .Where(i => holes.Contains(i.Id))
                                        .Select(i => new { i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom })
                                        .ToListAsync()
                                        .ConfigureAwait(false);

                                    dbGeoms = dbData.Select(i => new Tuple<int?, string, int?, int?, DbGeometry>(i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom)).ToList();

                                }
                                break;
                            case Classifications.PremiumCRoute:
                                {
                                    var dbData = await db.PremiumCRoutes
                                        .Where(i => holes.Contains(i.Id))
                                        .Select(i => new { i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom })
                                        .ToListAsync()
                                        .ConfigureAwait(false);

                                    dbGeoms = dbData.Select(i => new Tuple<int?, string, int?, int?, DbGeometry>(i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom)).ToList();
                                }
                                break;
                        }
                    }
                    dbGeoms.ForEach(i =>
                    {
                        var geom = WKTReader.Read(i.Item5.AsText());
                        geom = geom.Buffer(-0.000001);
                        geom = polygon.Intersection(geom);
                        geom.Normalize();
                        layers.Add(new Feature
                        {
                            Geometry = geom,
                            BoundingBox = geom.EnvelopeInternal,
                            Attributes = new AttributesTable
                            {
                                { "type", "submap-holes" },
                                { "id", $"submap-{subMap.Id}-hole-{i.Item2}" },
                            }
                        });
                    });
                }
            }


            var dMaps = await db.DistributionMaps
                .Where(i => subMapIds.Contains(i.SubMapId))
                .OrderBy(i => i.SubMapId)
                .ThenBy(i => i.Id)
                .ToListAsync()
                .ConfigureAwait(false);
            var dMapIds = dMaps.Select(i => i.Id).ToList();

            var dMapPolygon = new List<Polygon>();
            foreach (var dmap in dMaps)
            {
                var points = dmap.DistributionMapCoordinates.OrderBy(i=>i.Id).Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToList();
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
                        { "id", $"dmap-centroid-{dmap.Id}" },
                        { "oid", dmap.Id },
                        { "sid", dmap.Id },
                        { "name", dmap.Name },
                        { "color", $"#{dmap.ColorString}" },
                        { "total", (dmap.Total ?? 0) + (dmap.TotalAdjustment ?? 0)},
                        { "count", (dmap.Penetration ?? 0) + (dmap.CountAdjustment ?? 0)},
                        { "pen", AreaHelper.CalcPercent(dmap.Total, dmap.TotalAdjustment, dmap.Penetration, dmap.CountAdjustment)}
                    }
                });
                layers.Add(new Feature
                {
                    Geometry = polygon,
                    BoundingBox = polygon.EnvelopeInternal,
                    Attributes = new AttributesTable
                    {
                        { "type", "dmap" },
                        { "id", $"dmap-{dmap.Id}" },
                        { "oid", dmap.Id },
                        { "sid", dmap.Id },
                        { "name", dmap.Name },
                        { "color", $"#{dmap.ColorString}" },
                        
                    }
                });

                // holes
                var holes = dmap.Holes.Select(i => i.AreaId).ToHashSet();
                if (holes.Count > 0)
                {
                    List<Tuple<int?, string, int?, int?, DbGeometry>> dbGeoms = new List<Tuple<int?, string, int?, int?, DbGeometry>>();

                    var classification = dmap.DistributionMapRecords.FirstOrDefault()?.Classification;
                    if (classification != null && classification.HasValue)
                    {
                        var targetClassification = (Classifications)classification;
                        switch (targetClassification)
                        {
                            case Classifications.Z5:
                                {
                                    var dbData = await db.FiveZipAreas
                                        .Where(i => holes.Contains(i.Id))
                                        .Select(i => new { i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom })
                                        .ToListAsync()
                                        .ConfigureAwait(false);

                                    dbGeoms = dbData.Select(i => new Tuple<int?, string, int?, int?, DbGeometry>(i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom)).ToList();

                                }
                                break;
                            case Classifications.PremiumCRoute:
                                {
                                    var dbData = await db.PremiumCRoutes
                                        .Where(i => holes.Contains(i.Id))
                                        .Select(i => new { i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom })
                                        .ToListAsync()
                                        .ConfigureAwait(false);

                                    dbGeoms = dbData.Select(i => new Tuple<int?, string, int?, int?, DbGeometry>(i.Id, i.Code, i.APT_COUNT, i.HOME_COUNT, i.Geom)).ToList();
                                }
                                break;
                        }
                    }
                    dbGeoms.ForEach(i =>
                    {
                        var geom = WKTReader.Read(i.Item5.AsText());
                        geom = geom.Buffer(-0.000001);
                        geom = polygon.Intersection(geom);
                        geom.Normalize();
                        layers.Add(new Feature
                        {
                            Geometry = geom,
                            BoundingBox = geom.EnvelopeInternal,
                            Attributes = new AttributesTable
                            {
                                { "type", "dmap-holes" },
                                { "id", $"dmap-{dmap.Id}-hole-{i.Item2}" },
                            }
                        });
                    });
                }
            }
            var dMapIntersectionLine = new List<LineString>();
            for (var i = 0; i < dMapPolygon.Count; i++)
            {
                var current = dMapPolygon[i];
                for(var j = i + 1; j < dMapPolygon.Count; j++)
                {
                    try
                    {
                        var intersectionLine = current.Intersection(dMapPolygon[j]);
                        if (intersectionLine is LineString)
                        {
                            dMapIntersectionLine.Add((LineString)intersectionLine);
                        }
                        else if (intersectionLine is MultiLineString)
                        {
                            dMapIntersectionLine.AddRange(((MultiLineString)intersectionLine).Geometries.Select(p => (LineString)p));
                        }
                    }
                    catch
                    {

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
            

            //var recordGroup = subMapRecords
            //    .GroupBy(i => i.Classification)
            //    .ToDictionary(i => i.Key, i => i.Select(j => j.AreaId).ToList());

            //var recordSubMap = subMapRecords
            //    .GroupBy(k => $"{(Classifications)k.Classification}-{k.AreaId}", v => v.SubMapId)
            //    .ToDictionary(k => k.Key, v => v.FirstOrDefault());


            foreach (var item in subMapRecords)
            {
                var submapId = item.Item1;
                var submapPolygon = item.Item2;
                var recordType = item.Item3;
                var areaIds = item.Item4;

                List<Record> records = new List<Record>();

                switch (recordType)
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
                foreach(var recordItem in records)
                {
                    if(recordItem  == null)
                    {
                        continue;
                    }
                    Geometry geom = null;
                    try
                    {
                        geom = WKTReader.Read(recordItem.Geom.AsText());
                        geom = geom.Buffer(0).Intersection(submapPolygon);

                        if (geom.IsEmpty)
                        {
                            continue;
                        }
                    }
                    catch(Exception)
                    {
                        continue;
                    }
                    
                    var areaId = $"{((Classifications)recordType).ToString()}-{recordItem.Id}";
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
                            { "sid", submapId },
                            { "name", recordItem.Name },
                            { "zip", recordItem.Zip },
                            { "home", recordItem.Home },
                            { "apt", recordItem.APT },
                            { "business", recordItem.Business },
                            { "classification", ((Classifications)recordType).ToString() }
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
                            { "sid", submapId },
                            { "name", recordItem.Name },
                            { "zip", recordItem.Zip },
                            { "home", recordItem.Home },
                            { "apt", recordItem.APT },
                            { "business", recordItem.Business },
                            { "classification", ((Classifications)recordType).ToString() }
                        }
                    });
                }
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
            var dMap = await db.DistributionMaps.Where(i => i.Id == dMapId).Include(i => i.SubMap).FirstOrDefaultAsync().ConfigureAwait(false);
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

            var otherDMaps = dMap.SubMap.DistributionMaps.Where(i => i.Id != dMapId).Select(i => i.Id).ToHashSet();
            var otherDMapRecords = db.DistributionMapRecords.Where(i => otherDMaps.Contains(i.DistributionMapId)).Select(i => i.AreaId).ToHashSet();
            needAddRecords = needAddRecords.Where(i => !otherDMapRecords.Contains(i)).ToList();

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
            List<DistributionMapHole> holes = new List<DistributionMapHole>();
            Geometry mergedPolygon = null;
            Polygon finalPolygon = GeometryFactory.CreatePolygon();

            dbGeoms.ForEach(i =>
            {
                var geom = WKTReader.Read(i.Item5.AsText());
                switch (geom.GeometryType)
                {
                    case "Polygon":
                        geom = GeometryFactory.CreatePolygon(((Polygon)geom).Shell);
                        geom = geom.Buffer(0);
                        geom.Normalize();
                        break;
                    case "MultiPolygon":
                        var fixGeom = (MultiPolygon)geom;
                        Polygon[] geomCollection = new Polygon[fixGeom.NumGeometries];
                        for (var n = 0; n < fixGeom.NumGeometries; n++)
                        {
                            var partGeom = (Polygon)fixGeom.GetGeometryN(n);
                            partGeom = GeometryFactory.CreatePolygon(partGeom.Shell);
                            var fixPartGeom = partGeom.Buffer(0);
                            fixPartGeom.Normalize();
                            geomCollection[n] = (Polygon)fixPartGeom;
                        }
                        geom = GeometryFactory.CreateMultiPolygon(geomCollection);
                        break;
                    default:
                        geom = geom.Buffer(0);
                        geom.Normalize();
                        break;
                }

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

            if (mergedPolygon == null)
            {
                throw new Exception("merge failed. the merge area is not polygon");
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

            var subMapPolygon = GeometryFactory.CreatePolygon(submapCoordinates);

            mergedPolygon = subMapPolygon.Intersection(mergedPolygon);

            // find connect old dmap or max area polygon
            // load old dmap
            var oldDMapCoordinate = dMap.DistributionMapCoordinates.Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToArray();
            var oldDMap = GeometryFactory.CreatePolygon(oldDMapCoordinate);

            finalPolygon = null;
            if (mergedPolygon.GeometryType == "MultiPolygon" || mergedPolygon.GeometryType == "GeometryCollection")
            {
                bool marched = false;
                for (var i = 0; i < mergedPolygon.NumGeometries; i++)
                {
                    try
                    {
                        Polygon geom = (Polygon)mergedPolygon.GetGeometryN(i);
                        if (geom.IsValid && !geom.IsEmpty && geom.Contains(oldDMap))
                        {
                            finalPolygon = geom;
                            marched = true;
                            break;
                        }
                    }
                    catch
                    {

                    }

                }
                if (!marched)
                {
                    for (var i = 0; i < mergedPolygon.NumGeometries; i++)
                    {
                        try
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
                        catch
                        {

                        }

                    }
                }
            }
            else if (mergedPolygon.GeometryType == "Polygon")
            {
                finalPolygon = (Polygon)mergedPolygon;
            }

            if (finalPolygon == null)
            {
                throw new Exception("merge failed. the merge area is not polygon");
            }

            // fill holes
            finalPolygon = GeometryFactory.CreatePolygon(finalPolygon.Shell);

            // find holes area
            var sql = new StringBuilder($"DECLARE @g geometry = geometry::STPolyFromText(@p0, {SRID_DB});");
            switch (targetClassification)
            {
                case Classifications.Z5:
                    {
                        sql.AppendLine($"SELECT Id, Code, APT_COUNT, HOME_COUNT FROM [dbo].[fivezipareas_all] WITH(INDEX(IX_fivezipareas_all_Geom)) WHERE Geom.STIntersects(@g) = 1");
                    }
                    break;
                case Classifications.PremiumCRoute:
                    {
                        sql.AppendLine($"SELECT Id, Code, APT_COUNT, HOME_COUNT FROM [dbo].[premiumcroutes_all] WITH(INDEX(IX_premiumcroutes_all_Geom)) WHERE Geom.STIntersects(@g) = 1");
                    }
                    break;
            }
            var sqlCmd = db.Database.Connection.CreateCommand();
            sqlCmd.CommandTimeout = 300;
            sqlCmd.CommandText = sql.ToString();
            SqlParameter p = new SqlParameter()
            {
                ParameterName = "@p0",
                Value = finalPolygon.Buffer(-0.000001).AsText(),
                SqlDbType = System.Data.SqlDbType.NVarChar
            };
            sqlCmd.Parameters.Add(p);
            await db.Database.Connection.OpenAsync().ConfigureAwait(false);
            var sqlReader = await sqlCmd.ExecuteReaderAsync().ConfigureAwait(false);
            var existsArea = cachedGeometry.Select(i => (i.UserData as Record).Id).ToHashSet();
            while (sqlReader.Read())
            {
                var id = sqlReader.GetInt32(0);
                if (!existsArea.Contains(id))
                {
                    var code = sqlReader.GetString(1);
                    var holeApt = sqlReader.IsDBNull(2) ? null : new int?(sqlReader.GetInt32(2));
                    var holeHome = sqlReader.IsDBNull(3) ? null : new int?(sqlReader.GetInt32(3));

                    holes.Add(new DistributionMapHole { DistributionMapId = dMapId, AreaId = id, Code = code, Apt = holeApt, Home = holeHome });
                }
            }
            sqlReader.Close();

            await db.DistributionMapHoles.Where(i => i.DistributionMapId == dMapId).DeleteAsync().ConfigureAwait(false);

            if (holes.Count > 0)
            {
                // holes must in submap record
                var subMapRecords = db.SubMapRecords
                    .Where(i => i.SubMapId == submapId)
                    .Select(i => i.AreaId)
                    .ToHashSet();

                holes = holes.Where(h => subMapRecords.Contains(h.AreaId)).ToList();

                if (holes.Count > 0)
                {
                    db.DistributionMapHoles.AddRange(holes);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
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
                await db.DistributionMapRecords.Where(i => i.DistributionMapId == dMapId).DeleteAsync().ConfigureAwait(false);
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

            return Json(new { success = true, holes = holes });
        }

        [HttpDelete]
        [Route("{campaignId:int}/dmap/{dMapId:int}/delete")]
        public async Task<IHttpActionResult> DeleteDMap(int campaignId, int dMapId)
        {
            var dMap = await db.DistributionMaps.FindAsync(dMapId).ConfigureAwait(false);
            if (dMap == null)
            {
                throw new Exception("dmap not exists!");
            }

            using (var tran = db.Database.BeginTransaction())
            {
                await db.DistributionMapHoles.Where(i => i.DistributionMapId == dMapId).DeleteAsync().ConfigureAwait(false);
                await db.DistributionMapRecords.Where(i => i.DistributionMapId == dMapId).DeleteAsync().ConfigureAwait(false);
                await db.DistributionMapCoordinates.Where(i => i.DistributionMapId == dMapId).DeleteAsync().ConfigureAwait(false);
                await db.DistributionMaps.Where(i => i.Id == dMapId).DeleteAsync().ConfigureAwait(false);

                tran.Commit();
            }

            return Json(new { success = true });
        }
    }
}