using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using Vargainc.Timm.EF;
using Vargainc.Timm.REST.ViewModel;
using Vargainc.Timm.Models;
using NetTopologySuite.Geometries;
using Vargainc.Timm.Extentions;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("gtu")]
    public class GTUController : ApiController
    {
        private TimmContext db = new TimmContext();

        [Route("task/{taskId:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGtuListByTask(int taskId)
        {
            var task = await db.Tasks.FindAsync(taskId);
            if(task == null)
            {
                return NotFound();
            }
            var gtuColorList = await db.TaskGtuInfoMappings
                .Where(i => i.TaskId == taskId)
                .Select(i => new
                {
                    Id = i.GTUId,
                    Color = i.UserColor
                }).ToListAsync();
            var gtuColor = new Dictionary<int?, string>();
            foreach(var item in gtuColorList)
            {
                if (!gtuColor.ContainsKey(item.Id))
                {
                    gtuColor.Add(item.Id, item.Color);
                }
            }

            var assignedGtuList = await db.TaskGtuInfoMappings
                .Where(i => i.TaskId == taskId && i.UserId != null)
                .Select(i => new ViewGTU
                {
                    Id = i.GTUId,
                    UserColor = i.UserColor,
                    Company = i.Auditor.Company.Name,
                    Auditor = i.Auditor.FullName,
                    Role = i.Auditor.Role
                }).ToListAsync();
            Dictionary<int?, ViewGTU> assignedGtu = new Dictionary<int?, ViewGTU>();
            foreach (var item in assignedGtuList)
            {
                if (!assignedGtu.ContainsKey(item.Id))
                {
                    assignedGtu.Add(item.Id, item);
                }
            }

            //check userId == null means deleted gtu exist any gtu data
            var deleteButAlreadyHaveDataGTUList = await db.TaskGtuInfoMappings
                .Where(i => i.TaskId == taskId && i.UserId == null)
                .Select(i => new ViewGTU {
                    Id = i.GTUId,
                    UserColor = i.UserColor,
                    Company = i.Auditor.Company.Name,
                    Auditor = i.Auditor.FullName,
                    Role = i.Auditor.Role,
                    HaveData = db.GtuInfos.Where(g=>i.Id == g.TaskgtuinfoId).Any()
                })
                .Where(r=>r.HaveData == true)
                .ToListAsync();
            Dictionary<int?, ViewGTU> deleteButAlreadyHaveDataGTU = new Dictionary<int?, ViewGTU>();
            foreach (var item in deleteButAlreadyHaveDataGTUList)
            {
                if (!deleteButAlreadyHaveDataGTU.ContainsKey(item.Id))
                {
                    deleteButAlreadyHaveDataGTU.Add(item.Id, item);
                }
            }


            var checkTime = DateTime.Now.AddMinutes(-5);
            var onlineRecord = await db.GtuInfos
                .Where(i => i.dtReceivedTime >= checkTime)
                .OrderByDescending(i => i.dtReceivedTime).ThenBy(i => i.GtuUniqueID)
                .Select(i => new { i.Code, i.dwLatitude, i.dwLongitude })
                .ToListAsync();
            Dictionary<string, ViewModel.Location> onlineGTU = new Dictionary<string, ViewModel.Location>();
            foreach (var item in onlineRecord)
            {
                if (!onlineGTU.ContainsKey(item.Code))
                {
                    onlineGTU.Add(item.Code, new ViewModel.Location
                    {
                        Latitude = item.dwLatitude,
                        Longitude = item.dwLongitude
                    });
                }
            }

            var dmapPolygon = new PrintController().GetDMapPolygon(task.DistributionMapId);

            var bags = await db.GTUBags.Where(i => i.UserId == task.AuditorId).Select(i => i.Id).ToListAsync();
            var result = await db.GTUs.Where(i => bags.Contains(i.BagId)).Select(i => new {
                i.Id,
                i.UniqueID,
                i.ShortUniqueID,
                i.IsEnabled
            }).ToListAsync();

            return Json(result.Select(i => new {
                i.Id,
                i.UniqueID,
                i.ShortUniqueID,
                i.IsEnabled,
                IsOnline = onlineGTU.ContainsKey(i.UniqueID),
                Location = onlineGTU.ContainsKey(i.UniqueID) ? onlineGTU[i.UniqueID] : null,
                OutOfBoundary = onlineGTU.ContainsKey(i.UniqueID) ? !dmapPolygon.Contains(new Point(onlineGTU[i.UniqueID].Longitude ?? 0, onlineGTU[i.UniqueID].Latitude ?? 0)) : true,
                IsAssign = assignedGtu.ContainsKey(i.Id),
                UserColor = gtuColor.ContainsKey(i.Id) ? gtuColor[i.Id] : "",
                Company = assignedGtu.ContainsKey(i.Id) ? assignedGtu[i.Id].Company : "",
                Auditor = assignedGtu.ContainsKey(i.Id) ? assignedGtu[i.Id].Auditor : "",
                Role = assignedGtu.ContainsKey(i.Id) ? assignedGtu[i.Id].Role.ToString() : "",
                WithData = deleteButAlreadyHaveDataGTU.ContainsKey(i.Id) ? true : false
            }).OrderByDescending(i=>i.IsAssign).ThenByDescending(i=>i.IsOnline).ThenBy(i=>i.ShortUniqueID));
        }

        [Route("task/{taskId:int}/online")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGTULastLocation(int taskId)
        {
            var task = await db.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound();
            }
            var assignedGtuList = await db.TaskGtuInfoMappings
                .Where(i => i.TaskId == taskId)
                .Select(i => new
                {
                    i.GTU.Id,
                    i.GTU.UniqueID,
                    i.GTU.ShortUniqueID
                }).ToListAsync();

            var checkList = assignedGtuList.Select(i => i.UniqueID).ToArray();
            var checkTime = DateTime.Now.AddMinutes(-5);
            var onlineRecord = await db.GtuInfos
                .Where(i => i.dtReceivedTime >= checkTime && checkList.Contains(i.Code))
                .OrderByDescending(i => i.dtReceivedTime).ThenBy(i => i.Code)
                .Select(i => new { i.Code, i.dwLatitude, i.dwLongitude })
                .ToListAsync();
            Dictionary<string, ViewModel.Location> onlineGTU = new Dictionary<string, ViewModel.Location>();
            foreach (var item in onlineRecord)
            {
                if (!onlineGTU.ContainsKey(item.Code))
                {
                    onlineGTU.Add(item.Code, new ViewModel.Location
                    {
                        Latitude = item.dwLatitude,
                        Longitude = item.dwLongitude
                    });
                }
            }

            var dmapPolygon = new PrintController().GetDMapPolygon(task.DistributionMapId);

            return Json(assignedGtuList.Select(i => new
            {
                i.Id,
                IsOnline = onlineGTU.ContainsKey(i.UniqueID),
                Location = onlineGTU.ContainsKey(i.UniqueID) ? onlineGTU[i.UniqueID] : null,
                OutOfBoundary = onlineGTU.ContainsKey(i.UniqueID) ? !dmapPolygon.Contains(new Point(onlineGTU[i.UniqueID].Longitude ?? 0, onlineGTU[i.UniqueID].Latitude ?? 0)) : true
            }).ToList());

        }

        [Route("task/assign")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignGtu([FromBody] ViewGTU gtu)
        {
            var task = await db.Tasks.FindAsync(gtu.TaskId);
            if (task == null)
            {
                return NotFound();
            }
            var dbGtu = await db.GTUs.FindAsync(gtu.Id);
            var dbUser = await db.Users.Include(i=>i.Company).FirstOrDefaultAsync(i=>i.Id == gtu.AuditorId);
            var dbMapping = task.TaskGtuInfoMappings.Where(i => i.GTUId == dbGtu.Id).OrderByDescending(i=>i.Id).FirstOrDefault();
            if(dbMapping != null)
            {
                dbMapping.UserColor = gtu.UserColor;
                dbMapping.Auditor = dbUser;
            }
            else
            {
                task.TaskGtuInfoMappings.Add(new Models.TaskGtuInfoMapping
                {
                    GTU = dbGtu,
                    Auditor = dbUser,
                    Task = task,
                    UserColor = gtu.UserColor
                });
            }
            
            await db.SaveChangesAsync();
            var checkTime = DateTime.Now.AddMinutes(-5);
            var isOnline = await db.GtuInfos.Where(i => i.GtuUniqueID == dbGtu.UniqueID && i.dtReceivedTime >= checkTime).AnyAsync();
            return Json(new
            {
                success = true,
                data = new
                {
                    Id = dbGtu.Id,
                    UniqueID = dbGtu.UniqueID,
                    ShortUniqueID = dbGtu.ShortUniqueID,
                    IsEnabled = dbGtu.IsEnabled,
                    IsOnline = isOnline,
                    IsAssign = true,
                    UserColor = gtu.UserColor,
                    Company = dbUser.Company.Name,
                    Auditor = dbUser.FullName,
                    Role = dbUser.Role
                }
            });
        }

        [Route("task/{taskId:int}/unassign/{gtuId:int}")]
        [HttpDelete]
        public async Task<IHttpActionResult> UnassignGtu(int taskId, int gtuId)
        {
            var task = await db.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound();
            }
            var mapping = task.TaskGtuInfoMappings.FirstOrDefault(i => i.GTUId == gtuId);
            mapping.UserId = null;
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [Route("task/{taskId:int}/track/{gtuId:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGtuTrack(int taskId, int gtuId)
        {
            return await GetGtuTrackWithTime(taskId, gtuId, null);
        }

        [Route("task/{taskId:int}/track/{gtuId:int}/{date}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGtuTrackAfterTime(int taskId, int gtuId, [DateTimeParameter]DateTime? date)
        {
            return await GetGtuTrackWithTime(taskId, gtuId, date);
        }

        private async Task<IHttpActionResult> GetGtuTrackWithTime(int taskId, int gtuId, DateTime? lastTime)
        {
            var task = await db.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound();
            }
            var mapping = await db.TaskGtuInfoMappings.Where(i => i.TaskId == taskId && i.GTUId == gtuId).FirstOrDefaultAsync();
            if (mapping == null)
            {
                return NotFound();
            }
            var query = db.GtuInfos
                .Where(i => i.TaskgtuinfoId == mapping.Id && i.dwSpeed > 0 && (lastTime == null || i.dtReceivedTime > lastTime))
                .OrderBy(i => i.dtSendTime);

            var lastReceivedTimeQuery = query.Max(i => i.dtReceivedTime);

            var lastUpdateTime = DateTime.Now;
            if (lastReceivedTimeQuery.HasValue)
            {
                lastUpdateTime = lastReceivedTimeQuery.Value;
            }
            else if (lastTime.HasValue)
            {
                lastUpdateTime = lastTime.Value;
            }
            var result = await query.Select(i=>new {
                lat = i.dwLatitude,
                lng = i.dwLongitude
            }).ToListAsync();

            return Json(new {
                lastUpdateTime = lastUpdateTime.ToString("yyyyMMddhhmmss"),
                data = result
            });
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
