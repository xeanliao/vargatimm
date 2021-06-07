using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Vargainc.Timm.REST.Controllers
{
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
