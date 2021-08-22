using System;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Vargainc.Timm.EF;
using NetTopologySuite.Geometries;
using NetTopologySuite;


namespace Vargainc.Timm.REST.Controllers
{
    public class BaseController : ApiController
    {
        protected const int SRID_DB = 4326;
        protected const int SRID_MAP = 3857;
        /// <summary>
        /// Default 3857 GeometryFactory
        /// </summary>
        protected static GeometryFactory GeometryFactory;

        protected static NetTopologySuite.IO.WKTReader WKTReader;
        static BaseController()
        {
            NtsGeometryServices.Instance = new NtsGeometryServices(
                NetTopologySuite.Geometries.Implementation.CoordinateArraySequenceFactory.Instance,
                new PrecisionModel(PrecisionModels.Floating),
                SRID_DB);

            GeometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory();
            WKTReader = new NetTopologySuite.IO.WKTReader(NtsGeometryServices.Instance);
        }

        public BaseController()
        {
            db = new TimmContext();
        }

        protected TimmContext db;

        protected new JsonResult<T> Json<T>(T content)
        {
            JsonSerializerSettings config = new JsonSerializerSettings() {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "MM-dd-yyyy",
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            config.Converters.Add(new StringEnumConverter());
            return base.Json<T>(content, config, Encoding.UTF8);
        }

        protected string GetETag()
        {
            Request.Headers.TryGetValues("If-None-Match", out var values);
            string etag = values?.FirstOrDefault();
            return etag;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected static string GenerateETag(byte[] inputBytes)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}