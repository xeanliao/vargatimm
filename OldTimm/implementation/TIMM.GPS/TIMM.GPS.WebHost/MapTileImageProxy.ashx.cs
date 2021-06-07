using System;
using System.Web;
using System.IO;
using System.Net;
using System.Configuration;

namespace TIMM.GPS.WebHost
{
    /// <summary>
    /// Summary description for MapTileImageProxy
    /// </summary>
    public class MapTileImageProxy : IHttpHandler
    {
        //{0}:mode; {1}: center; {2}: zoom {3}:size; {4}:BingMapsKey;
        private const string BingMapImageAPIBaseURL = @"http://dev.virtualearth.net/REST/v1/Imagery/Map/{0}/{1}/{2}?mapSize={3}&key={4}&format=jpeg";
        ////{0}:mode; {1}: area; {2}:size; {4}:BingMapsKey;
        private const string BingMapImageAPIBaseURL2 = @"http://dev.virtualearth.net/REST/v1/Imagery/Map/{0}?mapArea={1}&mapSize={2}&key={3}&format=jpeg";
        private const string GoogleMapImageAPIBaseURL = @"http://maps.googleapis.com/maps/api/staticmap?center={0}&zoom={1}&size={2}&maptype=roadmap&sensor=false&scale=4";
        private const bool UseGoogleMapImageService = false;


        public void ProcessRequest(HttpContext context)
        {
            var mode = context.Request["mode"];
            var size = context.Request["size"];
            
            if (string.IsNullOrWhiteSpace(mode) ||
                string.IsNullOrWhiteSpace(size))
            {
                context.Response.Clear();
                context.Response.Write("paramter error!");
                context.Response.End();
            }

            var center = context.Request["center"];
            var zoom = context.Request["zoom"];

            var area = context.Request["mapArea"];

            try
            {
                context.Response.ContentType = "image/jpeg";
                using (Stream imageStream = DownloadMap(mode, size, area, center, zoom))
                {
                    byte[] buffer = new byte[4096];
                    int readed = 0;
                    do
                    {
                        readed = imageStream.Read(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Write(buffer, 0, readed);
                    } while (readed > 0);

                }
            }
            catch
            {

            }
            finally
            {
                context.Response.OutputStream.Flush();
                context.Response.End();
            }
        }

        private Stream DownloadMap(string mode, string size, string mapArea, string center, string zoom)
        {
            MemoryStream imageStream = new MemoryStream();
            HttpWebRequest request;
            if (UseGoogleMapImageService)
            {
                request = (HttpWebRequest)WebRequest.Create(string.Format(GoogleMapImageAPIBaseURL, center, zoom, size.Replace(",", "x")));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(mapArea))
                {
                    request = (HttpWebRequest)WebRequest.Create(string.Format(BingMapImageAPIBaseURL2, mode, mapArea, size,
                        ConfigurationManager.AppSettings["BinMapKey"]));
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(string.Format(BingMapImageAPIBaseURL, mode, center, zoom, size,
                        ConfigurationManager.AppSettings["BinMapKey"]));
                }
            }
            request.PreAuthenticate = true;
            request.UseDefaultCredentials = false;
            request.AllowAutoRedirect = true;


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (var responseStream = response.GetResponseStream())
            {
                byte[] buffer = new byte[4096];
                int readed = 0;

                do
                {
                    readed = responseStream.Read(buffer, 0, buffer.Length);
                    imageStream.Write(buffer, 0, readed);
                } while (readed > 0);
            }
            imageStream.Seek(0, SeekOrigin.Begin);
            return imageStream;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}