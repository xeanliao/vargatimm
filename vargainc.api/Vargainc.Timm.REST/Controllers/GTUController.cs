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
            var assignedGtuList = await db.TaskGtuInfoMappings.Where(i => i.TaskId == taskId && i.UserId != null).Select(i => new ViewGTU
            {
                Id = i.GTUId,
                UserColor = i.UserColor,
                Company = i.Auditor.Company.Name,
                Auditor = i.Auditor.FullName,
                Role = i.Auditor.Role
            }).ToListAsync();

            Dictionary<int?, ViewGTU> assignedGtu = new Dictionary<int?, ViewGTU>();
            foreach(var item in assignedGtuList)
            {
                if (!assignedGtu.ContainsKey(item.Id))
                {
                    assignedGtu.Add(item.Id, item);
                }
            }
         
            var checkTime = DateTime.Now.AddMinutes(-5);
            var onlineGtuQuery = db.GtuInfos
                .Where(i => i.dtReceivedTime >= checkTime)
                .GroupBy(i => i.GtuUniqueID)
                .Select(i => i.Key);
            //.ToDictionaryAsync(i=>i);
            //var test = onlineGtuQuery.ToString();
            //var onlineGtu = await onlineGtuQuery.ToDictionaryAsync(i => i);
            var onlineGtu = new HashSet<string>();
            foreach(var item in onlineGtuQuery)
            {
                if (!onlineGtu.Contains(item))
                {
                    onlineGtu.Add(item);
                }
            }
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
                IsOnline = onlineGtu.Contains(i.UniqueID),
                IsAssign = assignedGtu.ContainsKey(i.Id),
                UserColor = assignedGtu.ContainsKey(i.Id) ? assignedGtu[i.Id].UserColor : "",
                Company = assignedGtu.ContainsKey(i.Id) ? assignedGtu[i.Id].Company : "",
                Auditor = assignedGtu.ContainsKey(i.Id) ? assignedGtu[i.Id].Auditor : "",
                Role = assignedGtu.ContainsKey(i.Id) ? assignedGtu[i.Id].Role.ToString() : ""
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
                .Where(i => i.TaskId == taskId && i.UserId != null)
                .Select(i => new
                {
                    i.GTU.Id,
                    i.GTU.UniqueID,
                    i.GTU.ShortUniqueID
                }).ToListAsync();

            var checkList = assignedGtuList.Select(i => i.UniqueID).ToArray();
            var checkTime = DateTime.Now.AddMinutes(-5);
            var onlineRecord = await db.GtuInfos
                .Where(i => i.dtReceivedTime >= checkTime && checkList.Contains(i.GtuUniqueID))
                .OrderByDescending(i => i.dtReceivedTime).ThenBy(i => i.GtuUniqueID)
                .Select(i => new { i.GtuUniqueID, i.dwLatitude, i.dwLongitude })
                .ToListAsync();
            Dictionary<string, ViewModel.Location> onlineGTU = new Dictionary<string, ViewModel.Location>();
            foreach (var item in onlineRecord)
            {
                if (!onlineGTU.ContainsKey(item.GtuUniqueID))
                {
                    onlineGTU.Add(item.GtuUniqueID, new ViewModel.Location
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
                OutOfBoundary = onlineGTU.ContainsKey(i.UniqueID) ? dmapPolygon.Contains(new Point(onlineGTU[i.UniqueID].Longitude ?? 0, onlineGTU[i.UniqueID].Latitude ?? 0)) : true
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
            var dbMapping = task.TaskGtuInfoMappings.FirstOrDefault(i => i.GTUId == dbGtu.Id && i.TaskId == task.Id);
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
