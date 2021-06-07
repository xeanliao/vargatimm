using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.RESTService;
using Microsoft.ApplicationServer.Http;
using TIMM.GPS.RESTService.Extend;
using System.Net.Http.Formatting;
using System.Web.Routing;

[assembly: WebActivator.PreApplicationStartMethod(typeof(InitializeService), "RegistService")]
namespace TIMM.GPS.RESTService
{
    public static class InitializeService
    {
        public static void RegistService()
        {
            var config = new HttpConfiguration() 
            { 
                EnableTestClient = true,
                IncludeExceptionDetail = true,
                EnableHelpPage = true,
            };
            config.Formatters.Clear();
            config.Formatters.Add(new ContactFastJsonFormatter());
            //config.Formatters.Add(new ContactJsonZipFormatter());
            config.Formatters.Add(new XmlMediaTypeFormatter());
            config.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());

            RouteTable.Routes.MapServiceRoute<CampaignService>("service/campaign", config);
            RouteTable.Routes.MapServiceRoute<NdAddressService>("service/ndaddress", config);
            RouteTable.Routes.MapServiceRoute<UserService>("service/user", config);
            RouteTable.Routes.MapServiceRoute<TaskService>("service/task", config);
        }
    }
}
