using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using System.Text;
using System.Web;
using Vargainc.Timm.EF;
using Vargainc.Timm.Models;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.KML;
using Vargainc.Timm.REST.ViewModel.ControlCenter;
using Vargainc.Timm.REST.Helper;
using Vargainc.Timm.REST.ViewModel;
using System.IO;
using Vargainc.Timm.Extentions;
using Z.EntityFramework.Plus;
using System.Net.Http;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("campaign")]
    public class CampaignController : BaseController
    {
        #region static memeber
        public static string FormantCampaignName(Campaign campaign)
        {
            var parts = new string[]{
                campaign.Date.Value.ToString("MMddyy"),
                campaign.ClientCode,
                campaign.UserName,
                campaign.AreaDescription,
                campaign.Sequence.ToString()
            };
            return String.Join("-", parts);
        }
        #endregion

        #region Calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
        private const string APT = "APT ONLY";
        private const string HOME = "HOME ONLY";
        private const string APT_HOME = "APT + HOME";
        #endregion

        private readonly string TempPath = ConfigurationManager.AppSettings["TempPath"];

        private void FixLogoPath(List<Campaign> result)
        {
            string basePath = ConfigurationManager.AppSettings["campaignimagepath"];
            basePath = basePath.Substring(1).TrimEnd('/');
            foreach (var item in result)
            {
                if (!string.IsNullOrWhiteSpace(item.Logo))
                {
                    item.LogoPath = string.Format("{0}/{1}",
                        basePath,
                        item.Logo);
                }
            }

        }

        private void FixLogoPath(Campaign result)
        {
            FixLogoPath(new List<Campaign> { result });
        }

        /// <summary>
        /// Get Campaign List in Control Center
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCampaignList()
        {
            var result = await db.Campaigns
                .Where(i => i.Status.Max(s => s.Status) == 0)
                .OrderByDescending(i => i.Id)
                .Select(i => new
                {
                    i.AreaDescription,
                    i.ClientCode,
                    i.ClientName,
                    i.ContactName,
                    i.CreatorName,
                    i.CustemerName,
                    i.Description,
                    i.Date,
                    i.Id,
                    i.Name,
                    i.Sequence,
                    i.UserName
                }).ToListAsync();
            return Json(result);
        }

        /// <summary>
        /// Get Distribution Maps List in Control Center
        /// </summary>
        /// <returns></returns>
        [Route("distribution")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHaveDistributionMapsCampaignList()
        {
            var result = await db.Campaigns
                .Where(i => i.Status.Max(s => s.Status) == 1)
                .OrderByDescending(i => i.Id)
                .Select(i => new
                {
                    i.AreaDescription,
                    i.ClientCode,
                    i.ClientName,
                    i.ContactName,
                    i.CreatorName,
                    i.CustemerName,
                    i.Description,
                    i.Date,
                    i.Id,
                    i.Name,
                    i.Sequence,
                    i.UserName
                }).ToListAsync();

            return Json(result);
        }

        [Route("monitor")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCampaignListWithTask()
        {
            var result = await db.Campaigns
                 .Where(i => i.Status.Max(s => s.Status) == 2)
                 .OrderByDescending(i => i.Id)
                 .Select(i => new
                 {
                     i.AreaDescription,
                     i.ClientCode,
                     i.ClientName,
                     i.ContactName,
                     i.CreatorName,
                     i.CustemerName,
                     i.Description,
                     i.Date,
                     i.Id,
                     i.Name,
                     i.Sequence,
                     i.UserName,
                     Tasks = i.SubMaps.SelectMany(s => s.DistributionMaps.SelectMany(d => d.Tasks).Where(t => t.Status == 0)).Select(t => new
                     {
                         t.Id,
                         t.Name,
                         t.Date,
                         t.Status,
                     })
                 }).ToListAsync();
            return Json(result.Where(i => i.Tasks.Count() > 0).ToList());
        }

        [Route("{campaignId:int}/tasks/all")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCampaignWithAllTask(int? campaignId)
        {
            var result = await db.Campaigns
                 .Where(i => i.Status.Max(s => s.Status) == 2)
                 .Where(i => i.Id == campaignId)
                 .OrderByDescending(i => i.Id)
                 .Select(i => new
                 {
                     i.AreaDescription,
                     i.ClientCode,
                     i.ClientName,
                     i.ContactName,
                     i.CreatorName,
                     i.CustemerName,
                     i.Description,
                     i.Date,
                     i.Id,
                     i.Name,
                     i.Sequence,
                     i.UserName,
                     Tasks = i.SubMaps.SelectMany(s => s.DistributionMaps.SelectMany(d => d.Tasks)).Select(t => new
                     {
                         CampaignId = i.Id,
                         SubMapId = t.DistributionMap.SubMapId,
                         DMapId = t.DistributionMapId,
                         t.Id,
                         t.Name,
                         t.Date,
                         IsFinished = t.Status == 1,
                         Status = db.TaskTimes.Where(m=>m.TaskId == t.Id).OrderByDescending(m => m.Id).FirstOrDefault().TimeType
                     })
                 }).FirstOrDefaultAsync();
            return Json(result);
        }

        [Route("report")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCampaignListWithReport()
        {
            var result = await db.Campaigns
                .Where(i => i.Status.Max(s => s.Status) == 2)
                .OrderByDescending(i => i.Id)
                .Select(i => new
                {
                    i.AreaDescription,
                    i.ClientCode,
                    i.ClientName,
                    i.ContactName,
                    i.CreatorName,
                    i.CustemerName,
                    i.Description,
                    i.Date,
                    i.Id,
                    i.Name,
                    i.Sequence,
                    i.UserName,
                    Tasks = i.SubMaps.SelectMany(s => s.DistributionMaps.SelectMany(d => d.Tasks).Where(t => t.Status == 1)).Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Date,
                        t.Status
                    })
                }).ToListAsync();
            return Json(result.Where(i => i.Tasks.Count() > 0).ToList());
        }

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateCampaign([FromBody] CampaignViewModel campaign)
        {
            User loginedUser;
            if (!UserManager.TryGetCurrentUser(out loginedUser))
            {
                return Json(new { success = false, error = "NOT_LOGIN" });
            }
            var user = loginedUser;

            var dbCampaign = db.Campaigns.Create();


            //use current user name;
            var currentUser = user.UserName;
            dbCampaign.CreatorName = currentUser;
            dbCampaign.UserName = currentUser;

            //default value
            dbCampaign.Name = user.UserName;
            dbCampaign.Description = "";
            dbCampaign.CustemerName = "";
            //for new campaign. set the default center of USA and zoom level to 5.
            dbCampaign.Latitude = 39.82541310342478;
            dbCampaign.Longitude = -102.74414062500001;
            dbCampaign.ZoomLevel = 5;


            dbCampaign.AreaDescription = campaign.AreaDescription;
            dbCampaign.ClientCode = campaign.ClientCode;
            dbCampaign.ClientName = campaign.ClientName;
            dbCampaign.ContactName = campaign.ContactName;

            dbCampaign.Date = campaign.Date ?? DateTime.Now.Date;
            var minDate = dbCampaign.Date.Value.Date;
            var maxDate = minDate.AddDays(1);
            var querySequence = from p in db.Campaigns
                                where p.Date >= minDate
                                && p.Date < maxDate
                                && p.CreatorName == currentUser
                                && p.ClientCode == campaign.ClientCode
                                && p.AreaDescription == campaign.AreaDescription
                                select p.Sequence;
            dbCampaign.Sequence = (querySequence.Max() ?? 0) + 1;

            db.Campaigns.Add(dbCampaign);
            await db.SaveChangesAsync();


            var dbStatus = db.Status.Create();
            dbStatus.CampaignId = dbCampaign.Id.Value;
            dbStatus.Status = 0;
            dbStatus.UserId = user.Id;

            db.Status.Add(dbStatus);

            await db.SaveChangesAsync();
            campaign.Id = dbCampaign.Id;
            return Json(new { success = true, data = campaign });
        }

        [Route("{campaignId:int}")]
        [HttpGet]
        public async Task<List<SubMap>> GetCampaign(int campaignId)
        {
            var campaign = await db.Companies.FindAsync(campaignId).ConfigureAwait(false);
            var subMaps = await db.SubMaps.Where(i=>i.CampaignId == campaignId).OrderBy(i=>i.OrderId).ToListAsync().ConfigureAwait(false);
            return subMaps;
        }

        [Route("{campaignId:int}/submap")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCampaignWithSubMap(int campaignId)
        {
            var campaign = await db.Campaigns
                .Include(i=>i.SubMaps)
                .Include(i=>i.Addresses)
                .Include(i => i.SubMaps.Select(y => y.SubMapRecords))
                .Where(i=>i.Id == campaignId)
                .Select(i => new
                {
                    i.Id,
                    i.ClientCode,
                    i.ClientName,
                    i.ContactName,
                    i.CreatorName,
                    i.Longitude,
                    i.Latitude,
                    i.AreaDescription,
                    i.ZoomLevel,
                    Addresses = i.Addresses.Select(a=>new 
                    { 
                        a.AddressName, 
                        a.Color, 
                        a.Radiuses, 
                        a.ZipCode, 
                        a.Id, 
                        a.Latitude, 
                        a.Longitude
                    }).OrderBy(a=>a.AddressName).ToList(),
                    SubMaps = i.SubMaps.Select(s=>new
                    { 
                        s.Id, 
                        s.ColorString, 
                        s.CountAdjustment, 
                        s.Name, 
                        s.OrderId, 
                        s.Total,
                        s.Penetration, 
                        s.Percentage, 
                        s.TotalAdjustment,
                        s.CampaignId,
                        SubMapRecords = s.SubMapRecords.Select(r=>new { 
                            r.Classification,
                            r.AreaId
                        }).ToList()
                    }).OrderBy(s=>s.OrderId).ToList()
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            return Json(new { success = true, data = campaign });
        }

        [Route("{campaignId:int}/dmap")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCampaignWithDMap(int campaignId)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId).ConfigureAwait(false);
            var submaps = await db.SubMaps.Include(i=>i.SubMapRecords).Where(i=>i.CampaignId == campaignId).ToListAsync().ConfigureAwait(false);
            var submapIds = submaps.Select(i => i.Id).ToArray();
            var dmaps = await db.DistributionMaps.Include(i=>i.DistributionMapRecords).Where(i=>submapIds.Contains(i.SubMapId)).OrderBy(i=>i.SubMapId).ToListAsync().ConfigureAwait(false);

            var submapResults = submaps.Select(s => new
            {
                s.Id,
                s.ColorString,
                s.CountAdjustment,
                s.Name,
                s.OrderId,
                s.Total,
                s.Penetration,
                s.Percentage,
                s.TotalAdjustment,
                s.CampaignId,
                SubMapRecords = s.SubMapRecords.Select(r => new {
                    r.Classification,
                    r.AreaId
                }).ToList(),
                DMaps = dmaps.Where(d=>d.SubMapId == s.Id).Select(d=> new { 
                    d.Id,
                    s.CampaignId,
                    d.SubMapId,
                    d.Name,
                    d.ColorString,
                    d.Total,
                    d.Penetration,
                    d.Percentage,
                    d.TotalAdjustment,
                    d.CountAdjustment,
                    DMapRecords = d.DistributionMapRecords.Select(r => new { Classification = s.SubMapRecords.First().Classification ,r.AreaId })
                })
            }).OrderBy(s=>s.OrderId).ToList();

            var result = new
            {
                campaign.Id,
                campaign.ClientCode,
                campaign.ClientName,
                campaign.ContactName,
                campaign.CreatorName,
                campaign.Longitude,
                campaign.Latitude,
                campaign.ZoomLevel,
                campaign.AreaDescription,
                SubMaps = submapResults
            };
            return Json(new { success = true, data = result });
        }

        [Route("{campaignId:int}")]
        [HttpPut]
        public async Task<IHttpActionResult> EditCampaign(int campaignId, [FromBody] CampaignViewModel campaign)
        {
            if (!campaign.Id.HasValue)
            {
                return Json(new { success = false });
            }
            var dbCampaign = await db.Campaigns.FindAsync(campaign.Id.Value);
            if (dbCampaign == null)
            {
                return Json(new { success = false });
            }

            User loginedUser;
            if (!UserManager.TryGetCurrentUser(out loginedUser))
            {
                return Json(new { success = false, error = "NOT_LOGIN" });
            }

            var user = loginedUser;

            //use current user name;
            var currentUser = user.UserName;
            dbCampaign.CreatorName = currentUser;
            dbCampaign.UserName = currentUser;

            //default value
            dbCampaign.Name = "admin";
            dbCampaign.Description = "";
            dbCampaign.CustemerName = "";
            dbCampaign.Latitude = 0;
            dbCampaign.Longitude = 0;
            dbCampaign.ZoomLevel = 1;


            dbCampaign.AreaDescription = campaign.AreaDescription;
            dbCampaign.ClientCode = campaign.ClientCode;
            dbCampaign.ClientName = campaign.ClientName;
            dbCampaign.ContactName = campaign.ContactName;

            dbCampaign.Date = campaign.Date ?? dbCampaign.Date;
            dbCampaign.Sequence = campaign.Sequence ?? dbCampaign.Sequence;

            await db.SaveChangesAsync();

            return Json(new { success = true, data = campaign });
        }

        [Route("{campaignId:int}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCampaign(int? campaignId)
        {
            if (!campaignId.HasValue)
            {
                return Json(new { success = false });
            }
            var dbCampaign = db.Campaigns.Find(campaignId.Value);
            if (dbCampaign == null)
            {
                return Json(new { success = true });
            }

            User loginedUser;
            if (!UserManager.TryGetCurrentUser(out loginedUser))
            {
                return Json(new { success = false, error = "NOT_LOGIN" });
            }
            var user = loginedUser;

            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    //Create backup for Campaign
                    CampaignBackup backup = db.CampaignBackups.Create();
                    backup.AreaDescription = dbCampaign.AreaDescription;
                    backup.ClientCode = dbCampaign.ClientCode;
                    backup.ClientName = dbCampaign.ClientName;
                    backup.ContactName = dbCampaign.ContactName;
                    backup.CustemerName = dbCampaign.CustemerName;
                    backup.Date = dbCampaign.Date;
                    backup.Description = dbCampaign.Description;
                    backup.Id = dbCampaign.Id;
                    backup.Latitude = dbCampaign.Latitude;
                    backup.Longitude = dbCampaign.Longitude;
                    backup.Logo = dbCampaign.Logo;
                    backup.Name = dbCampaign.Name;
                    backup.Sequence = dbCampaign.Sequence;
                    backup.UserName = dbCampaign.UserName;
                    backup.ZoomLevel = dbCampaign.ZoomLevel;
                    backup.IPAddress = HttpContext.Current.Request.UserHostAddress;
                    backup.OperationTime = DateTime.Now;

                    backup.OperationUser = user.UserName;

                    db.CampaignBackups.Add(backup);
                    await db.SaveChangesAsync();

                    #region Fast Delete Campaign.
                    //current we used lazy load. you must load all the child entity if want entity framework auto cascade delete
                    db.DistributionJobs.RemoveRange(db.DistributionJobs.Where(i => i.CampaignId == campaignId));
                    await db.SaveChangesAsync();

                    var submaps = db.SubMaps.Where(i => i.CampaignId == campaignId).Select(i => i.Id).ToList();
                    if (submaps != null && submaps.Count > 0)
                    {
                        var fakeSubmaps = new List<SubMap>();
                        foreach (var submapId in submaps)
                        {
                            var dmaps = db.DistributionMaps.Where(i => i.SubMapId == submapId).Select(i => i.Id).ToList();
                            if (dmaps != null && dmaps.Count > 0)
                            {
                                var fakeDmaps = new List<DistributionMap>();
                                foreach (var dmapId in dmaps)
                                {
                                    db.DistributionMapRecords.RemoveRange(db.DistributionMapRecords.Where(i => i.DistributionMapId == dmapId));
                                    db.DistributionMapCoordinates.RemoveRange(db.DistributionMapCoordinates.Where(i => i.DistributionMapId == dmapId));

                                    var taskId = await db.Tasks.Where(i => i.DistributionMapId == dmapId).Select(i => i.Id).ToArrayAsync();

                                    db.TaskGtuInfoMappings.RemoveRange(db.TaskGtuInfoMappings.Where(i => taskId.Contains(i.TaskId)));
                                    db.TaskTimes.RemoveRange(db.TaskTimes.Where(i => taskId.Contains(i.TaskId)));

                                    db.Tasks.RemoveRange(db.Tasks.Where(i => i.DistributionMapId == dmapId));

                                    await db.SaveChangesAsync();
                                    fakeDmaps.Add(db.DistributionMaps.Attach(new DistributionMap { Id = dmapId }));
                                }
                                db.DistributionMaps.RemoveRange(fakeDmaps);
                                await db.SaveChangesAsync();
                            }
                            db.SubMapRecords.RemoveRange(db.SubMapRecords.Where(i => i.SubMapId == submapId));
                            db.SubMapCoordinates.RemoveRange(db.SubMapCoordinates.Where(i => i.SubMapId == submapId));
                            await db.SaveChangesAsync();
                            fakeSubmaps.Add(db.SubMaps.Attach(new SubMap { Id = submapId }));
                        }
                        db.SubMaps.RemoveRange(fakeSubmaps);
                        await db.SaveChangesAsync();
                    }

                    var address = db.Addresses.Where(i => i.CampaignId == campaignId).Select(i => i.Id).ToList();
                    if (address != null && address.Count > 0)
                    {
                        var fakeAddress = new List<Address>();
                        foreach (var addressId in address)
                        {
                            db.Radiuses.RemoveRange(db.Radiuses.Where(i => i.AddressId == addressId));
                            await db.SaveChangesAsync();
                            fakeAddress.Add(db.Addresses.Attach(new Address { Id = addressId }));
                        }
                        db.Addresses.RemoveRange(fakeAddress);
                        await db.SaveChangesAsync();
                    }

                    db.CampaignBlockGroupImporteds.RemoveRange(db.CampaignBlockGroupImporteds.Where(i => i.CampaignId == campaignId));
                    db.CampaignClassifications.RemoveRange(db.CampaignClassifications.Where(i => i.CampaignId == campaignId));
                    db.CampaignCRouteImporteds.RemoveRange(db.CampaignCRouteImporteds.Where(i => i.CampaignId == campaignId));
                    db.CampaignFiveZipImporteds.RemoveRange(db.CampaignFiveZipImporteds.Where(i => i.CampaignId == campaignId));
                    db.CampaignPercentageColors.RemoveRange(db.CampaignPercentageColors.Where(i => i.CampaignId == campaignId));
                    db.CampaignRecords.RemoveRange(db.CampaignRecords.Where(i => i.CampaignId == campaignId));
                    db.CampaignTractImporteds.RemoveRange(db.CampaignTractImporteds.Where(i => i.CampaignId == campaignId));
                    db.Status.RemoveRange(db.Status.Where(i => i.CampaignId == campaignId));
                    await db.SaveChangesAsync();
                    #endregion

                    db.Campaigns.Remove(dbCampaign);
                    await db.SaveChangesAsync();

                    trans.Commit();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json(new { success = false, debug = ex.ToString() });
                }

            }
        }

        [Route("{campaignId:int}/publish")]
        [HttpPut]
        public async Task<IHttpActionResult> PublishCampaignToDMap(int? campaignId, [FromBody] int? userId)
        {

            var dbCampaign = await db.Campaigns.FindAsync(campaignId);
            if (dbCampaign == null)
            {
                return NotFound();
            }
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var campaignStatus = dbCampaign.Status.Max(i => i.Status);
            if (campaignStatus != 0)
            {
                return Json(new { success = false, error = "Campaign is already published or updated. please refresh page and try again." });
            }
            if (dbCampaign.SubMaps.Count() == 0)
            {
                return Json(new { success = false, error = "You have not create submap yet!. please add some submaps first." });
            }
            db.Status.Add(new StatusInfo { Status = 1, Campaign = dbCampaign, User = user });
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }

        [Route("{campaignId:int}/monitor")]
        [HttpPut]
        public async Task<IHttpActionResult> PublishCampaignToMonitor(int? campaignId, [FromBody] int? userId)
        {
            var dbCampaign = await db.Campaigns.FindAsync(campaignId);
            if (dbCampaign == null)
            {
                return NotFound();
            }
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var campaignStatus = dbCampaign.Status.Max(i => i.Status);
            if (campaignStatus != 1)
            {
                return Json(new { success = false, error = "Campaign is already send to monitor or have not published. please refresh page and try again." });
            }
            if (dbCampaign.SubMaps.Count() == 0)
            {
                return Json(new { success = false, error = "You have not create submap yet!. please add some submaps first." });
            }
            var submapId = dbCampaign.SubMaps.Select(i => i.Id).ToList();
            var dmapCount = db.DistributionMaps.Where(i => submapId.Contains(i.SubMapId)).Count();
            if (dmapCount == 0)
            {
                return Json(new { success = false, error = "You have not create distribution map yet!. please add some distribution maps first." });
            }

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    //create task for all distribution map
                    var dmaps = db.DistributionMaps.Where(i => submapId.Contains(i.SubMapId)).Select(i => new { i.Id, i.Name });
                    List<Models.Task> taskList = new List<Models.Task>();
                    foreach (var item in dmaps)
                    {
                        var task = db.Tasks.Create();
                        task.Date = DateTime.Now;
                        task.DistributionMapId = item.Id.Value;
                        task.Name = item.Name;
                        task.Status = 0;
                        task.AuditorId = 1;
                        taskList.Add(task);
                    }
                    db.Tasks.AddRange(taskList);

                    db.Status.Add(new StatusInfo { Status = 2, Campaign = dbCampaign, User = user });
                    await db.SaveChangesAsync();
                    tran.Commit();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return Json(new { success = false, debug = ex.ToString() });
                }
            }
        }

        [Route("{campaignId:int}/dismiss")]
        [HttpPut]
        public async Task<IHttpActionResult> DismissCampaign(int? campaignId, [FromBody] int? userId)
        {
            var dbCampaign = await db.Campaigns.FindAsync(campaignId);
            if (dbCampaign == null)
            {
                return NotFound();
            }
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var campaignStatus = dbCampaign.Status.Max(i => i.Status);
            switch (campaignStatus)
            {
                case 1:
                    return await DismissToCampaign(user, dbCampaign);
                case 2:
                    return await DismissToDistribution(user, dbCampaign);
                default:
                    return Json(new { success = false });
            }
        }

        /// <summary>
        /// Dismiss from Distribution to Campaign
        /// </summary>
        /// <param name="dbCampaign"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> DismissToCampaign(User user, Campaign dbCampaign)
        {
            var submaps = db.SubMaps.Where(i => i.CampaignId == dbCampaign.Id).Select(i => i.Id).ToList();
            if (submaps != null && submaps.Count > 0)
            {
                foreach (var submapId in submaps)
                {
                    var dmaps = db.DistributionMaps.Where(i => i.SubMapId == submapId).Select(i => i.Id).ToList();
                    if (dmaps != null && dmaps.Count > 0)
                    {
                        var fakeDmaps = new List<DistributionMap>();
                        foreach (var dmapId in dmaps)
                        {
                            db.DistributionMapRecords.RemoveRange(db.DistributionMapRecords.Where(i => i.DistributionMapId == dmapId));
                            db.DistributionMapCoordinates.RemoveRange(db.DistributionMapCoordinates.Where(i => i.DistributionMapId == dmapId));
                            await db.SaveChangesAsync();
                            fakeDmaps.Add(db.DistributionMaps.Attach(new DistributionMap { Id = dmapId }));
                        }
                        db.DistributionMaps.RemoveRange(fakeDmaps);
                        await db.SaveChangesAsync();
                    }
                }
            }

            db.Status.RemoveRange(db.Status.Where(i => i.CampaignId == dbCampaign.Id));
            await db.SaveChangesAsync();

            var newStatus = db.Status.Create();
            newStatus.CampaignId = dbCampaign.Id.Value;
            newStatus.Status = 0;
            newStatus.UserId = user.Id;
            db.Status.Add(newStatus);
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }

        /// <summary>
        /// Dismiss from Monitor to Distribution
        /// </summary>
        /// <param name="dbCampaign"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> DismissToDistribution(User user, Campaign dbCampaign)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    var submaps = db.SubMaps.Where(i => i.CampaignId == dbCampaign.Id).Select(i => i.Id).ToList();
                    if (submaps != null && submaps.Count > 0)
                    {
                        foreach (var submapId in submaps)
                        {
                            var dmaps = db.DistributionMaps.Where(i => i.SubMapId == submapId).Select(i => i.Id).ToList();
                            if (dmaps != null && dmaps.Count > 0)
                            {
                                foreach (var dmapId in dmaps)
                                {
                                    var tasks = await db.Tasks.Where(i => i.DistributionMapId == dmapId).Select(i => i.Id).ToListAsync();
                                    var gtus = await db.TaskGtuInfoMappings.Where(i => tasks.Contains(i.TaskId)).Select(i => (long?)i.Id).ToListAsync();

                                    db.GtuInfos.RemoveRange(db.GtuInfos.Where(i => gtus.Contains(i.TaskgtuinfoId)));
                                    await db.SaveChangesAsync();

                                    db.TaskGtuInfoMappings.RemoveRange(db.TaskGtuInfoMappings.Where(i => tasks.Contains(i.TaskId)));
                                    await db.SaveChangesAsync();

                                    db.TaskTimes.RemoveRange(db.TaskTimes.Where(i => tasks.Contains(i.TaskId)));
                                    await db.SaveChangesAsync();

                                    db.Tasks.RemoveRange(db.Tasks.Where(i => i.DistributionMapId == dmapId));

                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                    }

                    db.Status.RemoveRange(db.Status.Where(i => i.CampaignId == dbCampaign.Id && i.Status > 1));
                    await db.SaveChangesAsync();

                    var newStatus = db.Status.Create();
                    newStatus.CampaignId = dbCampaign.Id.Value;
                    newStatus.Status = 1;
                    newStatus.UserId = user.Id;
                    db.Status.Add(newStatus);
                    await db.SaveChangesAsync();

                    tran.Commit();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return Json(new { success = false, ex = ex.ToString() });
                }
            }
        }

        [Route("copy/{campaignId:int}")]
        [HttpPost]
        public async Task<IHttpActionResult> CopyCampaign(int campaignId)
        {
            var sourceCampaign = await db.Campaigns.FindAsync(campaignId);
            if (sourceCampaign == null)
            {
                return Json(new { success = false });
            }

            User loginedUser;
            if (!UserManager.TryGetCurrentUser(out loginedUser))
            {
                return Json(new { success = false, error = "NOT_LOGIN" });
            }

            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    db.Configuration.ValidateOnSaveEnabled = false;

                    var user = loginedUser;
                    #region clone campaign base info
                    var newCampaign = db.Campaigns.Create();
                    newCampaign.AreaDescription = sourceCampaign.AreaDescription;
                    newCampaign.ClientCode = sourceCampaign.ClientCode;
                    newCampaign.ClientName = sourceCampaign.ClientName + " - copy";
                    newCampaign.ContactName = sourceCampaign.ContactName;
                    newCampaign.CreatorName = sourceCampaign.CreatorName;
                    newCampaign.CustemerName = sourceCampaign.CustemerName;
                    newCampaign.Date = DateTime.Today; //sourceCampaign.Date;
                    newCampaign.Description = sourceCampaign.Description;
                    newCampaign.Latitude = sourceCampaign.Latitude;
                    newCampaign.Logo = sourceCampaign.Logo;
                    newCampaign.Longitude = sourceCampaign.Longitude;
                    newCampaign.Name = sourceCampaign.Name;
                    newCampaign.Sequence = sourceCampaign.Sequence;
                    newCampaign.UserName = sourceCampaign.UserName;
                    newCampaign.ZoomLevel = sourceCampaign.ZoomLevel;

                    db.Campaigns.Add(newCampaign);
                    await db.SaveChangesAsync();
                    #endregion

                    #region status
                    var dbStatus = db.Status.Create();
                    dbStatus.Campaign = newCampaign;
                    dbStatus.Status = 0;
                    dbStatus.UserId = user.Id;
                    db.Status.Add(dbStatus);
                    await db.SaveChangesAsync();
                    #endregion

                    #region campaign record
                    if (sourceCampaign.CampaignRecords.Count() > 0)
                    {
                        var records = sourceCampaign.CampaignRecords.Select(i => new CampaignRecord
                        {
                            Campaign = newCampaign,
                            AreaId = i.AreaId,
                            Classification = i.Classification,
                            Value = i.Value
                        });
                        db.CampaignRecords.AddRange(records);
                        await db.SaveChangesAsync();
                    }
                    #endregion
                    #region campaign classifications
                    if (sourceCampaign.CampaignClassifications.Count() > 0)
                    {
                        var classifications = sourceCampaign.CampaignClassifications.Select(i => new CampaignClassification
                        {
                            Campaign = newCampaign,
                            Classification = i.Classification
                        });
                        db.CampaignClassifications.AddRange(classifications);
                        await db.SaveChangesAsync();
                    }
                    #endregion
                    #region campaign address

                    int addrCount = sourceCampaign.Addresses.Count();
                    if (addrCount > 0)
                    {
                        Address[] copies = new Address[addrCount];
                        sourceCampaign.Addresses.CopyTo(copies, 0);
                        foreach (var item in copies)
                        {
                            item.Campaign = newCampaign;
                            db.Addresses.Add(item);
                            await db.SaveChangesAsync();

                            if (item.Radiuses.Count() > 0)
                            {
                                var addressRadiuses = item.Radiuses.Select(i => new Radiuses
                                {
                                    IsDisplay = i.IsDisplay,
                                    Length = i.Length,
                                    LengthMeasuresId = i.LengthMeasuresId,
                                    Address = item
                                });
                                db.Radiuses.AddRange(addressRadiuses);
                                await db.SaveChangesAsync();
                            }
                        }
                    }
                    #endregion
                    #region campaign croute imported
                    if (sourceCampaign.CampaignCRouteImporteds.Count() > 0)
                    {
                        var crouteImported = sourceCampaign.CampaignCRouteImporteds.Select(i => new CampaignCRouteImported
                        {
                            CampaignId = newCampaign.Id.Value,
                            IsPartModified = i.IsPartModified,
                            PartPercentage = i.PartPercentage,
                            Penetration = i.Penetration,
                            PremiumCRouteId = i.PremiumCRouteId,
                            Total = i.Total
                        });
                        db.CampaignCRouteImporteds.AddRange(crouteImported);
                        await db.SaveChangesAsync();
                    }
                    #endregion
                    #region campaign percentage colors
                    if (sourceCampaign.CampaignPercentageColors.Count() > 0)
                    {
                        var colors = sourceCampaign.CampaignPercentageColors.Select(i => new CampaignPercentageColor
                        {
                            CampaignId = newCampaign.Id.Value,
                            ColorId = i.ColorId,
                            Max = i.Max,
                            Min = i.Min
                        });
                        db.CampaignPercentageColors.AddRange(colors);
                        await db.SaveChangesAsync();
                    }
                    #endregion
                    #region campaign sub maps
                    if (sourceCampaign.SubMaps.Count() > 0)
                    {
                        foreach (var sourceSubmap in sourceCampaign.SubMaps)
                        {
                            var newSubmap = db.SubMaps.Create();
                            newSubmap.Campaign = newCampaign;
                            newSubmap.ColorB = sourceSubmap.ColorB;
                            newSubmap.ColorG = sourceSubmap.ColorG;
                            newSubmap.ColorR = sourceSubmap.ColorR;
                            newSubmap.ColorString = sourceSubmap.ColorString;
                            newSubmap.Name = sourceSubmap.Name;
                            newSubmap.OrderId = sourceSubmap.OrderId;
                            newSubmap.Penetration = sourceSubmap.Penetration;
                            newSubmap.Percentage = sourceSubmap.Percentage;
                            newSubmap.Total = sourceSubmap.Total;
                            newSubmap.TotalAdjustment = sourceSubmap.TotalAdjustment;
                            newSubmap.CountAdjustment = sourceSubmap.CountAdjustment;

                            db.SubMaps.Add(newSubmap);
                            await db.SaveChangesAsync();

                            if (sourceSubmap.SubMapRecords.Count() > 0)
                            {
                                var submapRecords = sourceSubmap.SubMapRecords.Select(i => new SubMapRecord
                                {
                                    SubMap = newSubmap,
                                    AreaId = i.AreaId,
                                    Classification = i.Classification,
                                    Value = i.Value,
                                    Code = i.Code,
                                });
                                db.SubMapRecords.AddRange(submapRecords);
                                await db.SaveChangesAsync();
                            }

                            if (sourceSubmap.SubMapCoordinates.Count() > 0)
                            {
                                var submapCoordinates = sourceSubmap.SubMapCoordinates.Select(i => new SubMapCoordinate
                                {
                                    SubMap = newSubmap,
                                    Latitude = i.Latitude,
                                    Longitude = i.Longitude,
                                });
                                db.SubMapCoordinates.AddRange(submapCoordinates);
                                await db.SaveChangesAsync();
                            }
                        }
                    }
                    #endregion

                    trans.Commit();

                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            Id = newCampaign.Id,
                            AreaDescription = newCampaign.AreaDescription,
                            ClientCode = newCampaign.ClientCode,
                            ClientName = newCampaign.ClientName,
                            ContactName = newCampaign.ContactName,
                            CreatorName = newCampaign.CreatorName,
                            CustemerName = newCampaign.CustemerName,
                            Description = newCampaign.Description,
                            Date = newCampaign.Date,
                            Name = newCampaign.Name,
                            Sequence = newCampaign.Sequence,
                            UserNam = newCampaign.UserName
                        }
                    });
                }
                catch
                {
                    trans.Rollback();
                    return Json(new { success = false });
                }
            }

        }

        [Route("maintain")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllowCopyCampaignList()
        {
            var result = await db.Campaigns.Include(i => i.Status)
                .OrderByDescending(i => i.Date)
                .ThenBy(i => i.CustemerName)
                .ThenBy(i => i.UserName)
                .ThenByDescending(i => i.Sequence)
                .ToListAsync();
            List<Models.Campaign> allowCopyCampaign = new List<Models.Campaign>();
            foreach (var item in result)
            {
                if ((item.Status.Max(i => i.Status) ?? 0) < 1 && item.SubMaps.Count() > 0)
                {
                    allowCopyCampaign.Add(item);
                }
            }
            return Json(allowCopyCampaign.Select(i => new
            {
                i.Id,
                Name = string.Format("{0}-{1}-{2}-{3}-{4}",
                    i.Date.Value.ToString("MMddyy"),
                    i.ClientCode,
                    i.CreatorName,
                    i.AreaDescription,
                    i.Sequence),
                i.UserName
            }));
        }

        //[Route("{campaignId:int}/export")]
        //[HttpGet]
        //public async Task<IHttpActionResult> Export(int campaignId)
        //{
        //    var campaign = await db.Campaigns.FindAsync(campaignId);
        //    if (campaign == null)
        //    {
        //        return NotFound();
        //    }
        //    var submaps = campaign.SubMaps.OrderBy(i => i.OrderId)
        //        .Select(s => new
        //        {
        //            s.Name,
        //            s.ColorR,
        //            s.ColorG,
        //            s.ColorB,
        //            s.ColorString,
        //            s.CountAdjustment,
        //            s.TotalAdjustment,
        //            SubMapCRoutes = s.SubMapRecords.Where(i => i.Classification == 15 && i.Value == true).Select(i => i.AreaId).ToList(),
        //            SubMapGeoCode = new List<string>(),
        //            //SubMapAreas = new List<ViewModel.ImportCRoute>(),
        //            Boundary = s.SubMapCoordinates.OrderBy(i => i.Id).Select(i => new { Latitude = i.Latitude, Longitude = i.Longitude }).ToList()
        //        }).ToArray();

        //    var address = campaign.Addresses.Select(i => new
        //    {
        //        i.AddressName,
        //        i.Color,
        //        i.Latitude,
        //        i.Longitude,
        //        i.OriginalLatitude,
        //        i.OriginalLongitude,
        //        i.Picture,
        //        i.ZipCode,
        //        Radiuses = i.Radiuses.Select(r => new { r.Length, r.LengthMeasuresId, r.IsDisplay }).ToList()
        //    });
        //    var userId = await db.Status.Where(i => i.CampaignId == campaign.Id).OrderBy(i => i.Status).Select(i => i.UserId).FirstOrDefaultAsync();
        //    var user = db.Users.FirstOrDefault(i => i.Id == userId);
        //    var classifications = campaign.CampaignClassifications.Select(i => i.Classification).ToList();

        //    var allAreas = submaps.SelectMany(i => i.SubMapCRoutes).ToList();
        //    var allAreaMap = await QueryCRoutesById(allAreas);
        //    HashSet<string> allGeoCode = new HashSet<string>();
        //    foreach (var item in submaps)
        //    {
        //        foreach (var id in item.SubMapCRoutes)
        //        {
        //            if (id.HasValue && allAreaMap.ContainsKey(id.Value) && !allGeoCode.Contains(allAreaMap[id.Value].GEOCODE))
        //            {
        //                //item.SubMapAreas.Add(allAreaMap[id.Value]);
        //                item.SubMapGeoCode.Add(allAreaMap[id.Value].GEOCODE);
        //                allGeoCode.Add(allAreaMap[id.Value].GEOCODE);
        //            }
        //        }
        //    }
        //    var colors = campaign.CampaignPercentageColors.Select(i => new
        //    {
        //        i.ColorId,
        //        i.Max,
        //        i.Min
        //    }).ToList();

        //    var cRouteImporteds = await QueryPenetrationByCampaignId(campaignId);

        //    return Json(new
        //    {
        //        campaign.AreaDescription,
        //        campaign.ClientCode,
        //        campaign.ClientName,
        //        campaign.ContactName,
        //        campaign.CreatorName,
        //        campaign.CustemerName,
        //        campaign.Date,
        //        campaign.Description,
        //        campaign.Latitude,
        //        campaign.Longitude,
        //        campaign.Logo,
        //        campaign.Name,
        //        campaign.ZoomLevel,
        //        campaign.UserName,
        //        UserStatus = user.UserName,
        //        Address = address,
        //        Classifications = classifications,
        //        SubMap = submaps != null ? submaps.Select(s => new
        //        {
        //            s.ColorR,
        //            s.ColorG,
        //            s.ColorB,
        //            s.ColorString,
        //            s.Name,
        //            s.CountAdjustment,
        //            s.TotalAdjustment,
        //            s.Boundary,
        //            s.SubMapGeoCode
        //        }) : null,
        //        PercentageColors = colors,
        //        //Penetration = cRouteImporteds
        //    });

        //}

        //[Route("import")]
        //[HttpPost]
        //public async Task<IHttpActionResult> Import([FromBody]ViewModel.ImportCampaign campaign)
        //{
        //    StringBuilder info = new StringBuilder();
        //    //var crouteGeoCode = campaign.SubMap.SelectMany(i => i.CRoutes).Select(i=>i.GEOCODE).ToList();
        //    //var allCRoutes = await QueryCRoutesByGeoCode(crouteGeoCode);

        //    #region Check
        //    List<Models.Location> boundary = null;
        //    List<int> addedCRoute = new List<int>();
        //    StringBuilder message = null;
        //    List<int> systemCRoutes = new List<int>();
        //    HashSet<int> alreadyAddToSubMapCRoute = new HashSet<int>();
        //    foreach (var submap in campaign.SubMap)
        //    {
        //        if (submap.Boundary == null || submap.Boundary.Count == 0)
        //        {
        //            continue;
        //        }

        //        #region Check SubMAP
        //        //build oringal boundary
        //        var orginalLinearRing = submap.Boundary.Select(i => new Coordinate(i.Longitude ?? 0, i.Latitude ?? 0)).ToList();
        //        orginalLinearRing.Add(new Coordinate(orginalLinearRing[0].X, orginalLinearRing[0].Y));
        //        var orignalPolygon = new Polygon(new LinearRing(orginalLinearRing.ToArray()));
        //        orignalPolygon.Buffer(0);
        //        double? topRightLat = null, topRightLng = null, bottomLeftLat = null, bottomLeftLng = null;
        //        foreach (var latlng in orignalPolygon.Envelope.Coordinates)
        //        {
        //            topRightLat = !topRightLat.HasValue || latlng.Y > topRightLat ? latlng.Y : topRightLat;
        //            topRightLng = !topRightLng.HasValue || latlng.X > topRightLng ? latlng.X : topRightLng;
        //            bottomLeftLat = !bottomLeftLat.HasValue || latlng.Y < bottomLeftLat ? latlng.Y : bottomLeftLat;
        //            bottomLeftLng = !bottomLeftLng.HasValue || latlng.X < bottomLeftLng ? latlng.X : bottomLeftLng;

        //        }

        //        List<int?> cRoutes = await QueryCRouteInBox(topRightLat.Value, topRightLng.Value, bottomLeftLat.Value, bottomLeftLng.Value);
        //        HashSet<string> subMapGeoCode = new HashSet<string>();
        //        foreach (var item in submap.SubMapGeoCode)
        //        {
        //            if (!subMapGeoCode.Contains(item))
        //            {
        //                subMapGeoCode.Add(item);
        //            }
        //        }
        //        if (!ValidateCRouteArea(orignalPolygon, alreadyAddToSubMapCRoute, subMapGeoCode, cRoutes, out addedCRoute, out boundary, out message))
        //        {
        //            return Json(new
        //            {
        //                success = false,
        //                info = string.Format("validate croute in submap {0} failed.\r\n{1}", submap.Name, message)
        //            });
        //        }
        //        submap.SubMapCoordinate = boundary;
        //        submap.PremiumCRoutes = addedCRoute;
        //        foreach (var id in addedCRoute)
        //        {
        //            if (!alreadyAddToSubMapCRoute.Contains(id))
        //            {
        //                alreadyAddToSubMapCRoute.Add(id);
        //            }
        //        }

        //        #endregion

        //        #region Check DMap
        //        //if (submap.DMap != null && submap.DMap.Count > 0)
        //        //{
        //        //    foreach (var dmap in submap.DMap)
        //        //    {
        //        //        systemCRoutes.Clear();
        //        //        if (dmap.CRoutes == null || dmap.CRoutes.Count == 0)
        //        //        {
        //        //            continue;
        //        //        }
        //        //        foreach (var item in dmap.CRoutes)
        //        //        {
        //        //            if (!allCRoutes.ContainsKey(item.GEOCODE))
        //        //            {
        //        //                return Json(new
        //        //                {
        //        //                    success = false,
        //        //                    info = string.Format("Find GEOCODE {0} {1}/{2} failed in DMAP, the geocode is not exist", item.GEOCODE, item.PartCount, item.PartTotal)
        //        //                });
        //        //            }
        //        //            var currentCode = allCRoutes[item.GEOCODE];
        //        //            if (item.PartTotal != currentCode.Count)
        //        //            {
        //        //                return Json(new
        //        //                {
        //        //                    success = false,
        //        //                    info = string.Format("Find GEOCODE {0} {1}/{2} failed in DMAP, the croute partcount changed", item.GEOCODE, item.PartCount, item.PartTotal)
        //        //                });
        //        //            }
        //        //            var geoCode = currentCode.Where(i => i.PartCount == item.PartCount).FirstOrDefault();
        //        //            if (geoCode == null)
        //        //            {
        //        //                return Json(new
        //        //                {
        //        //                    success = false,
        //        //                    info = string.Format("Find GEOCODE {0} {1}/{2} failed in DMAP, the current part croute is not exist", item.GEOCODE, item.PartCount, item.PartTotal)
        //        //                });
        //        //            }
        //        //            systemCRoutes.Add(geoCode.Id.Value);
        //        //        }
        //        //        if (!ValidateCRouteArea(systemCRoutes, out boundary, out message))
        //        //        {
        //        //            return Json(new
        //        //            {
        //        //                success = false,
        //        //                info = string.Format("validate croute in submap {0} failed.\r\n{1}", submap.Name, message)
        //        //            });
        //        //        }
        //        //        dmap.PremiumCRoutes = systemCRoutes;
        //        //        dmap.Boundary = boundary;
        //        //    }
        //        //}
        //        #endregion
        //    }


        //    #endregion

        //    try
        //    {
        //        await SaveImportCampaign(campaign);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, info = ex.ToString() });
        //    }

        //    //dbCampaign.Addresses
        //    return Json(new { success = true, info = info.ToString() });
        //}

        [Route("{campaignId:int}/location/{zoom:double}/{lng:double}/{lat:double}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCampaignLocation(int campaignId, double zoom, double lng, double lat)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId).ConfigureAwait(false);
            if(campaign != null)
            {
                campaign.ZoomLevel = (int)Math.Round(zoom);
                campaign.Latitude = lat;
                campaign.Longitude = lng;

                await db.SaveChangesAsync().ConfigureAwait(false);
            }

            return Ok();
        }

        [Route("{campaignId:int}/penetration")]
        [HttpGet]
        public async Task<IHttpActionResult> ExportCampaignPenetration(int campaignId)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId).ConfigureAwait(false);
            if (campaign == null)
            {
                return NotFound();
            }

            var result = new Dictionary<Classifications, List<ImportArea>>();
            foreach (var subMap in campaign.SubMaps)
            {
                var firstArea = subMap.SubMapRecords.FirstOrDefault();
                if (firstArea == null)
                {
                    continue;
                }
                var classification = (Classifications)firstArea.Classification;
                var areaIdArray = subMap.SubMapRecords.Select(i => i.AreaId).ToArray();
                List<ImportArea> data = new List<ImportArea>();
                List<string> codes = null;
                switch (classification)
                {
                    case Classifications.Z5:
                         codes = db.FiveZipAreas.Where(i => areaIdArray.Contains(i.Id)).Select(i => i.Code).ToList();
                        data = db.FiveZipAreas.Where(i=> codes.Contains(i.Code))
                            .Select(i => new ImportArea
                            {
                                Name = i.Code,
                                APT = i.APT_COUNT,
                                HOME = i.HOME_COUNT,
                            })
                            .ToList();
                        break;
                    case Classifications.PremiumCRoute:
                         codes = db.PremiumCRoutes.Where(i => areaIdArray.Contains(i.Id)).Select(i=>i.Code).ToList();
                        data = db.PremiumCRoutes.Where(i=> codes.Contains(i.Code) )
                           .Select(i => new ImportArea
                           {
                               Name = i.Code,
                               APT = i.APT_COUNT,
                               HOME = i.HOME_COUNT,
                           })
                            .ToList();
                        break;
                    default:
                        break;
                }

                if (!result.ContainsKey(classification))
                {
                    result.Add(classification, new List<ImportArea>());
                }
                result[classification].AddRange(data);
            }

            var wb = new NanoXLSX.Workbook(false);
            foreach (var item in result)
            {
                var sheetName = item.Key.ToString();
                wb.AddWorksheet(sheetName);
                wb.SetCurrentWorksheet(sheetName);
                var addHeader = true;
                foreach (var row in item.Value)
                {
                    if (addHeader)
                    {
                        switch (item.Key)
                        {
                            case Classifications.Z5:
                                wb.CurrentWorksheet.AddNextCell("ZIP");

                                break;
                            case Classifications.PremiumCRoute:
                                wb.CurrentWorksheet.AddNextCell("PREMIUMCROUTE");
                                break;
                        }
                        wb.CurrentWorksheet.AddNextCell("APT");
                        wb.CurrentWorksheet.AddNextCell("HOME");
                        wb.CurrentWorksheet.AddNextCell("TOTAL");
                        wb.CurrentWorksheet.AddNextCell("PENETRATION");

                        addHeader = false;
                        wb.CurrentWorksheet.GoToNextRow();
                    }


                    wb.CurrentWorksheet.AddNextCell(row.Name);
                    wb.CurrentWorksheet.AddNextCell(row.APT);
                    wb.CurrentWorksheet.AddNextCell(row.HOME);
                    switch (campaign.AreaDescription)
                    {
                        case "APT ONLY":
                            wb.CurrentWorksheet.AddNextCell(row.APT ?? 0);
                            break;
                        case "HOME ONLY":
                            wb.CurrentWorksheet.AddNextCell(row.HOME ?? 0);
                            break;
                        case "APT + HOME":
                        default:
                            wb.CurrentWorksheet.AddNextCell((row.APT ?? 0) + (row.HOME ?? 0));
                            break;
                    }
                    wb.CurrentWorksheet.AddNextCell(row.Penetration);
                    wb.CurrentWorksheet.GoToNextRow();
                }
            }
            MemoryStream ms = new MemoryStream();
            await wb.SaveAsStreamAsync(ms, true).ConfigureAwait(false);
            return new ExcelResult($"{campaign.Name}.xlsx", ms);
        }

        [Route("{campaignId:int}/penetration")]
        [HttpPost]
        public async Task<dynamic> ImportCampaignPenetration(int campaignId)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId).ConfigureAwait(false);
            if (campaign == null)
            {
                return NotFound();
            }

            MultipartFormDataStreamProvider provider = null;
            try
            {
                provider = new MultipartFormDataStreamProvider(TempPath);
                await Request.Content.ReadAsMultipartAsync(provider).ConfigureAwait(false);
            }
            catch
            {

            }

            if (provider == null)
            {
                return NotFound();
            }

            var postedFile = provider.FileData.FirstOrDefault();

            if (postedFile == null)
            {
                return NotFound();
            }
            try
            {
                var wb = NanoXLSX.Workbook.Load(postedFile.LocalFileName);
                var totalRows = wb.CurrentWorksheet.GetLastDataRowNumber();
                List<ImportArea> z5Data = new List<ImportArea>();
                List<ImportArea> cRouteData = new List<ImportArea>();
                for (var rowIndex = 1; rowIndex <= totalRows; rowIndex++)
                {
                    var name = wb.CurrentWorksheet.GetCell(0, rowIndex)?.Value?.ToString() ?? String.Empty;
                    var totalValue = wb.CurrentWorksheet.GetCell(3, rowIndex)?.Value?.ToString() ?? "0";
                    var penetrationValue = wb.CurrentWorksheet.GetCell(4, rowIndex)?.Value?.ToString() ?? "0";
                    int.TryParse(totalValue, out int total);
                    int.TryParse(penetrationValue, out int penetration);
                    if (name.Length == 5)
                    {
                        z5Data.Add(new ImportArea
                        {
                            Name = name,
                            Penetration = penetration,
                            Total = total
                        });
                    }
                    else
                    {
                        cRouteData.Add(new ImportArea
                        {
                            Name = name,
                            Penetration = penetration,
                            Total = total
                        });
                    }

                }

                using (var tran = db.Database.BeginTransaction())
                {
                    await db.CampaignFiveZipImporteds.Where(i => i.CampaignId == campaignId).DeleteAsync().ConfigureAwait(false);
                    await db.CampaignCRouteImporteds.Where(i => i.CampaignId == campaignId).DeleteAsync().ConfigureAwait(false);

                    if (z5Data.Count > 0)
                    {
                        var ids = z5Data.Select(i => i.Name).Distinct().ToArray();
                        var kv = db.FiveZipAreas.Where(i => ids.Contains(i.Code))
                            .Select(i => new { i.Code, i.Id })
                            .ToDictionary(i => i.Code, i => i.Id);
                        var importData = z5Data.Select(i => new CampaignFiveZipImported
                        {
                            CampaignId = campaignId,
                            FiveZipAreaId = kv[i.Name],
                            Penetration = i.Penetration,
                            Code = i.Name,
                            Total = i.Total
                        });

                        db.CampaignFiveZipImporteds.AddRange(importData);
                    }

                    if (cRouteData.Count > 0)
                    {
                        var ids = cRouteData.Select(i => i.Name).Distinct().ToArray();
                        var kv = db.PremiumCRoutes.Where(i => ids.Contains(i.Code) ).ToDictionary(i => i.Code, i => i.Id);
                        var importData = cRouteData.Select(i => new CampaignCRouteImported
                        {
                            CampaignId = campaignId,
                            PremiumCRouteId = kv[i.Name],
                            Penetration = i.Penetration,
                            Code = i.Name,
                            Total = i.Total
                        });

                        db.CampaignCRouteImporteds.AddRange(importData);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }

                    await db.SaveChangesAsync().ConfigureAwait(false);

                    tran.Commit();
                }

                // fix submap penetration
                foreach (var subMap in campaign.SubMaps)
                {
                    var classifications = (Classifications)subMap.SubMapRecords.FirstOrDefault()?.Classification;
                    List<string> codes;
                    int? target = null;
                    switch (classifications)
                    {
                        case Classifications.Z5:
                            codes = subMap.SubMapRecords.Select(i=>i.Code).Distinct().ToList();
                            target = z5Data.Where(i => codes.Contains(i.Name)).Sum(i => i.Penetration ?? 0);
                            break;
                        case Classifications.PremiumCRoute:
                            codes = subMap.SubMapRecords.Select(i => i.Code).Distinct().ToList();
                            target = cRouteData.Where(i => codes.Contains(i.Name)).Sum(i => i.Penetration ?? 0);
                            break;
                    }
                    if (target.HasValue)
                    {
                        subMap.Penetration = target.Value;
                    }
                }
                await db.SaveChangesAsync().ConfigureAwait(false);
            }
            finally
            {
                try
                {
                    File.Delete(postedFile.LocalFileName);
                }
                catch { }

            }

            return new { success = true };
        }

        private async Task<bool> SaveImportCampaign(ViewModel.ImportCampaign campaign)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ValidateOnSaveEnabled = false;
            #region Save Campaign
            var dbCampaign = db.Campaigns.Create();
            dbCampaign.AreaDescription = campaign.AreaDescription ?? "";
            dbCampaign.ClientCode = campaign.ClientCode ?? "";
            dbCampaign.ClientName = campaign.ClientName ?? "";
            dbCampaign.ContactName = campaign.ContactName ?? "";
            dbCampaign.CreatorName = campaign.CreatorName ?? "";
            dbCampaign.CustemerName = campaign.CustemerName ?? "";
            dbCampaign.Date = campaign.Date;
            dbCampaign.Description = campaign.Description ?? "";
            dbCampaign.Latitude = campaign.Latitude;
            dbCampaign.Logo = campaign.Logo;
            dbCampaign.Longitude = campaign.Longitude;
            dbCampaign.Name = campaign.Name ?? "";
            dbCampaign.UserName = campaign.UserName;
            dbCampaign.ZoomLevel = campaign.ZoomLevel;
            dbCampaign.Sequence = 1;
            db.Campaigns.Add(dbCampaign);
            await db.SaveChangesAsync();

            #endregion

            #region save campaign user mapping
            var user = await db.Users.FirstOrDefaultAsync(i => i.UserName == campaign.UserStatus);
            db.Status.Add(new StatusInfo
            {
                Campaign = dbCampaign,
                User = user,
                Status = 0
            });
            await db.SaveChangesAsync();
            #endregion

            #region Save SubMap
            int subMapIndex = 1;
            if (campaign.SubMap != null && campaign.SubMap.Count > 0)
            {
                foreach (var subMap in campaign.SubMap)
                {
                    var dbSubMap = db.SubMaps.Create();
                    dbSubMap.Campaign = dbCampaign;
                    dbSubMap.ColorR = subMap.ColorR;
                    dbSubMap.ColorG = subMap.ColorG;
                    dbSubMap.ColorB = subMap.ColorB;
                    dbSubMap.ColorString = subMap.ColorString;
                    dbSubMap.CountAdjustment = subMap.CountAdjustment ?? 0;
                    dbSubMap.TotalAdjustment = subMap.TotalAdjustment ?? 0;
                    dbSubMap.Name = subMap.Name;
                    dbSubMap.OrderId = subMapIndex++;
                    //these value will changed later after croute have insert into database.
                    dbSubMap.Penetration = 0;
                    dbSubMap.Percentage = 0;
                    dbSubMap.Total = 0;

                    db.SubMaps.Add(dbSubMap);
                    await db.SaveChangesAsync();

                    if (subMap.PremiumCRoutes != null && subMap.PremiumCRoutes.Count > 0)
                    {
                        var submapRecord = subMap.PremiumCRoutes.Select(i => new SubMapRecord
                        {
                            AreaId = i,
                            Classification = 15,
                            SubMap = dbSubMap,
                            Value = true
                        });
                        db.SubMapRecords.AddRange(submapRecord);
                        await db.SaveChangesAsync();

                        var partCampaignRecord = subMap.PremiumCRoutes.Select(i => new CampaignRecord
                        {
                            AreaId = i,
                            Classification = 15,
                            Campaign = dbCampaign,
                            Value = true
                        });

                        db.CampaignRecords.AddRange(partCampaignRecord);
                        await db.SaveChangesAsync();
                    }

                    if (subMap.SubMapCoordinate != null && subMap.SubMapCoordinate.Count > 0)
                    {
                        var subMapCoordinate = subMap.SubMapCoordinate.Select(i => new SubMapCoordinate
                        {
                            Latitude = i.Latitude,
                            Longitude = i.Longitude,
                            SubMap = dbSubMap
                        }).ToList();
                        db.SubMapCoordinates.AddRange(subMapCoordinate);
                        await db.SaveChangesAsync();
                    }
                }
            }
            #endregion

            #region Save Classification
            await db.SaveChangesAsync();
            if (campaign.Classifications != null && campaign.Classifications.Count > 0)
            {
                var classification = campaign.Classifications.Select(i => new CampaignClassification
                {
                    Classification = i,
                    Campaign = dbCampaign
                });
                db.CampaignClassifications.AddRange(classification);
            }
            #endregion

            #region Save Address
            if (campaign.Address != null && campaign.Address.Count > 0)
            {
                foreach (var item in campaign.Address)
                {
                    var address = new Address
                    {
                        AddressName = item.AddressName,
                        Campaign = dbCampaign,
                        Color = item.Color,
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                        OriginalLatitude = item.OriginalLatitude,
                        OriginalLongitude = item.OriginalLongitude,
                        Picture = item.Picture,
                        ZipCode = item.ZipCode,
                    };

                    db.Addresses.Add(address);
                    await db.SaveChangesAsync();

                    if (item.Radiuses != null && item.Radiuses.Count > 0)
                    {
                        var addressRadiuses = item.Radiuses.Select(i => new Radiuses
                        {
                            IsDisplay = i.IsDisplay,
                            Length = i.Length,
                            LengthMeasuresId = i.LengthMeasuresId,
                            Address = address
                        });
                        db.Radiuses.AddRange(addressRadiuses);
                        await db.SaveChangesAsync();
                    }
                }
            }
            #endregion

            #region Save PercentageColors
            if (campaign.PercentageColors != null && campaign.PercentageColors.Count > 0)
            {
                var colors = campaign.PercentageColors.Select(i => new CampaignPercentageColor
                {
                    CampaignId = dbCampaign.Id.Value,
                    ColorId = i.ColorId,
                    Max = i.Max,
                    Min = i.Min
                });
                db.CampaignPercentageColors.AddRange(colors);
                await db.SaveChangesAsync();
            }
            #endregion

            #region Save Penetration
            //if (campaign.Penetration != null && campaign.Penetration.Count > 0)
            //{
            //    var campaignPenetration = new Dictionary<string, List<ViewModel.ImportPenetration>>();
            //    foreach (var item in campaign.Penetration)
            //    {
            //        if (!campaignPenetration.ContainsKey(item.GEOCODE))
            //        {
            //            campaignPenetration.Add(item.GEOCODE, new List<ViewModel.ImportPenetration>());
            //        }
            //        campaignPenetration[item.GEOCODE].Add(item);
            //    }

            //    var dbPenetration = new List<CampaignCRouteImported>();
            //    foreach (var item in campaignPenetration)
            //    {
            //        if (!allCRoutes.ContainsKey(item.Key))
            //        {
            //            continue;
            //        }
            //        var cRoute = allCRoutes[item.Key];
            //        if (cRoute.Count >= item.Value.Count)
            //        {
            //            for (var i = 0; i < item.Value.Count; i++)
            //            {
            //                dbPenetration.Add(new CampaignCRouteImported
            //                {
            //                    CampaignId = dbCampaign.Id,
            //                    IsPartModified = item.Value[i].IsPartModified,
            //                    PartPercentage = item.Value[i].PartPercentage,
            //                    Penetration = item.Value[i].Penetration,
            //                    PremiumCRouteId = cRoute[i].Id,
            //                    Total = CalcTotal(campaign.AreaDescription, cRoute[i])
            //                });
            //            }
            //        }
            //        else
            //        {
            //            for (var i = 0; i < cRoute.Count; i++)
            //            {
            //                dbPenetration.Add(new CampaignCRouteImported
            //                {
            //                    CampaignId = dbCampaign.Id,
            //                    IsPartModified = item.Value[i].IsPartModified,
            //                    PartPercentage = item.Value[i].PartPercentage,
            //                    Penetration = item.Value[i].Penetration,
            //                    PremiumCRouteId = cRoute[i].Id,
            //                    Total = CalcTotal(campaign.AreaDescription, cRoute[i])
            //                });
            //            }
            //            //add other source penetration to last added one
            //            var index = dbPenetration.Count - 1;
            //            for (var i = cRoute.Count - 1; i < item.Value.Count; i++)
            //            {
            //                dbPenetration[index].Penetration += item.Value[i].Penetration;
            //            }
            //        }
            //    }
            //    db.CampaignCRouteImporteds.AddRange(dbPenetration);
            //    await db.SaveChangesAsync();
            //}
            #endregion

            #region Update Total
            var submaps = dbCampaign.SubMaps.ToList();
            foreach (var item in submaps)
            {
                db.SubMaps.Attach(item);
                item.Total = await QuerySubMapTotal(dbCampaign.AreaDescription, item.Id.Value);
                db.Entry(item).State = EntityState.Modified;
            }
            await db.SaveChangesAsync();


            #endregion

            return true;
        }
        private async Task<Dictionary<int, ViewModel.ImportCRoute>> QueryCRoutesById(List<int?> allAreas)
        {
            if (allAreas == null || allAreas.Count == 0)
            {
                return new Dictionary<int, ViewModel.ImportCRoute>();
            }
            var values = new StringBuilder();
            values.AppendFormat("{0}", allAreas[0]);
            for (int i = 1; i < allAreas.Count; i++)
            {
                values.AppendFormat(", {0}", allAreas[i]);
            }

            string sql = @"
                SELECT [A].[Id],
                       [A].[GEOCODE],
                       [A].[PartCount],
                       [B].[Total]
                FROM [dbo].[premiumcroutes] AS [A]
                INNER JOIN
                  (SELECT GEOCODE,
                          COUNT(*) AS [Total]
                   FROM [dbo].[premiumcroutes]
                   GROUP BY GEOCODE) AS [B] ON [A].GEOCODE = [B].GEOCODE
                WHERE [A].[Id] IN ({0})";
            sql = string.Format(sql, values);

            var result = await db.Database.SqlQuery<ViewModel.ImportCRoute>(sql).ToListAsync();
            Dictionary<int, ViewModel.ImportCRoute> cRouteIdGeoName = new Dictionary<int, ViewModel.ImportCRoute>();
            foreach (var item in result)
            {
                cRouteIdGeoName.Add(item.Id.Value, item);
            }

            return cRouteIdGeoName;
        }
        private async Task<Dictionary<string, List<ViewModel.ImportCRoute>>> QueryCRoutesByGeoCode(List<string> allGeoCode)
        {
            var values = new StringBuilder();
            values.AppendFormat("'{0}'", allGeoCode[0]);
            for (int i = 1; i < allGeoCode.Count; i++)
            {
                values.AppendFormat(", '{0}'", allGeoCode[i]);
            }

            var sql = string.Format("SELECT [ID], [GEOCODE], [PartCount], [APT_COUNT], [HOME_COUNT] FROM [dbo].[premiumcroutes] WHERE [GEOCODE] IN ({0})", values);

            var result = await db.Database.SqlQuery<ViewModel.ImportCRoute>(sql).ToListAsync();
            Dictionary<string, List<ViewModel.ImportCRoute>> cRouteByGeoName = new Dictionary<string, List<ViewModel.ImportCRoute>>();
            foreach (var item in result)
            {
                if (!cRouteByGeoName.ContainsKey(item.GEOCODE))
                {
                    cRouteByGeoName.Add(item.GEOCODE, new List<ViewModel.ImportCRoute>());
                }
                cRouteByGeoName[item.GEOCODE].Add(item);
            }

            return cRouteByGeoName;
        }
        private async Task<List<ViewModel.ImportPenetration>> QueryPenetrationByCampaignId(int campaignId)
        {
            #region SQL
            var sql = @"
                SELECT [b].[GEOCODE] ,
                       [a].[Penetration] ,
                       [a].[PartPercentage] ,
                       CAST([a].[IsPartModified] as bit) AS [IsPartModified]
                FROM [dbo].[campaigncrouteimported] [a]
                INNER JOIN [dbo].[premiumcroutes] [b] ON [a].[PremiumCRouteId] = [b].[Id]
                WHERE [a].[CampaignId] = @CampaignId
                ORDER BY [b].[GEOCODE], [a].[Penetration]
                ";
            #endregion
            var result = await db.Database.SqlQuery<ViewModel.ImportPenetration>(sql, new SqlParameter("@CampaignId", campaignId)).ToListAsync();
            return result;
        }
        //private bool ValidateCRouteArea(Polygon orignalBoundary, HashSet<int> cRouteInOtherSubMap, HashSet<string> submapGeoCode, List<int?> area, out List<int> addedArea, out List<Models.Location> boundary, out StringBuilder message)
        //{
        //    boundary = new List<Models.Location>();
        //    addedArea = new List<int>();
        //    message = new StringBuilder();

        //    #region build croute polygon
        //    List<Geometry> croutePolygon = new List<Geometry>();
        //    var croutes = db.PremiumCRoutes.Where(i => area.Contains(i.Id)).ToDictionary(k => k.Id, v => v.Geom);
        //    foreach(var item in croutes)
        //    {
        //        var geom = WKTReader.Read(item.Value.AsText());
        //        geom.UserData = item.Key;
        //        croutePolygon.Add(geom);
        //    }
        //    //var values = new StringBuilder();
        //    //values.AppendFormat("{0}", area[0]);
        //    //for (int i = 1; i < area.Count; i++)
        //    //{
        //    //    values.AppendFormat(", '{0}'", area[i]);
        //    //}
        //    //var sql = string.Format("SELECT * FROM [dbo].[premiumcroutecoordinates] WHERE [PreminumCRouteId] in ({0}) ORDER BY [PreminumCRouteId], [Id]", values);
        //    //var result = db.Database.SqlQuery<Models.PremiumCRouteCoordinate>(sql);
        //    //int? lastAddedId = null;
        //    //List<Coordinate> crouteCoordinate = new List<Coordinate>();
            
        //    //var enumerator = result.GetEnumerator();
        //    //while (enumerator.MoveNext())
        //    //{
        //    //    if (lastAddedId.HasValue && lastAddedId != enumerator.Current.PreminumCRouteId)
        //    //    {
        //    //        crouteCoordinate.Add(crouteCoordinate[0]);
        //    //        Polygon polygon = new Polygon(new LinearRing(crouteCoordinate.ToArray()));
        //    //        polygon.Buffer(0);
        //    //        polygon.UserData = lastAddedId.Value;
        //    //        if (polygon.IsSimple && polygon.IsValid)
        //    //        {
        //    //            croutePolygon.Add(polygon);
        //    //        }

        //    //        crouteCoordinate.Clear();
        //    //    }
        //    //    crouteCoordinate.Add(new Coordinate { Y = enumerator.Current.Latitude.Value, X = enumerator.Current.Longitude.Value });
        //    //    lastAddedId = enumerator.Current.PreminumCRouteId;

        //    //}
        //    ////polygon must contant more than 2 points.
        //    //if (crouteCoordinate.Count > 1)
        //    //{
        //    //    crouteCoordinate.Add(crouteCoordinate[0]);
        //    //    Polygon polygon = new Polygon(new LinearRing(crouteCoordinate.ToArray()));
        //    //    polygon.Buffer(0);
        //    //    polygon.UserData = lastAddedId.Value;
        //    //    if (polygon.IsSimple && polygon.IsValid)
        //    //    {
        //    //        croutePolygon.Add(polygon);
        //    //    }
        //    //}
        //    #endregion

        //    // remove other 

        //    #region check the orignal boundary must contains these croute or if these croute have intersection with oringal boudnary then these croute geocode must in oringal submap croute geocodes
        //    //List<Geometry> validatedPolygon = new List<Geometry>();
        //    //foreach (Polygon item in croutePolygon)
        //    //{
        //    //    if (cRouteInOtherSubMap.Contains((int)item.UserData))
        //    //    {
        //    //        continue;
        //    //    }
        //    //    if (orignalBoundary.Contains(item))
        //    //    {
        //    //        validatedPolygon.Add(item);
        //    //    }
        //    //    else
        //    //    {
        //    //        var intersection = orignalBoundary.Intersection(item);
        //    //        bool haveIntersectionPolygon = false;
        //    //        if (intersection.IsEmpty)
        //    //        {
        //    //            haveIntersectionPolygon = false;
        //    //        }
        //    //        else if (intersection.GeometryType == "Polygon")
        //    //        {
        //    //            haveIntersectionPolygon = true;
        //    //        }
        //    //        else
        //    //        {
        //    //            for (var i = 0; i < intersection.NumGeometries; i++)
        //    //            {
        //    //                if (intersection.GetGeometryN(i).GeometryType == "Polygon")
        //    //                {
        //    //                    haveIntersectionPolygon = true;
        //    //                }
        //    //            }
        //    //        }

        //    //        if (haveIntersectionPolygon)
        //    //        {
        //    //            var itemCRoute = db.PremiumCRoutes.Find((int)item.UserData);
        //    //            if (itemCRoute != null && submapGeoCode.Contains(itemCRoute.GEOCODE))
        //    //            {
        //    //                validatedPolygon.Add(item);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    #endregion

        //    #region Union
        //    Polygon resultPolygon = null;
        //    Geometry unionPolygon = null;
        //    MultiPolygon cRouteCollection = null;
        //    try
        //    {
        //        cRouteCollection = new MultiPolygon(validatedPolygon.ToArray());
        //        unionPolygon = cRouteCollection.Union();
        //    }
        //    catch (Exception ex)
        //    {
        //        message.AppendFormat("union failed. {0}", ex.ToString());
        //        message.AppendFormat("already unioned Polygon: {0}", KMLWriter.WriteGeometry(cRouteCollection, 0));
        //        return false;
        //    }


        //    if (unionPolygon.NumGeometries > 1)
        //    {
        //        try
        //        {
        //            double maxArea = 0;
        //            for (var index = 0; index < unionPolygon.NumGeometries; index++)
        //            {
        //                var testPolygon = unionPolygon.GetGeometryN(index) as Polygon;
        //                resultPolygon = testPolygon != null && testPolygon.Area > maxArea ? testPolygon : resultPolygon;
        //                maxArea = resultPolygon.Area;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            message.AppendLine("find unioned max area polygon failed.");
        //            message.AppendLine(ex.ToString());
        //            return false;
        //        }
        //    }
        //    else if (unionPolygon.NumGeometries == 1)
        //    {
        //        resultPolygon = unionPolygon.GetGeometryN(0) as Polygon;
        //    }
        //    #endregion

        //    if (resultPolygon == null)
        //    {
        //        message.AppendFormat("union croute should be polygon, but this area is {0}", unionPolygon.GetGeometryN(0).GeometryType);
        //        return false;
        //    }
        //    //if(resultPolygon.Holes.Length > 0)
        //    //{
        //    //    message = "unionPolygon croute have holes";
        //    //    return false;
        //    //}
        //    foreach (var cRoute in validatedPolygon)
        //    {
        //        if (cRoute != null && resultPolygon.Contains(cRoute))
        //        {
        //            addedArea.Add((int)cRoute.UserData);
        //        }
        //    }
        //    boundary = resultPolygon.ExteriorRing.Coordinates.Select(i => new Models.Location
        //    {
        //        Latitude = i.Y,
        //        Longitude = i.X
        //    }).ToList();
        //    return true;

        //}
        //private async Task<List<int>> QueryCRouteInBox(double topRightLat, double topRightLng, double bottomLeftLat, double bottomLeftLng)
        //{
        //    string sql = @"SELECT PreminumCRouteId FROM premiumcroutecoordinates WHERE Latitude <= @TopRightLat AND Latitude >= @BottomLeftLat AND Longitude >= @BottomLeftLng AND Longitude < @TopRightLng GROUP BY PreminumCRouteId";
        //    SqlParameter[] param = new SqlParameter[4];
        //    param[0] = new SqlParameter("@TopRightLat", topRightLat);
        //    param[1] = new SqlParameter("@TopRightLng", topRightLng);
        //    param[2] = new SqlParameter("@BottomLeftLat", bottomLeftLat);
        //    param[3] = new SqlParameter("@BottomLeftLng", bottomLeftLng);
        //    var result = await db.Database.SqlQuery<int>(sql, param).ToListAsync();
        //    return result;
        //}
        private static int CalcTotal(string areaDescription, ViewModel.ImportCRoute croute)
        {
            switch (areaDescription)
            {
                case "APT ONLY":
                    return croute.APT_COUNT ?? 0;
                case "HOME ONLY":
                    return croute.HOME_COUNT ?? 0;
                case "APT + HOME":
                    return croute.APT_COUNT ?? 0 + croute.HOME_COUNT ?? 0;
                default:
                    return croute.APT_COUNT ?? 0;
            }
        }
        private async Task<int> QuerySubMapTotal(string areaDescription, int subMapId)
        {
            string sqlAPT = "SELECT SUM([B].[APT_COUNT]) FROM [dbo].[submaprecords] [A] INNER JOIN [dbo].[premiumcroutes] [B] ON [A].AreaId = [B].Id WHERE [A].[SubMapId] = @SubMapId";
            string sqlHOME = "SELECT SUM([B].[HOME_COUNT]) FROM [dbo].[submaprecords] [A] INNER JOIN [dbo].[premiumcroutes] [B] ON [A].AreaId = [B].Id WHERE [A].[SubMapId] = @SubMapId";
            string sqlAPT_HOME = "SELECT SUM([B].[APT_COUNT] + [B].[HOME_COUNT]) FROM [dbo].[submaprecords] [A] INNER JOIN [dbo].[premiumcroutes] [B] ON [A].AreaId = [B].Id WHERE [A].[SubMapId] = @SubMapId";
            string sql = null;
            switch (areaDescription)
            {
                case "APT ONLY":
                    sql = sqlAPT;
                    break;
                case "HOME ONLY":
                    sql = sqlHOME;
                    break;
                case "APT + HOME":
                    sql = sqlAPT_HOME;
                    break;
                default:
                    sql = sqlAPT;
                    break;
            }
            var total = await db.Database.SqlQuery<int>(sql, new SqlParameter("@SubMapId", subMapId)).FirstOrDefaultAsync();
            return total;
        }
    }
}
