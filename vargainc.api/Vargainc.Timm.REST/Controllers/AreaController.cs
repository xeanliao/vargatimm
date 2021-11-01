using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.VectorTiles;
using NetTopologySuite.IO.VectorTiles.Mapbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
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
        public async Task<HttpResponseMessage> GetTiles(Classifications layer, int z, int x, int y)
        {
            if (!LAYERS.Contains(layer))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage response = null;

            var cacheFile = Path.Combine(CachePath, layer.ToString(), z.ToString(), $"{x}.{y}.pbf");
            //try
            //{
            //    if (File.Exists(cacheFile))
            //    {
            //        byte[] content = File.ReadAllBytes(cacheFile);
            //        string eTag = null;
            //        eTag = GenerateETag(content);
            //        var eTagCheck = GetETag();
            //        if (eTag == eTagCheck)
            //        {
            //            return new HttpResponseMessage(HttpStatusCode.NotModified);
            //        }

            //        response = new HttpResponseMessage(HttpStatusCode.OK);
            //        response.Headers.TryAddWithoutValidation("ETag", eTag);
            //        response.Headers.TryAddWithoutValidation("profile", "cache");
            //        response.Content = new ByteArrayContent(content);
            //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");

            //        response.Headers.CacheControl = new CacheControlHeaderValue()
            //        {
            //            Public = true,
            //            MaxAge = new TimeSpan(365, 0, 0, 0),
            //        };

            //        return response;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    logger.Error("Load cache file failed. continue...", ex);
            //}

            long database = 0;
            long process = 0;
            Stopwatch watch = new Stopwatch();
            watch.Start();

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
                        sql.AppendLine("SELECT [Id],[Code] AS [Name],'' AS [ZIP],[HOME_COUNT],[BUSINESS_COUNT],[APT_COUNT],[IsMaster],[IsInnerRing], [Latitude],[Longitude],[Geom]");
                        sql.AppendLine("FROM [dbo].[threezipareas] WITH(INDEX(threezipareas_spatial_index))");
                        sql.AppendLine("WHERE geom.STIntersects(@g) = 1");
                        break;
                    case Classifications.Z5:
                        sql.AppendLine("SELECT [Id],[Code] AS [Name],[Code] AS [ZIP],[HOME_COUNT],[BUSINESS_COUNT],[APT_COUNT],[IsMaster],[IsInnerRing],[Latitude],[Longitude],[Geom]");
                        sql.AppendLine("FROM [dbo].[fivezipareas] WITH(INDEX(fivezipareas_spatial_index))");
                        sql.AppendLine("WHERE geom.STIntersects(@g) = 1");
                        break;
                    case Classifications.PremiumCRoute:
                        sql.AppendLine("SELECT [Id],[GEOCODE] AS [Name],[ZIP],[HOME_COUNT],[BUSINESS_COUNT],[APT_COUNT],[IsMaster],[IsInnerRing],[Latitude],[Longitude],[Geom]");
                        sql.AppendLine("FROM [dbo].[premiumcroutes] WITH(INDEX(premiumcroutes_spatial_index))");
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
                        IsMaster = sqlReader.IsDBNull(6) ? false : sqlReader.GetBoolean(6),
                        IsInnerRing = sqlReader.IsDBNull(7) ? false : sqlReader.GetByte(7) == 1,
                        Latitude = sqlReader.GetDouble(8),
                        Longitude = sqlReader.GetDouble(9),
                        Geom = (SqlGeometry)sqlReader.GetValue(10),
                    });
                }


                database = watch.ElapsedMilliseconds;
                watch.Restart();

                var tree = new VectorTileTree();

                var polygons = data.Select(i =>
                {
                    var geom = WKTReader.Read(i.Geom.ToString());
                    if (i.IsMaster)
                    {
                        var center = geom.InteriorPoint;
                        i.Center = new double[] { center.X, center.Y };
                    }

                    return new Feature
                    {
                        Geometry = geom,
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
                }).ToList();

                var points = data.Where(i => i.IsMaster == true).Select(i =>
                {
                    return new Feature
                    {
                        Geometry = GeometryFactory.CreatePoint(new Coordinate(i.Center[0], i.Center[1])),
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
                }).ToList();

                var features = polygons.Concat(points);
                tree.Add(features, z, layer.ToString());

                response = new HttpResponseMessage(HttpStatusCode.OK);


                if (tree.Contains(tile.Id))
                {
                    byte[] content = null;
                    string eTag = null;
                    using (var ms = new MemoryStream())
                    {
                        tree[tile.Id].Write(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        content = ms.ToArray();
                    }

                    //try
                    //{
                    //    if (!Directory.Exists(Path.GetDirectoryName(cacheFile)))
                    //    {
                    //        Directory.CreateDirectory(Path.GetDirectoryName(cacheFile));
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    logger.Error("Create cache file directory failed. continue...", ex);
                    //}

                    //try
                    //{
                    //    File.WriteAllBytes(cacheFile, content);
                    //}
                    //catch (Exception ex)
                    //{
                    //    logger.Error("Create cache file failed. continue...", ex);
                    //}

                    eTag = GenerateETag(content);
                    var eTagCheck = GetETag();
                    if (eTag == eTagCheck)
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotModified);
                    }
                    response.Headers.TryAddWithoutValidation("ETag", eTag);
                    response.Headers.TryAddWithoutValidation("profile", "database");
                    response.Content = new ByteArrayContent(content);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");

                    response.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = new TimeSpan(365, 0, 0, 0),
                    };
                }
                else
                {
                    response.Content = new StringContent(string.Empty, Encoding.UTF8, "application/x-protobuf");

                    response.Headers.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true,
                        NoStore = true,
                        NoTransform = true
                    };
                }
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString(), Encoding.UTF8, "html/text");
            }
            finally
            {
                db.Database.Connection.Close();
            }

            watch.Stop();
            process = watch.ElapsedMilliseconds;
            response.Headers.TryAddWithoutValidation("profile-database", database.ToString());
            response.Headers.TryAddWithoutValidation("profile-process", process.ToString());

            return response;
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
            if(searchKey.Length == 0)
            {
                return BadRequest();
            }
            Object result = null;
            switch(searchKey.Length){
                case 3:
                    result = db.ThreeZipAreas
                        .Where(i => i.Code == searchKey)
                        .Select(i => new { name = i.Code, apt = i.APT_COUNT, home = i.HOME_COUNT, lat = i.Latitude, lng = i.Longitude })
                        .FirstOrDefault();
                    break;
                    case 5:
                    result = db.FiveZipAreas
                        .Where(i=>i.Code == searchKey)
                        .Select(i => new { name = i.Code, apt = i.APT_COUNT, home = i.HOME_COUNT, lat = i.Latitude, lng = i.Longitude })
                        .FirstOrDefault();
                    break;
                    default:
                    result = db.PremiumCRoutes
                        .Where(i=>i.GEOCODE == searchKey)
                        .Select(i => new { name = i.GEOCODE, apt = i.APT_COUNT, home = i.HOME_COUNT, lat = i.Latitude, lng = i.Longitude })
                        .FirstOrDefault();
                    break;
            }
            return new { success = result != null, result = result };
        }

        [Route("tiles/3zip/build")]
        [HttpGet]
        public async Task<dynamic> Build3ZipTiles()
        {
            db.Configuration.AutoDetectChangesEnabled = false;

            var mainTable = db.ThreeZipAreas;
            var detailTable = db.ThreeZipCoordinates;

            var batchSize = 1000;
            var count = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
            var loops = (int)Math.Ceiling((double)count / (double)batchSize);
            int failed = 0;
            for (var j = 0; j < loops; j++)
            {
                var areas = mainTable.Where(i => i.Geom == null)
                    .OrderBy(i => i.Id)
                    .Select(i => new { i.Id, i.IsInnerRing })
                    .Take(batchSize)
                    .Skip(failed)
                    .ToDictionary(i => i.Id, i => i.IsInnerRing == 1 ? true : false);

                Dictionary<int?, KeyValuePair<bool, List<Coordinate>>> updateData = new Dictionary<int?, KeyValuePair<bool, List<Coordinate>>>();
                var ids = areas.Keys.ToList();
                var query = detailTable.Where(i => ids.Contains(i.ThreeZipAreaId))
                    .OrderBy(i => i.ThreeZipAreaId)
                    .ThenBy(i => i.Id)
                    .Select(i => new { AreaId = i.ThreeZipAreaId, Longitude = i.Longitude ?? 0, Latitude = i.Latitude ?? 0 });

                foreach (var item in query)
                {
                    if (!updateData.ContainsKey(item.AreaId))
                    {
                        var isRing = areas.ContainsKey(item.AreaId) ? areas[item.AreaId] : false;
                        updateData.Add(item.AreaId, new KeyValuePair<bool, List<Coordinate>>(isRing, new List<Coordinate>()));
                    }
                    updateData[item.AreaId].Value.Add(new Coordinate(item.Longitude, item.Latitude));
                }


                foreach (var item in updateData)
                {
                    try
                    {
                        Geometry geom = null;

                        if (!item.Value.Key)
                        {
                            //polygon
                            var startPoint = item.Value.Value.FirstOrDefault();
                            var endPoint = item.Value.Value.LastOrDefault();
                            if (!startPoint.Equals2D(endPoint))
                            {
                                item.Value.Value.Add(endPoint);
                            }
                            geom = GeometryFactory.CreatePolygon(item.Value.Value.ToArray());
                            if (!geom.IsValid)
                            {
                                geom = geom.Buffer(0);
                            }
                        }
                        else
                        {
                            geom = GeometryFactory.CreateLineString(item.Value.Value.ToArray());
                        }

                        geom.Normalize();

                        if (geom == null)
                        {
                            failed++;
                            continue;
                        }


                        var dbItem = new Models.ThreeZipArea { Id = item.Key, Geom = DbGeometry.FromText(geom.AsText(), SRID_DB) };
                        mainTable.Attach(dbItem);
                        db.Entry(dbItem).Property("Geom").IsModified = true;
                    }
                    catch
                    {
                        failed++;
                    }
                }
                db.SaveChanges();
            }

            var total = await mainTable.CountAsync().ConfigureAwait(false);
            var valid = await mainTable.Where(i => i.Geom.IsValid).CountAsync().ConfigureAwait(false);
            var empty = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
            db.Configuration.AutoDetectChangesEnabled = true;

            return new { success = true, total = total, empty = empty, valid = valid, failed = failed };
        }

        [Route("tiles/5zip/build")]
        [HttpGet]
        public async Task<dynamic> Build5ZipTiles()
        {
            db.Configuration.AutoDetectChangesEnabled = false;

            var mainTable = db.FiveZipAreas;
            var detailTable = db.FiveZipCoordinates;

            var batchSize = 1000;
            var count = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
            var loops = (int)Math.Ceiling((double)count / (double)batchSize);
            int failed = 0;
            for (var j = 0; j < loops; j++)
            {
                var areas = mainTable.Where(i => i.Geom == null)
                    .OrderBy(i => i.Id)
                    .Select(i => new { i.Id, i.IsInnerRing })
                    .Take(batchSize)
                    .Skip(failed)
                    .ToDictionary(i => i.Id, i => i.IsInnerRing == 1 ? true : false);

                Dictionary<int?, KeyValuePair<bool, List<Coordinate>>> updateData = new Dictionary<int?, KeyValuePair<bool, List<Coordinate>>>();
                var ids = areas.Keys.ToList();
                var query = detailTable.Where(i => ids.Contains(i.FiveZipAreaId))
                    .OrderBy(i => i.FiveZipAreaId)
                    .ThenBy(i => i.Id)
                    .Select(i => new { AreaId = i.FiveZipAreaId, Longitude = i.Longitude ?? 0, Latitude = i.Latitude ?? 0 });

                foreach (var item in query)
                {
                    if (!updateData.ContainsKey(item.AreaId))
                    {
                        var isRing = areas.ContainsKey(item.AreaId) ? areas[item.AreaId] : false;
                        updateData.Add(item.AreaId, new KeyValuePair<bool, List<Coordinate>>(isRing, new List<Coordinate>()));
                    }
                    updateData[item.AreaId].Value.Add(new Coordinate(item.Longitude, item.Latitude));
                }


                foreach (var item in updateData)
                {
                    try
                    {
                        Geometry geom = null;

                        if (!item.Value.Key)
                        {
                            //polygon
                            var startPoint = item.Value.Value.FirstOrDefault();
                            var endPoint = item.Value.Value.LastOrDefault();
                            if (!startPoint.Equals2D(endPoint))
                            {
                                item.Value.Value.Add(endPoint);
                            }
                            geom = GeometryFactory.CreatePolygon(item.Value.Value.ToArray());
                            if (!geom.IsValid)
                            {
                                geom = geom.Buffer(0);
                            }
                        }
                        else
                        {
                            geom = GeometryFactory.CreateLineString(item.Value.Value.ToArray());
                        }

                        geom.Normalize();

                        if (geom == null)
                        {
                            failed++;
                            continue;
                        }


                        var dbItem = new Models.FiveZipArea { Id = item.Key, Geom = DbGeometry.FromText(geom.AsText(), SRID_DB) };
                        mainTable.Attach(dbItem);
                        db.Entry(dbItem).Property("Geom").IsModified = true;
                    }
                    catch
                    {
                        failed++;
                    }
                }
                db.SaveChanges();
            }

            var total = await mainTable.CountAsync().ConfigureAwait(false);
            var valid = await mainTable.Where(i => i.Geom.IsValid).CountAsync().ConfigureAwait(false);
            var empty = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
            db.Configuration.AutoDetectChangesEnabled = true;

            return new { success = true, total = total, empty = empty, valid = valid, failed = failed };
        }

        [Route("tiles/croute/build")]
        [HttpGet]
        public async Task<dynamic> BuildCRouteTiles()
        {
            db.Configuration.AutoDetectChangesEnabled = false;

            var mainTable = db.PremiumCRoutes;
            var detailTable = db.PremiumCRouteCoordinates;

            var batchSize = 1000;
            var count = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
            var loops = (int)Math.Ceiling((double)count / (double)batchSize);
            int failed = 0;
            for (var j = 0; j < loops; j++)
            {
                var areas = mainTable.Where(i => i.Geom == null)
                    .OrderBy(i => i.Id)
                    .Select(i => new { i.Id, i.IsInnerRing })
                    .Take(batchSize)
                    .Skip(failed)
                    .ToDictionary(i => i.Id, i => i.IsInnerRing == 1 ? true : false);

                Dictionary<int?, KeyValuePair<bool, List<Coordinate>>> updateData = new Dictionary<int?, KeyValuePair<bool, List<Coordinate>>>();
                var ids = areas.Keys.ToList();
                var query = detailTable.Where(i => ids.Contains(i.PreminumCRouteId))
                    .OrderBy(i => i.PreminumCRouteId)
                    .ThenBy(i => i.Id)
                    .Select(i => new { AreaId = i.PreminumCRouteId, Longitude = i.Longitude ?? 0, Latitude = i.Latitude ?? 0 });

                foreach (var item in query)
                {
                    if (!updateData.ContainsKey(item.AreaId))
                    {
                        var isRing = areas.ContainsKey(item.AreaId) ? areas[item.AreaId] : false;
                        updateData.Add(item.AreaId, new KeyValuePair<bool, List<Coordinate>>(isRing, new List<Coordinate>()));
                    }
                    updateData[item.AreaId].Value.Add(new Coordinate(item.Longitude, item.Latitude));
                }


                foreach (var item in updateData)
                {
                    try
                    {
                        Geometry geom = null;

                        if (!item.Value.Key)
                        {
                            //polygon
                            var temp = GeometryFactory.CreateLineString(item.Value.Value.ToArray());
                            if (!temp.IsClosed)
                            {
                                temp.Normalize();
                                var coordinates = temp.Coordinates.ToList();
                                coordinates.Add(coordinates[0]);
                                geom = GeometryFactory.CreatePolygon(coordinates.ToArray());
                            }
                            else
                            {
                                geom = GeometryFactory.CreatePolygon(temp.Coordinates);
                            }

                            if (!geom.IsValid)
                            {
                                geom = geom.Buffer(0);
                            }
                        }
                        else
                        {
                            geom = GeometryFactory.CreateLineString(item.Value.Value.ToArray());
                        }

                        geom.Normalize();

                        if (geom == null)
                        {
                            failed++;
                            continue;
                        }


                        var dbItem = new Models.PremiumCRoute { Id = item.Key, Geom = DbGeometry.FromText(geom.AsText(), SRID_DB) };
                        mainTable.Attach(dbItem);
                        db.Entry(dbItem).Property("Geom").IsModified = true;
                    }
                    catch
                    {
                        failed++;
                    }
                }
                db.SaveChanges();
            }

            var total = await mainTable.CountAsync().ConfigureAwait(false);
            var valid = await mainTable.Where(i => i.Geom.IsValid).CountAsync().ConfigureAwait(false);
            var empty = await mainTable.Where(i => i.Geom == null).CountAsync().ConfigureAwait(false);
            db.Configuration.AutoDetectChangesEnabled = true;

            return new { success = true, total = total, empty = empty, valid = valid, failed = failed };
        }
    }
}