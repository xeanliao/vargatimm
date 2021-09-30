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

            var subMaps = await db.SubMaps.Where(i=>i.CampaignId == campaignId).ToListAsync().ConfigureAwait(false);
            var subMapIds = subMaps.Select(i=>i.Id).ToList();
            var subMapRecords = await db.SubMapRecords
                .Where(i => subMapIds.Contains(i.SubMapId))
                .OrderBy(i => i.SubMapId)
                .ThenBy(i => i.Id)
                .Select(i=>new { i.AreaId, i.Classification})
                .ToListAsync()
                .ConfigureAwait(false);
            var subMapGeom = await db.SubMapCoordinates
                .Where(i => subMapIds.Contains(i.SubMapId))
                .OrderBy(i => i.SubMapId)
                .ThenBy(i => i.Id)
                .Select(i => new { i.SubMapId, i.Longitude, i.Latitude })
                .ToListAsync()
                .ConfigureAwait(false);

            foreach(var item in subMaps)
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
                        { "sid", item.Id },
                        { "name", item.Name },
                        { "color", $"#{item.ColorString}" },
                    }
                });
            }

            var dMaps = await db.DistributionMaps
                .Where(i=> subMapIds.Contains(i.SubMapId))
                .OrderBy(i=>i.SubMapId)
                .ThenBy(i=>i.Id)
                .ToListAsync()
                .ConfigureAwait(false);
            var dMapIds = dMaps.Select(i=>i.Id).ToList();
            var dMapGeom = await db.DistributionMapCoordinates
                .Where(i=>dMapIds.Contains(i.DistributionMapId))
                .OrderBy(i=>i.DistributionMapId)
                .ThenBy(i=>i.Id)
                .Select(i=> new { i.DistributionMapId, i.Longitude, i.Latitude })
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var item in dMaps)
            {
                var coorinates = dMapGeom.Where(i => i.DistributionMapId == item.Id).Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToList();
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
                        { "type", "dmap" },
                        { "id", $"dmap-{item.Id}" },
                        { "sid", item.Id },
                        { "name", item.Name },
                        { "color", $"#{item.ColorString}" },
                    }
                });
            }

            var recordGroup = subMapRecords
                .GroupBy(i => i.Classification)
                .ToDictionary(i => i.Key, i => i.Select(j=>j.AreaId).ToList());

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
                    layers.Add(new Feature
                    {
                        Geometry = geom,
                        BoundingBox = geom.EnvelopeInternal,
                        Attributes = new AttributesTable
                        {
                            { "type", "area" },
                            { "id", $"{((Classifications)item.Key).ToString()}-{sqlReader.GetInt32(0)}" },
                            { "name", sqlReader.GetString(1) },
                            { "zip", sqlReader.GetString(2) },
                            { "home", sqlReader.GetInt32(3) },
                            { "apt", sqlReader.GetInt32(5) },
                            { "business", sqlReader.GetInt32(4) },
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
    }
}