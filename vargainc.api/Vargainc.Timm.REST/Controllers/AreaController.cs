﻿using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.VectorTiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Vargainc.Timm.EF;

namespace Vargainc.Timm.REST.Controllers
{
    public class AreaController : ApiController
    {
        private TimmContext db = new TimmContext();

        private double Tile2Lng(int z, int x)
        {
            return (double)x / Math.Pow(2.0, z) * 360.0 - 180.0;
        }

        private double Tile2Lat(int z, int y)
        {
            double n = Math.PI - (2.0 * Math.PI * y) / (double)Math.Pow(2.0, z);
            return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
        }

        [Route("titles/{type}/{z}/{x}/{y}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetTiles(string type, int z, int x, int y)
        {
            var tree = new VectorTileTree();


            var topLeftLng = Tile2Lng(z, x);
            var topLeftLat = Tile2Lat(z, y);
            var bottomRightLng = Tile2Lng(z, x + 1);
            var bottomRightLat = Tile2Lat(z, y + 1);

            var ids = db.FiveZipAreas.Where(
                i => i.MinLongitude >= topLeftLng 
                && i.MinLongitude <= bottomRightLng 
                && i.MinLatitude >= bottomRightLat 
                && i.MinLatitude <= topLeftLat
            )
                .Select(i => i.Id)
                .ToList();

            var data = db.FiveZipCoordinates.Where(i => ids.Contains(i.FiveZipAreaId))
                .OrderBy(i => i.FiveZipAreaId)
                .ThenBy(i => i.Id)
                .Select(i => new { id = i.FiveZipAreaId, lnglat = new Coordinate(i.Longitude.Value, i.Latitude.Value) })
                .ToList();


            List<Geometry> result = new List<Geometry>();
            List<Coordinate> linearRing = null;
            int? lastId = null;
            foreach (var p in data)
            {
                if(p.id != lastId)
                {
                    if(linearRing.Count > 0)
                    {
                        var newLinearRing = new LinearRing(linearRing.ToArray());
                        if (!newLinearRing.IsClosed)
                        {
                            linearRing.Add(linearRing[0]);
                            newLinearRing = new LinearRing(linearRing.ToArray());
                        }
                        var polygon = new Polygon(newLinearRing);
                        polygon.SRID = lastId.Value;
                        result.Add(polygon);
                    }
                    lastId = p.id;
                    linearRing = new List<Coordinate>();
                }
                linearRing.Add(p.lnglat);
            }

            GeometryCollection geometryCollection = new GeometryCollection(result.ToArray());

            

            var serializer = GeoJsonSerializer.Create();
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, geometryCollection);
                
                ms.Seek(0, SeekOrigin.Begin);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(ms);
                response.Headers.Add("content-type", "application/geo+json");
                return response;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}