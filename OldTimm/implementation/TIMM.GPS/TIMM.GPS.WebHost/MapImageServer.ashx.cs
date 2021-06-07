using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Configuration;

namespace TIMM.GPS.WebHost
{
    /// <summary>
    /// MapImageServer use bing map address:http://ecn.t1.tiles.virtualearth.net/tiles/{r}{key}.png?g=2045&mkt=en-US&shading=hill&n=z
    /// </summary>
    public class MapImageServer : IHttpHandler
    {
        private static string BingMapVersion = ConfigurationManager.AppSettings["BingMapVersion"];
        private const string BaseURL = "http://ecn.t{0}.tiles.virtualearth.net/tiles/{1}{2}.png?g={3}&mkt=en-US&shading=hill&n=z";
        private static string CachePath = ConfigurationManager.AppSettings["BingMapTileImageCachePath"];

        private struct Location
        {
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }

        private struct Point
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        private void BadRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = 400;
            context.Response.End();
        }

        private static byte[] Cache(string key, string mapMode, int serial = 0)
        {
            byte[] result = null;
            string folder = Path.Combine(CachePath, BingMapVersion);
            
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string path = Path.Combine(folder, string.Format(@"{0}_{1}.png", mapMode, key));

                if (File.Exists(path))
                {
                    using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
                    using (BinaryReader reader = new BinaryReader(file))
                    {
                        result = reader.ReadBytes((int)file.Length);
                    }
                }
                else
                {
                    byte[] cache = DownloadFile(key, mapMode, serial);
                    if (cache != null && cache.Length > 0)
                    {
                        using (FileStream cacheStream = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
                        {
                            cacheStream.Write(cache, 0, cache.Length);
                            result = cache;
                        }
                    }
                }
            }
            catch
            {
                result = DownloadFile(key, mapMode, serial);
            }

            return result;
        }

        private static byte[] DownloadFile(string key, string mapMode, int serial)
        {
            try
            {
                var url = string.Format(BaseURL,
                                serial % 3,//bing map tile image server load blance serial (0, 3)
                                mapMode,
                                key,
                                BingMapVersion);
                var request = HttpWebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var responseStream = response.GetResponseStream())
                    using (MemoryStream msStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];
                        int readed = 0;
                        do
                        {
                            readed = responseStream.Read(buffer, 0, buffer.Length);
                            msStream.Write(buffer, 0, readed);
                        } while (readed > 0);
                        return msStream.ToArray();
                    }
                }
            }
            catch { }
            return null;
        }

        public void ProcessRequest(HttpContext context)
        {
            #region Request paramter
            var request = context.Request;
            int zoomLevel;
            if(!int.TryParse(request["zoom"], out zoomLevel))
            {
                BadRequest(context);
                return;
            }
            int dpi;
            if (string.IsNullOrWhiteSpace(request["dpi"]) || !int.TryParse(request["dpi"], out dpi) || dpi < 96)
            {
                dpi = 96;
            }

            var mode = request["mode"];
            //a:Aerial; r:Road; h:Aerial With Labels
            switch (mode)
            {
                case "Road":
                    mode = "r";
                    break;
                case "AerialWithLabels":
                    mode = "h";
                    break;
                case "Aerial":
                    mode = "a";
                    break;
                default:
                    mode = "r";
                    break;
            }

            double leftBottomLongitude, leftBottomLatitude;
            if (!string.IsNullOrWhiteSpace(request["leftBottom"]))
            {
                var leftBottomArray = request["leftBottom"].Split(',');
                if(leftBottomArray.Length != 2 
                    || !double.TryParse(leftBottomArray[0], out leftBottomLatitude)
                    || !double.TryParse(leftBottomArray[1], out leftBottomLongitude))
                {
                    BadRequest(context);
                    return;
                }
            }else
            {
                BadRequest(context);
                return;
            }

            double rightTopLongitude, rightTopLatitude;
            if(!string.IsNullOrWhiteSpace(request["rightTop"]))
            {
                var rightTopArray = request["rightTop"].Split(',');
                if(rightTopArray.Length != 2 
                    || !double.TryParse(rightTopArray[0], out rightTopLatitude)
                    || !double.TryParse(rightTopArray[1], out rightTopLongitude))
                {
                    BadRequest(context);
                    return;
                }
            }else
            {
                BadRequest(context);
                return;
            }
            #endregion

            var leftBottom = new Location { Longitude = leftBottomLongitude, Latitude = leftBottomLatitude };
            var rightTop = new Location { Longitude = rightTopLongitude, Latitude = rightTopLatitude };
            if (dpi > 96)
            {
                double zoom = TileSystem.MapScale(leftBottomLatitude, zoomLevel, dpi);
                zoomLevel = (int)Math.Ceiling((double)zoomLevel * (1d + 1d / zoom));
                zoomLevel = zoomLevel > 20 ? 20 : zoomLevel;
            }
            int leftDownX, leftDownY, leftDownTileX, leftDownTileY;
            int rightUpX, rightUpY, rightUpTileX, rightUpTileY;

            TileSystem.LatLongToPixelXY(leftBottom.Latitude, leftBottom.Longitude, zoomLevel, out leftDownX, out leftDownY);
            TileSystem.PixelXYToTileXY(leftDownX, leftDownY, out leftDownTileX, out leftDownTileY);

            TileSystem.LatLongToPixelXY(rightTop.Latitude, rightTop.Longitude, zoomLevel, out rightUpX, out rightUpY);
            TileSystem.PixelXYToTileXY(rightUpX, rightUpY, out rightUpTileX, out rightUpTileY);

            int minX = leftDownX, minY = rightUpY;
            int width = Math.Abs(leftDownX - rightUpX);
            int height = Math.Abs(leftDownY - rightUpY);
            int pixelX, pixelY;
            using (Bitmap mapImage = new Bitmap(width, height))
            using(Graphics g = Graphics.FromImage(mapImage))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                for (var y = rightUpTileY; y <= leftDownTileY; y++)
                for (var x = leftDownTileX; x <= rightUpTileX; x++)
                {
                    var key = TileSystem.TileXYToQuadKey(x, y, zoomLevel);
                    byte[] tileContent = Cache(key, mode, x + y);
                    TileSystem.TileXYToPixelXY(x, y, out pixelX, out pixelY);
                    using (var tileStream = new MemoryStream(tileContent))
                    using (var img = Bitmap.FromStream(tileStream))
                    {
                        float px = (float)(pixelX - minX);
                        float py = (float)(pixelY - minY);
                        g.DrawImage(img, px, py);
                    }
                }

                context.Response.Clear();
                context.Response.ContentType = "image/png";
                context.Response.Flush();
                using (MemoryStream imgStream = new MemoryStream())
                {
                    mapImage.Save(imgStream, ImageFormat.Png);
                    int readed = 0;
                    byte[] buffer = new byte[1024];
                    imgStream.Seek(0, SeekOrigin.Begin);
                    do
                    {
                        readed = imgStream.Read(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Write(buffer, 0, readed);
                        context.Response.Flush();
                    } while (readed > 0);
                }
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}