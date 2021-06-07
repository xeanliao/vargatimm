using System;
using System.Web;
using System.IO;
using System.Net;
using System.Configuration;

namespace TIMM.GPS.HttpHandler
{
    public class MapTileImageService : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        //{0}:center; {1}: zoomLevel; {2}:mapSize; {3}:BingMapsKey;
        private const string BingMapImageAPIBaseURL = @"http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/{0}/{1}?mapSize={2}&key={3}";
        private const string GoogleMapImageAPIBaseURL = @"http://maps.googleapis.com/maps/api/staticmap?center={0}&zoom={1}&size={2}&maptype=roadmap&sensor=false&scale=4";
        private const bool UseGoogleMapImageService = false;

        public void ProcessRequest(HttpContext context)
        {
            
            var size = context.Request["size"];
            var center = context.Request["center"];
            var zoom = context.Request["zoom"];
            if (string.IsNullOrWhiteSpace(size) ||
                string.IsNullOrWhiteSpace(center)||
                string.IsNullOrWhiteSpace(zoom))
            {
                context.Response.Clear();
                context.Response.Write("paramter error!");
                context.Response.End();
            }

            try
            {
                context.Response.ContentType = "image/jpeg";
                using (Stream imageStream = DownloadMap(size, center, zoom))
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
            catch (Exception ex)
            {
                context.Response.Clear();
                context.Response.Write(ex.ToString());
            }
            finally
            {
                context.Response.OutputStream.Flush();
                context.Response.End();
            }

        }


        private Stream DownloadMap(string size, string center, string zoom)
        {
            HttpContext.Current.Response.Write("begin get map");
            MemoryStream imageStream = new MemoryStream();
            HttpWebRequest request;
            if (UseGoogleMapImageService)
            {
                request = (HttpWebRequest)WebRequest.Create(string.Format(GoogleMapImageAPIBaseURL, center, zoom, size.Replace(",", "x")));
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(string.Format(BingMapImageAPIBaseURL, center, zoom, size,
                    ConfigurationManager.AppSettings["BingMapKey"]));
            }
            request.PreAuthenticate = true;
            request.UseDefaultCredentials = true;
            HttpContext.Current.Response.Write("begin get response");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            HttpContext.Current.Response.Write("end get response");
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

        #endregion
    }
}
