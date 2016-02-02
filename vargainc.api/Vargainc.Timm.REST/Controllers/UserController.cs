using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Collections.Concurrent;
using Vargainc.Timm.EF;
using Vargainc.Timm.Models;
using System.Data.Entity;
using System.Text;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;
using NetTopologySuite.IO.KML;
using Vargainc.Timm.REST.ViewModel.ControlCenter;
using Vargainc.Timm.REST.Helper;
using System.Web;
using Vargainc.Timm.REST.ViewModel;
using System.Net.Http;
using System.Security.Claims;
using System.Net.Http.Headers;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("user")]
    public class UserController : ApiController
    {
        private const string AuthCookieName = "ssid";
        private TimmContext db = new TimmContext();

        [Route("info")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentLoginUser()
        {
            string token = null;
            var requestCookies = Request.Headers.GetCookies(AuthCookieName).FirstOrDefault();
            if (requestCookies != null && requestCookies.Cookies != null && requestCookies.Cookies.Count > 0)
            {
                var authCookie = requestCookies.Cookies.Where(i => i.Name == AuthCookieName).FirstOrDefault();
                if(authCookie != null)
                {
                    token = authCookie.Value;
                }
            }
            if (!string.IsNullOrWhiteSpace(token))
            {
                var timeout = DateTime.Now.AddHours(-12);
                var existUser = await db.Users.Where(i => i.Token == token && i.LastLoginTime > timeout && i.Enabled == true).FirstOrDefaultAsync();
                if (existUser != null)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            Token = token,
                            FullName = existUser.FullName,
                            Email = existUser.Email
                        }
                    });
                }
            }
            return Json(new { success = false });
        }

        
        [Route("group/campaign")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserForCamapign()
        {
            var queryGroups = new List<int?>() { 46, 47, 53 };
            return await queryUserInGroup(queryGroups);
        }


        [Route("group/distribution")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserForDistribution()
        {
            var queryGroups = new List<int?>() { 51, 52, 53 };
            return await queryUserInGroup(queryGroups);
        }

        [Route("group/monitor")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserForMonitor()
        {
            var queryGroups = new List<int?>() { 48, 49, 50, 51, 52, 53, 46 };
            return await queryUserInGroup(queryGroups);
        }

        private async Task<IHttpActionResult> queryUserInGroup(List<int?> queryGroups)
        {
            var result = await db.Groups.Where(i => queryGroups.Contains(i.Id))
               .SelectMany(i => i.Users)
               .Where(i => i.Enabled == true)
               .Select(i => new
               {
                   i.Id,
                   i.UserCode,
                   i.UserName,
                   i.FullName,
                   i.Email
               }).ToListAsync();

            return Json(new { success = true, data = result });
        }

        [Route("login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login([FromBody]LoginUser user)
        {
            var existUser = await db.Users.Where(i => i.UserName == user.Username && i.Password == user.Password && i.Enabled == true).FirstOrDefaultAsync();
            if (existUser != null)
            {
                //var claims = new List<System.Security.Claims.Claim>();
                //claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, existUser.FullName));
                //var id = new System.Security.Claims.ClaimsIdentity(claims, "");
                //ClaimsIdentity identity = new System.Security.Claims.ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);

                //var ctx = Request.GetOwinContext();
                //var authenticationManager = ctx.Authentication;
                //authenticationManager.SignIn(id);

                //AuthenticationManager

                string token = null;
                if(existUser.LastLoginTime.HasValue && existUser.LastLoginTime.Value > DateTime.Now.AddDays(-12) && !string.IsNullOrWhiteSpace(existUser.Token))
                {
                    token = existUser.Token;
                }
                else
                {
                    token = Guid.NewGuid().ToString();
                }
                existUser.Token = token;
                existUser.LastLoginTime = DateTime.Now;
                await db.SaveChangesAsync();

                var authCookie = new HttpCookie("ssid");
                authCookie.HttpOnly = true;
                authCookie.Shareable = false;
                authCookie.Value = token;

                HttpContext.Current.Response.SetCookie(authCookie);
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        token = token,
                        displayName = existUser.FullName,
                        email = existUser.Email
                    }
                });
            }
            return Json(new { success = false });
        }

        [Route("logout")]
        [HttpPost]
        public IHttpActionResult Logout()
        {
            var authCookie = HttpContext.Current.Request.Cookies["ssid"];
            if(authCookie != null && !string.IsNullOrWhiteSpace(authCookie.Value))
            {
                authCookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Add(authCookie);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
