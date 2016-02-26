using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using System.Configuration;

[assembly: OwinStartup(typeof(Vargainc.Timm.REST.Startup))]

namespace Vargainc.Timm.REST
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            HttpConfiguration config = new HttpConfiguration();
            // Web API configuration and services
            // newton.json formatter optimization
            var json = config.Formatters.JsonFormatter.SerializerSettings;
            // MM-dd-yyyy date formate
            json.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            json.ContractResolver = new CamelCasePropertyNamesContractResolver();
            json.DateFormatString = "MM-dd-yyyy";
            // enum to string
            json.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            json.NullValueHandling = NullValueHandling.Ignore;
            json.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
           

            // Web API Cros for Dev or local test
            var allowWebsite = ConfigurationManager.AppSettings["CorsSite"];
            if (!string.IsNullOrWhiteSpace(allowWebsite))
            {
                var cors = new EnableCorsAttribute(allowWebsite, "*", "*");
                config.EnableCors(cors);
            }
            
            // Web API routes
            config.MapHttpAttributeRoutes();
            
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.EnsureInitialized();

            app.UseWebApi(config);

            

        }
    }
}
