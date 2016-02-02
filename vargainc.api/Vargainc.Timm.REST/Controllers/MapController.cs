using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vargainc.Timm.EF;
using Vargainc.Timm.Models;
using Wintellect.PowerCollections;
using Vargainc.Timm.REST.Helper;
using System.Threading.Tasks;

namespace Vargainc.Timm.REST.Controllers
{
    /// <summary>
    /// Lat = Y Long = X
    /// </summary>
    [RoutePrefix("map")]
    public class MapController : ApiController
    {
        private TimmContext db = new TimmContext();

        [HttpPost]
        [Route("campaign")]
        public IHttpActionResult GetCampaignSummaryMap()
        {

            // call PhantomJS map image service
            // {"tiles":"ee1c0a62-7989-48bd-97e7-498d1a0984bb.jpg","geometry":"9fc02fd0-ce6c-42ff-8530-9d33f2a14342.png","success":true,"campaignId":"45"}
            return Json(new
            {

            });
        }

        [HttpGet]
        [Route("submap")]
        public IHttpActionResult GetSubmap()
        {

//googleKey:AIzaSyAKdtxTHEA7_knbSLYAw8iQDrKQ70_K7uI
//targetMethod:
//gTUDotsRadii:5
//PenetrationColors[]:20
//PenetrationColors[]:40
//PenetrationColors[]:60
//PenetrationColors[]:80
//baseUrl:http://52.8.230.211/201601/api/
//mapUrl:http://54.67.112.52/map/
//imgUrl:http://ec2-54-67-112-52.us-west-1.compute.amazonaws.com/mapimg/
//campaignId:45
//suppressCover:false
//suppressCampaign:false
//suppressCampaignSummary:false
//suppressNDAInCampaign:false
//suppressSubMap:false
//suppressSubMapCountDetail:false
//suppressNDAInSubMap:false
//suppressDMap:true
//showPenetrationColors:true
//suppressLocations:false
//suppressRadii:false
//suppressGTU:false
//suppressNDAInDMap:false
//btnCheckAllDMap:false
//type:Campaign
//mapType:ROADMAP



            // call PhantomJS map image service
            // {"tiles":"ee1c0a62-7989-48bd-97e7-498d1a0984bb.jpg","geometry":"9fc02fd0-ce6c-42ff-8530-9d33f2a14342.png","success":true,"campaignId":"45"}
            return Json(new
            {

            });
        }

        [HttpPost]
        [Route("dmap")]
        public IHttpActionResult GetDmap()
        {
//googleKey:AIzaSyAKdtxTHEA7_knbSLYAw8iQDrKQ70_K7uI
//targetMethod:
//gTUDotsRadii:5
//PenetrationColors[]:20
//PenetrationColors[]:40
//PenetrationColors[]:60
//PenetrationColors[]:80
//baseUrl:http://52.8.230.211/201601/api/
//mapUrl:http://54.67.112.52/map/
//imgUrl:http://ec2-54-67-112-52.us-west-1.compute.amazonaws.com/mapimg/
//campaignId:45
//suppressCover:false
//suppressCampaign:false
//suppressCampaignSummary:false
//suppressNDAInCampaign:false
//suppressSubMap:false
//suppressSubMapCountDetail:false
//suppressNDAInSubMap:false
//suppressDMap:true
//showPenetrationColors:true
//suppressLocations:false
//suppressRadii:false
//suppressGTU:false
//suppressNDAInDMap:false
//btnCheckAllDMap:false
//submapId:434
//type:SubMap
//mapType:HYBRID

            // call PhantomJS map image service
            // {"tiles":"624977ef-c6af-4f2b-b2f0-4c40f7c170a4.jpg","geometry":"1d96a1ad-5869-4f7e-b0d2-ea220b39e3f3.png","success":true,"campaignId":"45","submapId":"434"}
            return Json(new
            {

            });
        }

        [HttpPost]
        [Route("report")]
        public IHttpActionResult GetGtuReport()
        {

            // call PhantomJS map image service
            // {"tiles":"624977ef-c6af-4f2b-b2f0-4c40f7c170a4.jpg","geometry":"1d96a1ad-5869-4f7e-b0d2-ea220b39e3f3.png","success":true,"campaignId":"45","submapId":"434"}
            return Json(new
            {

            });
        }
    }
}
