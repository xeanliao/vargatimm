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
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using Microsoft.SqlServer.Types;
using System.Text;

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

        /// <summary>
        /// Records is area code
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="submapId"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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

            var removeAreas = records.Where(i => i.Value == false).Select(i => i.Name).ToList();
            var addAreas = records.Where(i => i.Value == true).Select(i => i.Name).ToList();

            List<int?> needRemoveRecords = new List<int?>();
            List<int?> needAddRecords = new List<int?>();
            switch (targetClassification)
            {
                case Classifications.Z5:
                    needRemoveRecords = await db.FiveZipAreas.Where(i => removeAreas.Contains(i.Code)).Select(i => i.Id).ToListAsync().ConfigureAwait(false);
                    needAddRecords = await db.FiveZipAreas.Where(i => addAreas.Contains(i.Code)).Select(i => i.Id).ToListAsync().ConfigureAwait(false);
                    break;
                case Classifications.PremiumCRoute:
                    needRemoveRecords = await db.PremiumCRoutes.Where(i => removeAreas.Contains(i.Code)).Select(i => i.Id).ToListAsync().ConfigureAwait(false);
                    needAddRecords = await db.PremiumCRoutes.Where(i => addAreas.Contains(i.Code)).Select(i => i.Id).ToListAsync().ConfigureAwait(false);
                    break;
            }
            if (needRemoveRecords.Count == 0 && needAddRecords.Count == 0)
            {
                return BadRequest();
            }

            var subMapRecords = submap.SubMapRecords
                .Select(i => i.AreaId)
                .ToList();

            var needRemoveRecordsSet = needRemoveRecords.ToHashSet();
            var newRecords = subMapRecords.Where(i => !needRemoveRecordsSet.Contains(i)).ToList();
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
            Polygon finalPolygon = GeometryFactory.CreatePolygon();

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

            if (mergedPolygon != null)
            {
                // find max area polygon
                
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

                // fill holes
                finalPolygon = GeometryFactory.CreatePolygon(finalPolygon.Shell);

                // add missed area
                if (finalPolygon != null)
                {
                    var sql = new StringBuilder($"DECLARE @g geometry = geometry::STPolyFromText(@p0, {SRID_DB});");
                    switch (targetClassification)
                    {
                        case Classifications.Z5:
                            {
                                sql.AppendLine($"SELECT Id, Code, APT_COUNT, HOME_COUNT, Geom FROM [dbo].[fivezipareas_all] WITH(INDEX(IX_fivezipareas_all_Geom)) WHERE Geom.STIntersects(@g) = 1");
                            }
                            break;
                        case Classifications.PremiumCRoute:
                            {
                                sql.AppendLine($"SELECT Id, Code, APT_COUNT, HOME_COUNT, Geom FROM [dbo].[premiumcroutes_all] WITH(INDEX(IX_premiumcroutes_all_Geom)) WHERE Geom.STIntersects(@g) = 1");
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
                            var geom = WKTReader.Read(((SqlGeometry)sqlReader.GetValue(4)).ToString());
                            geom = geom.Buffer(0);
                            geom.Normalize();

                            geom.UserData = new Record
                            {
                                Id = id,
                                Code = sqlReader.GetString(1),
                                APT = sqlReader.GetInt32(2),
                                Home = sqlReader.GetInt32(3),
                            };
                            cachedGeometry.Add(geom);
                        }
                    }
                    sqlReader.Close();
                }

                // merge again
                mergedPolygon = finalPolygon;

                cachedGeometry.ForEach(geom =>
                {
                    mergedPolygon = mergedPolygon.Union(geom);
                });

                // find max area polygon or connect old submap
                var oldSubMapCoordinate = submap.SubMapCoordinates.Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToArray();
                var oldSubMap = GeometryFactory.CreatePolygon(oldSubMapCoordinate);
                finalPolygon = null;
                if (mergedPolygon.GeometryType == "MultiPolygon")
                {
                    bool marched = false;
                    for (var i = 0; i < mergedPolygon.NumGeometries; i++)
                    {
                        try
                        {
                            Polygon geom = (Polygon)mergedPolygon.GetGeometryN(i);
                            if (geom.IsValid && !geom.IsEmpty && geom.Contains(oldSubMap))
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
                }
                else if (mergedPolygon.GeometryType == "Polygon")
                {
                    finalPolygon = (Polygon)mergedPolygon;
                }

                // fill holes
                finalPolygon = GeometryFactory.CreatePolygon(finalPolygon.Shell);

                if (finalPolygon == null)
                {
                    throw new Exception("merge failed. the merge area is not polygon");
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
                await db.SubMapRecords.Where(i => i.SubMapId == submapId).DeleteAsync().ConfigureAwait(false);
                await db.SubMapCoordinates.Where(i => i.SubMapId == submapId).DeleteAsync().ConfigureAwait(false);

                var toAddRecords = fixedRecords.Select(record => new SubMapRecord
                {
                    Classification = (int)targetClassification,
                    SubMapId = submapId,
                    AreaId = record.Item1,
                    Code = record.Item2,
                    Value = true,
                });

                db.SubMapRecords.AddRange(toAddRecords);

                var toAddCoordinate = finalPolygon.Coordinates.Select(coordinate => new SubMapCoordinate
                {
                    SubMapId = submapId,
                    Longitude = coordinate.X,
                    Latitude = coordinate.Y
                });

                db.SubMapCoordinates.AddRange(toAddCoordinate);

                submap.Total = (int)total;

                await db.SaveChangesAsync().ConfigureAwait(false);

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
