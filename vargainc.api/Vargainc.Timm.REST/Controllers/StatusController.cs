using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Vargainc.Timm.REST.Controllers
{
    public class ResultModel
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class StatusController : ApiController
    {
        [HttpGet]
        [Route("status")]
        public IHttpActionResult Status()
        {
            return Ok();
        }
    }
}
