using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.VectorTiles;
using NetTopologySuite.IO.VectorTiles.Mapbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using LinearRing = NetTopologySuite.Geometries.LinearRing;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.SqlServer.Types;
using System.Diagnostics;
using Vargainc.Timm.REST.ViewModel;
using System.Configuration;
using log4net;
using Vargainc.Timm.Extentions;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("area")]
    public class AreaController : BaseController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AreaController));

        private static readonly string CachePath = ConfigurationManager.AppSettings["TilesCachePath"];

        private static readonly HashSet<Classifications> LAYERS = new HashSet<Classifications>()
        {
            Classifications.Z3,
            Classifications.Z5,
            Classifications.PremiumCRoute,
        };

        /// <summary>
        /// Need Load inner ring
        /// </summary>
        /// <returns></returns>
        [Route("tiles/{layer}/{z}/{x}/{y}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTiles(Classifications layer, int z, int x, int y)
        {
            if (!LAYERS.Contains(layer))
            {
                return BadRequest($"bad layer type {layer}, only allowed Z3 Z5 PremiumCRoute");
            }

            var cacheFile = Path.Combine(CachePath, layer.ToString(), z.ToString(), $"{x}.{y}.pbf");
            var eTag = GetETag();
            if (File.Exists(cacheFile))
            {
                return new MapboxTileResponse(cacheFile, eTag);
            }

            try
            {
                var tile = new NetTopologySuite.IO.VectorTiles.Tiles.Tile(x, y, z);
                var tileBBbox = new Envelope(tile.Left, tile.Right, tile.Top, tile.Bottom);
                var tilePolygon = GeometryFactory.CreatePolygon(new LinearRing(new Coordinate[] {
                    new Coordinate(tile.Left, tile.Top),
                    new Coordinate(tile.Right, tile.Top),
                    new Coordinate(tile.Right, tile.Bottom),
                    new Coordinate(tile.Left, tile.Bottom),
                    new Coordinate(tile.Left, tile.Top),
                }));
                StringBuilder sql = new StringBuilder($"DECLARE @g geometry = geometry::STPolyFromText(@p0, {SRID_DB});");
                switch (layer)
                {
                    case Classifications.Z3:
                        sql.AppendLine("SELECT [Id],[Name],[ZIP],[HOME_COUNT],[BUSINESS_COUNT],[APT_COUNT],[Geom]");
                        sql.AppendLine("FROM [dbo].[threezipareas_all] WITH(INDEX(IX_threezipareas_all_Geom))");
                        sql.AppendLine("WHERE geom.STIntersects(@g) = 1");
                        break;
                    case Classifications.Z5:
                        sql.AppendLine("SELECT [Id],[Name],[ZIP],[HOME_COUNT],[BUSINESS_COUNT],[APT_COUNT],[Geom]");
                        sql.AppendLine("FROM [dbo].[fivezipareas_all] WITH(INDEX(IX_fivezipareas_all_Geom))");
                        sql.AppendLine("WHERE geom.STIntersects(@g) = 1");
                        break;
                    case Classifications.PremiumCRoute:
                        sql.AppendLine("SELECT [Id],[Name],[ZIP],[HOME_COUNT],[BUSINESS_COUNT],[APT_COUNT],[Geom]");
                        sql.AppendLine("FROM [dbo].[premiumcroutes_all] WITH(INDEX(IX_premiumcroutes_all_Geom))");
                        sql.AppendLine("WHERE geom.STIntersects(@g) = 1");
                        break;
                }

                db.Database.Connection.Open();
                var sqlCmd = db.Database.Connection.CreateCommand();
                sqlCmd.CommandTimeout = 300;
                sqlCmd.CommandText = sql.ToString();
                SqlParameter p = new SqlParameter()
                {
                    ParameterName = "@p0",
                    Value = tilePolygon.AsText(),
                    SqlDbType = System.Data.SqlDbType.NVarChar
                };
                sqlCmd.Parameters.Add(p);
                List<ViewModel.Tile> data = new List<ViewModel.Tile>();

                var sqlReader = await sqlCmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (sqlReader.Read())
                {
                    data.Add(new ViewModel.Tile
                    {
                        Layer = layer.ToString(),
                        Id = sqlReader.GetInt32(0),
                        Name = sqlReader.GetString(1),
                        ZIP = sqlReader.GetString(2),
                        HOME_COUNT = sqlReader.GetInt32(3),
                        BUSINESS_COUNT = sqlReader.GetInt32(4),
                        APT_COUNT = sqlReader.GetInt32(5),
                        Geom = (SqlGeometry)sqlReader.GetValue(6),
                    });
                }

                var tree = new VectorTileTree();
                List<Feature> features = new List<Feature>();
                data.ForEach(i =>
                {
                    var geom = WKTReader.Read(i.Geom.ToString());
                    List<Polygon> areaPolygons = new List<Polygon>();
                    if(geom.GeometryType == "MultiPolygon")
                    {
                        areaPolygons = GeometryFactory.ToPolygonArray((geom as MultiPolygon).ToArray()).ToList();
                    }
                    else
                    {
                        areaPolygons = new List<Polygon> { (geom as Polygon) };
                    }

                    areaPolygons.ForEach(polygon => {
                        var fixedPolygon = polygon.Buffer(0);
                        fixedPolygon.Normalize();

                        var polygonFeature = new Feature
                        {
                            Geometry = fixedPolygon,
                            Attributes = new AttributesTable
                            {
                                { "id", $"{i.Layer}-{i.Id}" },
                                { "name", i.Name },
                                { "zip", i.ZIP },
                                { "home", i.HOME_COUNT },
                                { "apt", i.APT_COUNT },
                                { "business", i.BUSINESS_COUNT },
                            }
                        };
                        var pointFeature = new Feature
                        {
                            Geometry = fixedPolygon.InteriorPoint,
                            Attributes = new AttributesTable
                            {
                                { "id", $"{i.Layer}-label-{i.Id}" },
                                { "name", i.Name },
                                { "zip", i.ZIP },
                                { "home", i.HOME_COUNT },
                                { "apt", i.APT_COUNT },
                                { "business", i.BUSINESS_COUNT },
                            }
                        };

                        features.Add(polygonFeature);
                        features.Add(pointFeature);
                    });
                });

                tree.Add(features, z, layer.ToString());

                if (tree.Contains(tile.Id))
                {
                    return new MapboxTileResponse(tree[tile.Id], cacheFile, eTag);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"build tile for tiles/{layer}/{z}/{x}/{y} failed", ex);
            }
            finally
            {
                db.Database.Connection.Close();
            }

            return new EmptyWithNoCacheMapboxTileResponse();
        }

        [Route("zip/{code}")]
        [HttpGet]
        public dynamic SearchZip(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest();
            }
            var searchKey = code.Trim();
            if (searchKey.Length == 0)
            {
                return BadRequest();
            }
            Record result = null;
            switch (searchKey.Length)
            {
                case 3:
                    result = db.ThreeZipAreas
                        .Where(i => i.Code == searchKey)
                        .Select(i => new Record { Name = i.Name, APT = i.APT_COUNT, Home = i.HOME_COUNT, Geom = i.Geom })
                        .FirstOrDefault();
                    break;
                case 5:
                    result = db.FiveZipAreas
                        .Where(i => i.Code == searchKey)
                        .Select(i => new Record { Name = i.Name, APT = i.APT_COUNT, Home = i.HOME_COUNT, Geom = i.Geom })
                        .FirstOrDefault();
                    break;
                default:
                    result = db.PremiumCRoutes
                        .Where(i => i.Code == searchKey)
                         .Select(i => new Record { Name = i.Name, APT = i.APT_COUNT, Home = i.HOME_COUNT, Geom = i.Geom })
                        .FirstOrDefault();
                    break;
            }
            if (result.Geom != null)
            {
                var geom = WKTReader.Read(result.Geom.AsText());
                if (geom.GeometryType == "Polygon")
                {
                    return new
                    {
                        success = true,
                        result = new
                        {
                            name = result.Name,
                            apt = result.APT,
                            home = result.Home,
                            lat = geom.Centroid.Y,
                            lng = geom.Centroid.X,
                        }
                    };
                }
                else if (geom.GeometryType == "MultiPolygon")
                {
                    var data = ((MultiPolygon)geom).ToArray().Select(p => new {
                        name = result.Name,
                        apt = result.APT,
                        home = result.Home,
                        lat = p.Centroid.Y,
                        lng = p.Centroid.X,
                    });
                    return new { success = true, result = data };
                }
            }
            return new { success = false };
        }

        //[Route("tiles/3zip/build")]
        //[HttpGet]
        //public async Task<dynamic> Build3ZipTiles()
        //{
        //    db.Configuration.AutoDetectChangesEnabled = false;

        //    var mainTable = db.ThreeZipAreas;
        //    var detailTable = db.ThreeZipCoordinates;

        //    var batchSize = 1000;
        //    var count = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
        //    var loops = (int)Math.Ceiling((double)count / (double)batchSize);
        //    int failed = 0;
        //    for (var j = 0; j < loops; j++)
        //    {
        //        var areas = mainTable.Where(i => i.Geom == null)
        //            .OrderBy(i => i.Id)
        //            .Select(i => new { i.Id, i.IsInnerRing })
        //            .Take(batchSize)
        //            .Skip(failed)
        //            .ToDictionary(i => i.Id, i => i.IsInnerRing == 1 ? true : false);

        //        Dictionary<int?, KeyValuePair<bool, List<Coordinate>>> updateData = new Dictionary<int?, KeyValuePair<bool, List<Coordinate>>>();
        //        var ids = areas.Keys.ToList();
        //        var query = detailTable.Where(i => ids.Contains(i.ThreeZipAreaId))
        //            .OrderBy(i => i.ThreeZipAreaId)
        //            .ThenBy(i => i.Id)
        //            .Select(i => new { AreaId = i.ThreeZipAreaId, Longitude = i.Longitude ?? 0, Latitude = i.Latitude ?? 0 });

        //        foreach (var item in query)
        //        {
        //            if (!updateData.ContainsKey(item.AreaId))
        //            {
        //                var isRing = areas.ContainsKey(item.AreaId) ? areas[item.AreaId] : false;
        //                updateData.Add(item.AreaId, new KeyValuePair<bool, List<Coordinate>>(isRing, new List<Coordinate>()));
        //            }
        //            updateData[item.AreaId].Value.Add(new Coordinate(item.Longitude, item.Latitude));
        //        }


        //        foreach (var item in updateData)
        //        {
        //            try
        //            {
        //                Geometry geom = null;

        //                if (!item.Value.Key)
        //                {
        //                    //polygon
        //                    var startPoint = item.Value.Value.FirstOrDefault();
        //                    var endPoint = item.Value.Value.LastOrDefault();
        //                    if (!startPoint.Equals2D(endPoint))
        //                    {
        //                        item.Value.Value.Add(endPoint);
        //                    }
        //                    geom = GeometryFactory.CreatePolygon(item.Value.Value.ToArray());
        //                    if (!geom.IsValid)
        //                    {
        //                        geom = geom.Buffer(0);
        //                    }
        //                }
        //                else
        //                {
        //                    geom = GeometryFactory.CreateLineString(item.Value.Value.ToArray());
        //                }

        //                geom.Normalize();

        //                if (geom == null)
        //                {
        //                    failed++;
        //                    continue;
        //                }


        //                var dbItem = new Models.ThreeZipArea { Id = item.Key, Geom = DbGeometry.FromText(geom.AsText(), SRID_DB) };
        //                mainTable.Attach(dbItem);
        //                db.Entry(dbItem).Property("Geom").IsModified = true;
        //            }
        //            catch
        //            {
        //                failed++;
        //            }
        //        }
        //        db.SaveChanges();
        //    }

        //    var total = await mainTable.CountAsync().ConfigureAwait(false);
        //    var valid = await mainTable.Where(i => i.Geom.IsValid).CountAsync().ConfigureAwait(false);
        //    var empty = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
        //    db.Configuration.AutoDetectChangesEnabled = true;

        //    return new { success = true, total = total, empty = empty, valid = valid, failed = failed };
        //}

        //[Route("tiles/5zip/build")]
        //[HttpGet]
        //public async Task<dynamic> Build5ZipTiles()
        //{
        //    db.Configuration.AutoDetectChangesEnabled = false;

        //    var mainTable = db.FiveZipAreas;
        //    var detailTable = db.FiveZipCoordinates;

        //    var batchSize = 1000;
        //    var count = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
        //    var loops = (int)Math.Ceiling((double)count / (double)batchSize);
        //    int failed = 0;
        //    for (var j = 0; j < loops; j++)
        //    {
        //        var areas = mainTable.Where(i => i.Geom == null)
        //            .OrderBy(i => i.Id)
        //            .Select(i => new { i.Id, i.IsInnerRing })
        //            .Take(batchSize)
        //            .Skip(failed)
        //            .ToDictionary(i => i.Id, i => i.IsInnerRing == 1 ? true : false);

        //        Dictionary<int?, KeyValuePair<bool, List<Coordinate>>> updateData = new Dictionary<int?, KeyValuePair<bool, List<Coordinate>>>();
        //        var ids = areas.Keys.ToList();
        //        var query = detailTable.Where(i => ids.Contains(i.FiveZipAreaId))
        //            .OrderBy(i => i.FiveZipAreaId)
        //            .ThenBy(i => i.Id)
        //            .Select(i => new { AreaId = i.FiveZipAreaId, Longitude = i.Longitude ?? 0, Latitude = i.Latitude ?? 0 });

        //        foreach (var item in query)
        //        {
        //            if (!updateData.ContainsKey(item.AreaId))
        //            {
        //                var isRing = areas.ContainsKey(item.AreaId) ? areas[item.AreaId] : false;
        //                updateData.Add(item.AreaId, new KeyValuePair<bool, List<Coordinate>>(isRing, new List<Coordinate>()));
        //            }
        //            updateData[item.AreaId].Value.Add(new Coordinate(item.Longitude, item.Latitude));
        //        }


        //        foreach (var item in updateData)
        //        {
        //            try
        //            {
        //                Geometry geom = null;

        //                if (!item.Value.Key)
        //                {
        //                    //polygon
        //                    var startPoint = item.Value.Value.FirstOrDefault();
        //                    var endPoint = item.Value.Value.LastOrDefault();
        //                    if (!startPoint.Equals2D(endPoint))
        //                    {
        //                        item.Value.Value.Add(endPoint);
        //                    }
        //                    geom = GeometryFactory.CreatePolygon(item.Value.Value.ToArray());
        //                    if (!geom.IsValid)
        //                    {
        //                        geom = geom.Buffer(0);
        //                    }
        //                }
        //                else
        //                {
        //                    geom = GeometryFactory.CreateLineString(item.Value.Value.ToArray());
        //                }

        //                geom.Normalize();

        //                if (geom == null)
        //                {
        //                    failed++;
        //                    continue;
        //                }


        //                var dbItem = new Models.FiveZipArea { Id = item.Key, Geom = DbGeometry.FromText(geom.AsText(), SRID_DB) };
        //                mainTable.Attach(dbItem);
        //                db.Entry(dbItem).Property("Geom").IsModified = true;
        //            }
        //            catch
        //            {
        //                failed++;
        //            }
        //        }
        //        db.SaveChanges();
        //    }

        //    var total = await mainTable.CountAsync().ConfigureAwait(false);
        //    var valid = await mainTable.Where(i => i.Geom.IsValid).CountAsync().ConfigureAwait(false);
        //    var empty = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
        //    db.Configuration.AutoDetectChangesEnabled = true;

        //    return new { success = true, total = total, empty = empty, valid = valid, failed = failed };
        //}

        //[Route("tiles/croute/build")]
        //[HttpGet]
        //public async Task<dynamic> BuildCRouteTiles()
        //{
        //    db.Configuration.AutoDetectChangesEnabled = false;

        //    var mainTable = db.PremiumCRoutes;
        //    var detailTable = db.PremiumCRouteCoordinates;

        //    var batchSize = 1000;
        //    var count = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
        //    var loops = (int)Math.Ceiling((double)count / (double)batchSize);
        //    int failed = 0;
        //    for (var j = 0; j < loops; j++)
        //    {
        //        var areas = mainTable.Where(i => i.Geom == null)
        //            .OrderBy(i => i.Id)
        //            .Select(i => new { i.Id, i.IsInnerRing })
        //            .Take(batchSize)
        //            .Skip(failed)
        //            .ToDictionary(i => i.Id, i => i.IsInnerRing == 1 ? true : false);

        //        Dictionary<int?, KeyValuePair<bool, List<Coordinate>>> updateData = new Dictionary<int?, KeyValuePair<bool, List<Coordinate>>>();
        //        var ids = areas.Keys.ToList();
        //        var query = detailTable.Where(i => ids.Contains(i.PreminumCRouteId))
        //            .OrderBy(i => i.PreminumCRouteId)
        //            .ThenBy(i => i.Id)
        //            .Select(i => new { AreaId = i.PreminumCRouteId, Longitude = i.Longitude ?? 0, Latitude = i.Latitude ?? 0 });

        //        foreach (var item in query)
        //        {
        //            if (!updateData.ContainsKey(item.AreaId))
        //            {
        //                var isRing = areas.ContainsKey(item.AreaId) ? areas[item.AreaId] : false;
        //                updateData.Add(item.AreaId, new KeyValuePair<bool, List<Coordinate>>(isRing, new List<Coordinate>()));
        //            }
        //            updateData[item.AreaId].Value.Add(new Coordinate(item.Longitude, item.Latitude));
        //        }


        //        foreach (var item in updateData)
        //        {
        //            try
        //            {
        //                Geometry geom = null;

        //                if (!item.Value.Key)
        //                {
        //                    //polygon
        //                    var temp = GeometryFactory.CreateLineString(item.Value.Value.ToArray());
        //                    if (!temp.IsClosed)
        //                    {
        //                        temp.Normalize();
        //                        var coordinates = temp.Coordinates.ToList();
        //                        coordinates.Add(coordinates[0]);
        //                        geom = GeometryFactory.CreatePolygon(coordinates.ToArray());
        //                    }
        //                    else
        //                    {
        //                        geom = GeometryFactory.CreatePolygon(temp.Coordinates);
        //                    }

        //                    if (!geom.IsValid)
        //                    {
        //                        geom = geom.Buffer(0);
        //                    }
        //                }
        //                else
        //                {
        //                    geom = GeometryFactory.CreateLineString(item.Value.Value.ToArray());
        //                }

        //                geom.Normalize();

        //                if (geom == null)
        //                {
        //                    failed++;
        //                    continue;
        //                }


        //                var dbItem = new Models.PremiumCRoute { Id = item.Key, Geom = DbGeometry.FromText(geom.AsText(), SRID_DB) };
        //                mainTable.Attach(dbItem);
        //                db.Entry(dbItem).Property("Geom").IsModified = true;
        //            }
        //            catch
        //            {
        //                failed++;
        //            }
        //        }
        //        db.SaveChanges();
        //    }

        //    var total = await mainTable.CountAsync().ConfigureAwait(false);
        //    var valid = await mainTable.Where(i => i.Geom.IsValid).CountAsync().ConfigureAwait(false);
        //    var empty = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
        //    db.Configuration.AutoDetectChangesEnabled = true;

        //    return new { success = true, total = total, empty = empty, valid = valid, failed = failed };
        //}
    }
}