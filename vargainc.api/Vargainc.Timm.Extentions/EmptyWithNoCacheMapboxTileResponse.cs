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
    public class EmptyWithNoCacheMapboxTileResponse : IHttpActionResult
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MapboxTileResponse));

        public EmptyWithNoCacheMapboxTileResponse()
        {
           
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(String.Empty);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
            response.Headers.TryAddWithoutValidation("source", "empty");
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
                NoStore = true,
                NoTransform = true
            };
            return Task.FromResult(response);
        }
    }
}
