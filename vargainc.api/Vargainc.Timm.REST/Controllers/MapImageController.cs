using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vargainc.Timm.REST.ViewModel.MapImage;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("map")]
    public class MapImageController : ApiController
    {
        private static readonly TimeSpan TIMEOUT = new TimeSpan(0, 10, 0);

        [HttpPost]
        [Route("campaign")]
        public async Task<IHttpActionResult> GetCampaignMapImage([FromBody] MapImageOptions options)
        {

            var url = ConfigurationManager.AppSettings["MapImageServerAddress"];
            options.baseUrl = ConfigurationManager.AppSettings["APIServiceAddress"];
            options.type = "Campaign";
            var postString = JsonConvert.SerializeObject(options);
            var postContent = new ByteArrayContent(Encoding.UTF8.GetBytes(postString));
            var client = new HttpClient();
            client.Timeout = TIMEOUT;
            var response = await client.PostAsync(url + "map/", postContent);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var serverResult = JsonConvert.DeserializeObject<MapServerResult>(responseString);
                serverResult.tiles = url + "mapimg/" + serverResult.tiles;
                serverResult.geometry = url + "mapimg/" + serverResult.geometry;
                return Json(serverResult);
            }
            return Json(new MapServerResult
            {
                success = false
            });
        }

        [HttpPost]
        [Route("submap")]
        public async Task<IHttpActionResult> GetSubMapImage([FromBody] MapImageOptions options)
        {
            var url = ConfigurationManager.AppSettings["MapImageServerAddress"];
            options.baseUrl = ConfigurationManager.AppSettings["APIServiceAddress"];
            options.type = "SubMap";
            var postString = JsonConvert.SerializeObject(options);
            var postContent = new ByteArrayContent(Encoding.UTF8.GetBytes(postString));
            var client = new HttpClient();
            client.Timeout = TIMEOUT;

            var response = await client.PostAsync(url + "map/", postContent);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var serverResult = JsonConvert.DeserializeObject<MapServerResult>(responseString);
                serverResult.tiles = url + "mapimg/" + serverResult.tiles;
                serverResult.geometry = url + "mapimg/" + serverResult.geometry;
                return Json(serverResult);
            }

            return Json(new MapServerResult
            {
                success = false
            });
        }

        [HttpPost]
        [Route("dmap")]
        public async Task<IHttpActionResult> GetDMapImage([FromBody] MapImageOptions options)
        {
            var url = ConfigurationManager.AppSettings["MapImageServerAddress"];
            options.baseUrl = ConfigurationManager.AppSettings["APIServiceAddress"];
            options.type = "DMap";
            var postString = JsonConvert.SerializeObject(options);
            var postContent = new ByteArrayContent(Encoding.UTF8.GetBytes(postString));
            var client = new HttpClient();
            client.Timeout = TIMEOUT;
            var response = await client.PostAsync(url + "map/", postContent);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var serverResult = JsonConvert.DeserializeObject<MapServerResult>(responseString);
                serverResult.tiles = url + "mapimg/" + serverResult.tiles;
                serverResult.geometry = url + "mapimg/" + serverResult.geometry;
                return Json(serverResult);
            }

            return Json(new MapServerResult
            {
                success = false
            });
        }

        [HttpPost]
        [Route("distribution")]
        public async Task<IHttpActionResult> GetDistributionMapImage([FromBody] MapImageOptions options)
        {
            var url = ConfigurationManager.AppSettings["MapImageServerAddress"];
            options.baseUrl = ConfigurationManager.AppSettings["APIServiceAddress"];
            options.mapType = MapTypeEnum.ROADMAP;
            options.type = "Distribute";
            var postString = JsonConvert.SerializeObject(options);
            var postContent = new ByteArrayContent(Encoding.UTF8.GetBytes(postString));
            var client = new HttpClient();
            client.Timeout = TIMEOUT;
            var response = await client.PostAsync(url + "map/", postContent);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var serverResult = JsonConvert.DeserializeObject<MapServerResult>(responseString);
                serverResult.tiles = url + "mapimg/" + serverResult.tiles;
                serverResult.geometry = url + "mapimg/" + serverResult.geometry;
                return Json(serverResult);
            }

            return Json(new MapServerResult
            {
                success = false
            });
        }
    }
}
