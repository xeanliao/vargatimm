using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Vargainc.Timm.EF;
using Vargainc.Timm.Models;
using System.Data.Entity;
using System.Web;
using Vargainc.Timm.REST.ViewModel;
using System.Net.Http;
using System.Configuration;
using System.IO;
using System.Net;

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

        /// <summary>
        /// return role in auditor walker and driver
        /// </summary>
        /// <returns></returns>
        [Route("gtu")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserForGTU()
        {
            HashSet<UserRoles> allowedUser = new HashSet<UserRoles>();
            allowedUser.Add(UserRoles.Auditor);
            allowedUser.Add(UserRoles.Driver);
            allowedUser.Add(UserRoles.Walker);
            var result = await db.Users.Where(i=>i.CompanyId != null && i.Role != null).Select(i => new
            {
                i.CompanyId,
                CompanyName = i.Company.Name,
                UserId = i.Id,
                UserName = i.FullName,
                i.Role
            }).OrderBy(i => i.CompanyId).ThenBy(i => i.UserId).ToListAsync();
            return Json(result.Where((i) =>
            {
                return i.Role.HasValue && allowedUser.Contains(i.Role.Value);
            }).ToList());
        }

        /// <summary>
        /// This will recieved client post picture. not like normal ajax request
        /// the post data is multipart/form-data. that we need parse the post data by ourself
        /// see more here http://www.asp.net/web-api/overview/advanced/sending-html-form-data-part-2
        /// </summary>
        /// <returns>new create employee</returns>
        [Route("employee")]
        [HttpPost]
        public async Task<IHttpActionResult> AddEmployee()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string root = ConfigurationManager.AppSettings["TempPath"];
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            var provider = new MultipartFormDataStreamProvider(root);
            
            await Request.Content.ReadAsMultipartAsync(provider);

            var postCompanyId = provider.FormData["CompanyId"];
            var postRole = provider.FormData["Role"];
            var postFullName = provider.FormData["FullName"];
            var postCellPhone = provider.FormData["CellPhone"];
            var postDateOfBirth = provider.FormData["DateOfBirth"];
            var postNotes = provider.FormData["Notes"];

            var dbUser = db.Users.Create();
            int companyId;
            if(!int.TryParse(postCompanyId, out companyId))
            {
                return NotFound();
            }
            else
            {
                var dbCompany = await db.Companies.FindAsync(companyId);
                if(dbCompany == null)
                {
                    return NotFound();
                }
                dbUser.Company = dbCompany;
            }

            UserRoles role;
            if(!Enum.TryParse<UserRoles>(postRole, out role))
            {
                return NotFound();
            }
            else if(role != UserRoles.Auditor && role != UserRoles.Driver && role != UserRoles.Walker)
            {
                return NotFound();
            }
            else
            {
                dbUser.Role = role;
            }
            if (string.IsNullOrWhiteSpace(postFullName))
            {
                return NotFound();
            }
            else
            {
                dbUser.FullName = postFullName;
            }


            dbUser.CellPhone = postCellPhone;

            DateTime birthday;
            if(DateTime.TryParse(postDateOfBirth, out birthday))
            {
                dbUser.DateOfBirth = birthday;
            }

            dbUser.Notes = postNotes;

            dbUser.UserCode = dbUser.FullName.Replace(" ", ".");
            dbUser.UserName = Guid.NewGuid().ToString();
            dbUser.Password = Guid.NewGuid().ToString();
            dbUser.Enabled = false;  // employee cannot access the software.

            if(provider.FileData.Count > 0)
            {
                var fileName = provider.FileData[0].Headers.ContentDisposition.FileName;
                fileName = fileName.Trim('"');
                var fileExtension = Path.GetExtension(fileName);
                var fileGuidName = string.Format("{0}{1}", Guid.NewGuid().ToString(), fileExtension);
                var fileServerPath = ConfigurationManager.AppSettings["PictureRoot"];
                if (!Directory.Exists(fileServerPath))
                {
                    Directory.CreateDirectory(fileServerPath);
                }
                var fileSavePath = Path.Combine(fileServerPath, fileGuidName);
                File.Move(provider.FileData[0].LocalFileName, fileSavePath);
                dbUser.Picture = fileGuidName;
            }

            

            db.Users.Add(dbUser);
            await db.SaveChangesAsync();
            return Json(new {
                CompanyId = dbUser.CompanyId,
                CompanyName = dbUser.Company.Name,
                UserId = dbUser.Id,
                UserName = dbUser.FullName,
                dbUser.Role
            });
        }

        [Route("company")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCompany()
        {
            var result = await db.Companies.Select(i => new { i.Id, i.Name }).ToListAsync();
            return Json(result);
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

            return Json(result);
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

        [Route("login")]
        [HttpGet]
        public async Task<IHttpActionResult> Login(string username, string password)
        {
            var existUser = await db.Users.Where(i => i.UserName == username && i.Password == password && i.Enabled == true).FirstOrDefaultAsync();
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
                if (existUser.LastLoginTime.HasValue && existUser.LastLoginTime.Value > DateTime.Now.AddDays(-12) && !string.IsNullOrWhiteSpace(existUser.Token))
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
