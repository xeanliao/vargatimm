using log4net;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Vargainc.Timm.Extentions;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("nd")]
    public class NdController : BaseController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(NdController));
        private static string GoogleMapKey = System.Configuration.ConfigurationManager.AppSettings["GoogleMapKey"];

        [Route("geojson")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllND()
        {
            var layers = new FeatureCollection();
            var area = await db.NdArea.Where(i=>i.Polygon != null).OrderByDescending(i => i.Id).ToListAsync().ConfigureAwait(false);
            var address = await db.NdAddresses.Where(i => i.Polygon != null).OrderByDescending(i => i.Id).ToListAsync().ConfigureAwait(false);

            foreach (NdArea item in area)
            {
                var geom = item.Polygon != null ? WKTReader.Read(item.Polygon.AsText()) : GeometryFactory.CreatePoint();
                if (geom.GeometryType == "GeometryCollection")
                {
                    Geometry fixGeom = null;
                    foreach(var child in (geom as GeometryCollection))
                    {
                        if(child.GeometryType == "Polygon")
                        {
                            fixGeom = fixGeom == null ? child : (child.Area > fixGeom.Area ? child : fixGeom);
                        }
                    }

                    geom = fixGeom;
                }
                layers.Add(new Feature
                {
                    Geometry = geom,
                    BoundingBox = geom.EnvelopeInternal,
                    Attributes = new AttributesTable
                    {
                        { "type", "area" },
                        { "oid", item.Id },
                        { "id", $"area-{item.Id}" },
                        { "name", item.Name },
                        { "desc", item.Description }
                    }
                });

                layers.Add(new Feature
                {
                    Geometry = geom.InteriorPoint,
                    Attributes = new AttributesTable
                    {
                        { "type", "area" },
                        { "oid", item.Id },
                        { "id", $"area-centroid-{item.Id}" },
                        { "name", item.Name },
                        { "desc", item.Description }
                    }
                });
            }

            foreach (var item in address)
            {
                var geom = item.Polygon != null ? WKTReader.Read(item.Polygon.AsText()) : GeometryFactory.CreatePoint();
                layers.Add(new Feature
                {
                    Geometry = geom,
                    BoundingBox = geom.EnvelopeInternal,
                    Attributes = new AttributesTable
                    {
                        { "type", "address" },
                        { "oid", item.Id },
                        { "id", $"address-{item.Id}" },
                        { "name", $"{item.Street} {item.ZipCode}" },
                        { "desc", item.Description }
                    }
                });

                layers.Add(new Feature
                {
                    Geometry = geom.InteriorPoint,
                    Attributes = new AttributesTable
                    {
                        { "type", "address" },
                        { "oid", item.Id },
                        { "id", $"address-centroid-{item.Id}" },
                        { "name", $"{item.Street} {item.ZipCode}" },
                        { "desc", item.Description }
                    }
                });
            }
            var eTag = GetETag();
            return new GeoJsonResponse(layers, eTag);
        }

        [Route("area")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateCustomArea([FromBody] dynamic data)
        {
            string name = data?.name;
            string desc = data?.desc;
            int total = data?.total;
            JArray geom = data?.geom;

            List<Coordinate> coordinates = new List<Coordinate>();
            foreach (dynamic row in geom)
            {
                double lng = (double)((JValue)(row as JArray)[0]).Value;
                double lat = (double)((JValue)(row as JArray)[1]).Value;
                coordinates.Add(new Coordinate(lng, lat));
            }
            var polygon = GeometryFactory.CreatePolygon(coordinates.ToArray());

            db.NdArea.Add(new NdArea()
            {
                Name = name,
                Description = desc,
                Total = total,
                Polygon = DbGeometry.FromText(polygon.AsText())
            });
            await db.SaveChangesAsync().ConfigureAwait(false);

            return Json(new { success = true });
        }

        [Route("address")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateAddress([FromBody] dynamic data)
        {
            string name = data?.name;
            string desc = data?.desc;
            string street  = data?.street;
            string zipCode = data?.zipCode;
            int geofence = data?.geofence;

            var lnglat = await GeocodingFromGoogle(street, zipCode).ConfigureAwait(false);

            Point center = GeometryFactory.CreatePoint(lnglat);
            Geometry polygon = center.Buffer((double)geofence);

            db.NdAddresses.Add(new NdAddress()
            {
                Street = street,
                ZipCode = zipCode,
                Geofence = geofence,
                Description = desc,
                Polygon = DbGeometry.FromText(polygon.AsText())
            });

            await db.SaveChangesAsync().ConfigureAwait(false);

            return Json(new { success = true });
        }

        private static async Task<Coordinate> GeocodingFromGoogle(string address, string zipCode)
        {
            var url = $"http://maps.googleapis.com/maps/api/geocode/json?address={HttpUtility.UrlEncode($"{address}, {zipCode}")}&key={GoogleMapKey}";
            using (HttpClient client = new HttpClient())
            {
                var body = await client.GetStringAsync(url).ConfigureAwait(false);
                dynamic data = JObject.Parse(body);
                double lat = data?.lat;
                double lng = data?.lng;
                return new Coordinate(lng, lat);
            }
        }
    }
}