using Microsoft.SqlServer.Types;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using Vargainc.Timm.Models;
using Vargainc.Timm.REST.ViewModel;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("campaign")]
    public class DMapController : BaseController
    {
        [HttpGet]
        [Route("{campaignId}/dmap/geojson")]
        public async Task<HttpResponseMessage> LoadDMapGeoJson(int? campaignId)
        {
            var layers = new FeatureCollection();
            var campaign = await db.Campaigns.FindAsync(campaignId).ConfigureAwait(false);

            var subMaps = await db.SubMaps.Include(i => i.SubMapRecords).Where(i => i.CampaignId == campaignId).ToListAsync().ConfigureAwait(false);
            var subMapIds = subMaps.Select(i => i.Id).ToList();
            var subMapRecords = subMaps.SelectMany(i => i.SubMapRecords.Select(j => new { j.Classification, j.AreaId, j.SubMapId })).ToList();
            var subMapGeom = await db.SubMapCoordinates
                .Where(i => subMapIds.Contains(i.SubMapId))
                .OrderBy(i => i.SubMapId)
                .ThenBy(i => i.Id)
                .Select(i => new { i.SubMapId, i.Longitude, i.Latitude })
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var item in subMaps)
            {
                var coorinates = subMapGeom.Where(i => i.SubMapId == item.Id).Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToList();
                if (!coorinates.First().Equals2D(coorinates.Last()))
                {
                    coorinates.Add(coorinates.First());
                }
                var geom = GeometryFactory.CreatePolygon(coorinates.ToArray());
                geom.Normalize();
                if (!geom.IsValid)
                {
                    geom = geom.Buffer(0) as Polygon;
                }

                layers.Add(new Feature
                {
                    Geometry = geom,
                    BoundingBox = geom.EnvelopeInternal,
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
            }

            var dMaps = await db.DistributionMaps
                .Where(i => subMapIds.Contains(i.SubMapId))
                .OrderBy(i => i.SubMapId)
                .ThenBy(i => i.Id)
                .ToListAsync()
                .ConfigureAwait(false);
            var dMapIds = dMaps.Select(i => i.Id).ToList();
            var dMapGeom = await db.DistributionMapCoordinates
                .Where(i => dMapIds.Contains(i.DistributionMapId))
                .OrderBy(i => i.DistributionMapId)
                .ThenBy(i => i.Id)
                .Select(i => new { i.DistributionMapId, i.Longitude, i.Latitude })
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var item in dMaps)
            {
                var coorinates = dMapGeom.Where(i => i.DistributionMapId == item.Id).Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToList();
                if (coorinates.Count == 0)
                {
                    continue;
                }
                if (!coorinates.First().Equals2D(coorinates.Last()))
                {
                    coorinates.Add(coorinates.First());
                }
                var geom = GeometryFactory.CreatePolygon(coorinates.ToArray());
                geom.Normalize();
                if (!geom.IsValid)
                {
                    geom = geom.Buffer(0) as Polygon;
                }
                if(geom == null)
                {
                    continue;
                }
                layers.Add(new Feature
                {
                    Geometry = geom,
                    BoundingBox = geom.EnvelopeInternal,
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

            var recordGroup = subMapRecords
                .GroupBy(i => i.Classification)
                .ToDictionary(i => i.Key, i => i.Select(j => j.AreaId).ToList());

            var recordSubMap = subMapRecords
                .GroupBy(k => $"{(Classifications)k.Classification}-{k.AreaId}", v => v.SubMapId)
                .ToDictionary(k => k.Key, v => v.FirstOrDefault());

            db.Database.Connection.Open();
            var sqlCmd = db.Database.Connection.CreateCommand();

            foreach (var item in recordGroup)
            {
                StringBuilder sql = new StringBuilder();
                switch (item.Key)
                {
                    case (int)Classifications.Z3:
                        sql.AppendLine("SELECT [Id],[Code] AS [Name],'' AS [ZIP],[HOME_COUNT],[BUSINESS_COUNT],[APT_COUNT],[IsMaster],[IsInnerRing], [Latitude],[Longitude],[Geom]");
                        sql.AppendLine("FROM [dbo].[threezipareas]");
                        sql.AppendLine($"WHERE [Id] IN ({string.Join(",", item.Value)})");
                        break;
                    case (int)Classifications.Z5:
                        sql.AppendLine("SELECT [Id],[Code] AS [Name],[Code] AS [ZIP],[HOME_COUNT],[BUSINESS_COUNT],[APT_COUNT],[IsMaster],[IsInnerRing],[Latitude],[Longitude],[Geom]");
                        sql.AppendLine("FROM [dbo].[fivezipareas]");
                        sql.AppendLine($"WHERE [Id] IN ({string.Join(",", item.Value)})");
                        break;
                    case (int)Classifications.PremiumCRoute:
                        sql.AppendLine("SELECT [Id],[GEOCODE] AS [Name],[ZIP],[HOME_COUNT],[BUSINESS_COUNT],[APT_COUNT],[IsMaster],[IsInnerRing],[Latitude],[Longitude],[Geom]");
                        sql.AppendLine("FROM [dbo].[premiumcroutes]");
                        sql.AppendLine($"WHERE [Id] IN ({string.Join(",", item.Value)})");
                        break;
                }

                sqlCmd.CommandTimeout = 300;
                sqlCmd.CommandText = sql.ToString();

                var sqlReader = await sqlCmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (sqlReader.Read())
                {
                    var geomWKT = ((SqlGeometry)sqlReader.GetValue(10)).ToString();
                    var geom = WKTReader.Read(geomWKT);
                    var id = sqlReader.GetInt32(0);
                    var areaId = $"{((Classifications)item.Key).ToString()}-{id}";
                    layers.Add(new Feature
                    {
                        Geometry = geom,
                        BoundingBox = geom.EnvelopeInternal,
                        Attributes = new AttributesTable
                        {
                            { "type", "area" },
                            { "id", areaId },
                            { "oid", id },
                            { "vid", areaId },
                            { "sid", recordSubMap[areaId] },
                            { "name", sqlReader.GetString(1) },
                            { "zip", sqlReader.GetString(2) },
                            { "home", sqlReader.GetInt32(3) },
                            { "apt", sqlReader.GetInt32(5) },
                            { "business", sqlReader.GetInt32(4) },
                            { "classification", item.Key }
                        }
                    });
                }
                sqlReader.Close();
            }

            MemoryStream ms = new MemoryStream();
            var serializer = GeoJsonSerializer.Create();
            var streamWriter = new StreamWriter(ms, Encoding.Default, 1024, true);

            serializer.Serialize(streamWriter, layers);
            streamWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var eTag = Convert.ToBase64String(MD5.Create().ComputeHash(ms));
            var eTagCheck = GetETag();
            if (eTag == eTagCheck)
            {
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }

            ms.Seek(0, SeekOrigin.Begin);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.TryAddWithoutValidation("ETag", eTag);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/geo+json");
            return response;
        }

        [HttpPost]
        [Route("{campaignId:int}/{subMapId:int}/dmap/edit")]
        public async Task<IHttpActionResult> NewDMap(int subMapId, [FromBody] DistributionMap data)
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


            var dMapRecords = dMap.DistributionMapRecords
                .Select(i => i.AreaId ?? 0)
                .ToList();

            var targetClassification = records.FirstOrDefault().Classification;

            var removeAreas = records.Where(i => i.Value == false).Select(i => i.Id).ToList();
            var addAreas = records.Where(i => i.Value == true).Select(i => i.Id).ToList();
            List<int> needRemoveRecords = records.Where(i => i.Value == false).Select(i => i.Id ?? 0).ToList();
            List<int> needAddRecords = records.Where(i => i.Value != false).Select(i => i.Id ?? 0).ToList();


            var needRemoveRecordsSet = needRemoveRecords.ToHashSet();
            var newRecords = dMapRecords.Where(i => !needRemoveRecordsSet.Contains(i)).ToList();
            newRecords.AddRange(needAddRecords);

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
            switch (targetClassification)
            {
                case Classifications.Z3:
                    var resultZ3 = db.ThreeZipAreas.Where(i => newRecords.Contains(i.Id.Value))
                        .GroupBy(i => i.Code)
                        .Select(g => new { APT = g.Max(i => i.APT_COUNT), HOME = g.Max(i => i.HOME_COUNT) })
                        .ToList();
                    apt = resultZ3.Sum(i => i.APT ?? 0);
                    home = resultZ3.Sum(i => i.HOME ?? 0);
                    break;
                case Classifications.Z5:
                    var resultZ5 = db.FiveZipAreas.Where(i => newRecords.Contains(i.Id.Value))
                        .GroupBy(i => i.Code)
                        .Select(g => new { APT = g.Max(i => i.APT_COUNT), HOME = g.Max(i => i.HOME_COUNT) })
                        .ToList();
                    apt = resultZ5.Sum(i => i.APT ?? 0);
                    home = resultZ5.Sum(i => i.HOME ?? 0);
                    break;
                case Classifications.PremiumCRoute:
                    var resultCRoute = db.PremiumCRoutes.Where(i => newRecords.Contains(i.Id.Value))
                        .GroupBy(i => i.GEOCODE)
                        .Select(g => new { APT = g.Max(i => i.APT_COUNT), HOME = g.Max(i => i.HOME_COUNT) })
                        .ToList();
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


            db.Configuration.AutoDetectChangesEnabled = false;
            using (var transaction = db.Database.BeginTransaction())
            {
                db.Database.ExecuteSqlCommand($"delete from [dbo].[distributionmaprecords] where [DistributionMapId] = @p0", dMapId);
                db.Database.ExecuteSqlCommand($"delete from [dbo].[distributionmapcoordinates] where [DistributionMapId] = @p0", dMapId);

                foreach (var record in newRecords)
                {
                    db.DistributionMapRecords.Add(new DistributionMapRecord
                    {
                        Classification = (int)targetClassification,
                        DistributionMapId = dMapId,
                        AreaId = record,
                        Value = true
                    });
                }


                foreach (var coordinate in mergedPolygon.Coordinates)
                {
                    db.DistributionMapCoordinates.Add(new DistributionMapCoordinate
                    {
                        DistributionMapId = dMapId,
                        Longitude = coordinate.X,
                        Latitude = coordinate.Y
                    });
                }

                dMap.Total = total;

                db.Entry(dMap).Property(i => i.Total).IsModified = true;

                db.SaveChanges();

                transaction.Commit();
            }
            db.Configuration.AutoDetectChangesEnabled = true;

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
            try
            {
                db.Database.Connection.Open();

                using (var transaction = db.Database.BeginTransaction())
                {


                    db.Database.ExecuteSqlCommand("delete from [dbo].[distributionmaprecords] where [SubMapId] = @p0", dMapId);
                    db.Database.ExecuteSqlCommand("delete from [dbo].[distributionmapcoordinates] where [SubMapId] = @p0", dMapId);
                    db.Database.ExecuteSqlCommand("delete from [dbo].[distributionmaps] where [Id] = @p0", dMapId);

                    transaction.Commit();
                }

            }
            finally
            {
                db.Database.Connection.Close();
            }

            return Json(new { success = true });
        }
    }
}