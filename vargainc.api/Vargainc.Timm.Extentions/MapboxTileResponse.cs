using log4net;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using NetTopologySuite.IO.VectorTiles;
using NetTopologySuite.IO.VectorTiles.Mapbox;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;


namespace Vargainc.Timm.Extentions
{
    public class MapboxTileResponse : IHttpActionResult
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MapboxTileResponse));

        public MapboxTileResponse(string cachePath, string etag)
        {
            this._eTag = etag;
            this._cachePath = cachePath;
        }
        public MapboxTileResponse(VectorTile tile, string cachePath, string etag)
        {
            this._tile = tile;
            this._eTag = etag;
            this._cachePath = cachePath;
        }

        private VectorTile _tile;
        private string _eTag;
        private string _cachePath;

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            Stream result = null;
            if(this._tile == null)
            {
                response.Headers.TryAddWithoutValidation("source", "disk");
                result = new FileStream(this._cachePath, FileMode.Open, FileAccess.Read);
            }
            else
            {
                response.Headers.TryAddWithoutValidation("source", "database");
                result = new MemoryStream();
                this._tile.Write(result);
                result.Seek(0, SeekOrigin.Begin);
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(this._cachePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(this._cachePath));
                    }

                    using (var fs = new FileStream(this._cachePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        await result.CopyToAsync(fs).ConfigureAwait(false);
                        await fs.FlushAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Create cache file failed. continue...", ex);
                }
            }

            result.Seek(0, SeekOrigin.Begin);

            var eTag = Convert.ToBase64String(MD5.Create().ComputeHash(result));

            if (eTag == this._eTag)
            {
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }

            result.Seek(0, SeekOrigin.Begin);
            
            response.Headers.TryAddWithoutValidation("ETag", eTag);
            response.Content = new StreamContent(result);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = new TimeSpan(365, 0, 0, 0),
            };
            return response;
        }
    }
}
