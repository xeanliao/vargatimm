using NetTopologySuite.Features;
using NetTopologySuite.IO;
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
    public class GeoJsonResponse : IHttpActionResult
    {
        public GeoJsonResponse(FeatureCollection layers, string etag)
        {
            this._layers = layers;
        }

        private FeatureCollection _layers;
        private string _eTag;

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            MemoryStream ms = new MemoryStream();
            var serializer = GeoJsonSerializer.Create();
            var streamWriter = new StreamWriter(ms, Encoding.Default, 1024, true);

            serializer.Serialize(streamWriter, this._layers);
            streamWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var eTag = Convert.ToBase64String(MD5.Create().ComputeHash(ms));

            if (eTag == this._eTag)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotModified));
            }

            ms.Seek(0, SeekOrigin.Begin);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.TryAddWithoutValidation("ETag", eTag);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/geo+json");
            return Task.FromResult(response);
        }
    }
}
