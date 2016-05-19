﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Vargainc.Timm.Extentions;

namespace Vargainc.Timm.REST.Helper
{
    public class PublicAccessFilter : ActionFilterAttribute
    {
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var arguments = actionContext.ActionArguments;
            if (arguments.ContainsKey("taskId"))
            {
                var value = actionContext.RequestContext.RouteData.Values["taskId"] as string;
                int fixedValue;
                if(!int.TryParse(value, out fixedValue))
                {
                    value = value.DesDecrypt();
                    int.TryParse(value, out fixedValue);
                }
                actionContext.RequestContext.RouteData.Values["taskId"] = fixedValue;
                actionContext.ActionArguments["taskId"] = fixedValue;
            }
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}