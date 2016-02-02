using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vargainc.Timm.EF;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.REST.Helper
{
    public static class UserManager
    {
        public static bool TryGetCurrentUser(out User loginUser)
        {
            loginUser = null;
            //var sessionUserId = HttpContext.Current.Session["CrossDomainUserId"];
            var authCookie = HttpContext.Current.Request.Cookies["ssid"];
            if(authCookie != null && !string.IsNullOrWhiteSpace(authCookie.Value))
            {
                using(var db = new TimmContext())
                {
                    var timeout = DateTime.Now.AddHours(-12);
                    loginUser = db.Users.FirstOrDefault(i => i.Token == authCookie.Value && i.LastLoginTime > timeout);
                    if(loginUser != null && loginUser.Enabled == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}