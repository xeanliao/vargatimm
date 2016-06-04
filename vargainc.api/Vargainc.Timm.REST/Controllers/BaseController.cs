using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Vargainc.Timm.REST.Controllers
{
    public class BaseController : ApiController
    {
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
    }
}