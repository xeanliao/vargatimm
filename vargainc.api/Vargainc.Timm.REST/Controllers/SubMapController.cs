using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using SharpKml.Base;
using SharpKml.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vargainc.Timm.EF;
using Vargainc.Timm.Models;

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
        public async Task<HttpResponseMessage> GetSubmapGeoJson(int campaignId)
        {
            var subMaps = await db.SubMaps.Where(i=>i.CampaignId == campaignId).ToListAsync().ConfigureAwait(false);
            var layers = new FeatureCollection();
            foreach (var subMap in subMaps)
            {
                var points = subMap.SubMapCoordinates.Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToList();
                // close it
                if (!points.First().Equals2D(points.Last()))
                {
                    points.Add(points.First());
                }
                var polygon = new Polygon(new LinearRing(points.ToArray()));
                polygon.Normalize();
                if (!polygon.IsValid)
                {
                    polygon = polygon.Buffer(0) as Polygon;
                }
                layers.Add(new Feature { 
                    Geometry = polygon,
                    BoundingBox = polygon.EnvelopeInternal,
                    Attributes = new AttributesTable
                    {
                        { "id", subMap.Id },
                        { "sid", subMap.Id },
                        { "name", subMap.Name},
                        { "color", $"#{subMap.ColorString}" }
                    }
                });
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
