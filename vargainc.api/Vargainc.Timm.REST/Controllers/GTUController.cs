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
using System.Data.Common;

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
                    MappingId = i.Id,
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
            #region Query GTU Last Location
            Dictionary<string, ViewModel.Location> gtuLastLocation = new Dictionary<string, ViewModel.Location>();
            if (checkList.Length > 0)
            {
                #region SQL
                const string sql = @"
                SELECT
	                [T].[Code],
	                [Info].[dwLatitude] AS [Latitude],
	                [Info].[dwLongitude] AS [Longitude]
                FROM (
	                SELECT
		                [Code],
		                Max([dtReceivedTime]) AS [dtReceivedTime]
	                FROM
		                [gtuinfo]
                    WHERE [TaskgtuinfoId] IN ( SELECT [Id] FROM [taskgtuinfomapping] WHERE [TaskId] = @TaskId ) 
                        AND [Code] IN ({0})
	                GROUP BY
		                [Code]
                ) [T]
                INNER JOIN [gtuinfo] [Info] ON [T].Code = [Info].Code
                AND [T].[dtReceivedTime] = [Info].[dtReceivedTime]
            ";
                #endregion
                List<string> param = new List<string>(checkList.Length);
                List<DbParameter> runParam = new List<DbParameter>(checkList.Length + 1);
                for(var i = 0; i < checkList.Length; i++)
                {
                    var name = "@P" + i;
                    param.Add(name);
                    runParam.Add(new System.Data.SqlClient.SqlParameter(name, checkList[i]));
                }
                runParam.Add(new System.Data.SqlClient.SqlParameter("@TaskId", taskId));
                string runSql = string.Format(sql, string.Join(",", param));
                var gtuLastLocationList = db.Database.SqlQuery<ViewGTUInfo>(runSql, runParam.ToArray());
                foreach(var item in gtuLastLocationList)
                {
                    if (!gtuLastLocation.ContainsKey(item.Code))
                    {
                        gtuLastLocation.Add(item.Code, new ViewModel.Location {
                            Latitude = item.Latitude,
                            Longitude = item.Longitude
                        });
                    }
                }
            }
            #endregion

            var dmapPolygon = new PrintController().GetDMapPolygon(task.DistributionMapId);

            return Json(assignedGtuList.Select(i => new
            {
                i.Id,
                IsOnline = onlineGTU.ContainsKey(i.UniqueID),
                Location = gtuLastLocation.ContainsKey(i.UniqueID) ? gtuLastLocation[i.UniqueID] : null,
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
            var result = await query.Select(i => new GeoAPI.Geometries.Coordinate
            {
                Y = i.dwLatitude ?? 0,
                X = i.dwLongitude ?? 0
            }).ToArrayAsync();
            GeoAPI.Geometries.Coordinate[] simplify = null;
            if (result.Length > 3)
            {
                simplify = NetTopologySuite.Simplify.DouglasPeuckerLineSimplifier.Simplify(result, 0.0001);
            }
            else
            {
                simplify = new GeoAPI.Geometries.Coordinate[0];

            }
            

            return Json(new {
                lastUpdateTime = lastUpdateTime.ToString("yyyyMMddhhmmss"),
                data = simplify.Select(i=> new {
                    lat = i.Y,
                    lng = i.X
                })
            });
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllGTUWithStatus()
        {
            #region SQL
            const string sql = @"
                SELECT
	                [T].[Code],
                    [T].[dtReceivedTime] AS [ReceivedTime],
	                [Info].[dwLatitude] AS [Latitude],
	                [Info].[dwLongitude] AS [Longitude]
                FROM (
	                SELECT
		                Code,
		                Max([dtReceivedTime]) AS [dtReceivedTime]
	                FROM
		                [gtuinfo]
	                GROUP BY
		                Code
                ) [T]
                INNER JOIN [gtuinfo] [Info] ON [T].Code = [Info].Code
                AND [T].[dtReceivedTime] = [Info].[dtReceivedTime]
            ";
            #endregion

            var checkTime = DateTime.Now.AddMinutes(-5);
            var gtuLocationList = await db.Database.SqlQuery<ViewGTUInfo>(sql).ToListAsync();
            var gtuLocation = new Dictionary<string, KeyValuePair<bool, ViewModel.Location>>();
            foreach(var item in gtuLocationList)
            {
                if (!gtuLocation.ContainsKey(item.Code))
                {
                    var isOnline = item.ReceivedTime.HasValue && item.ReceivedTime.Value > checkTime;
                    var latlng = new ViewModel.Location
                    {
                        Latitude = item.Latitude,
                        Longitude = item.Longitude
                    };
                    gtuLocation.Add(item.Code, new KeyValuePair<bool, ViewModel.Location>(isOnline, latlng));
                }
            }
            var assignedGTUList = await db.TaskGtuInfoMappings
                .Where(i => i.UserId > 0)
                .Select(i => new
                {
                    i.GTUId,
                    i.Task.Name,
                    Role = i.Auditor.Role,
                    Company = i.Auditor.Company.Name,
                    Auditor = i.Auditor.FullName,
                    TaskName = i.Task.Name
                }).ToListAsync();
            var assignedGTU = new Dictionary<int?, ViewGTU>();
            foreach(var item in assignedGTUList)
            {
                if (!assignedGTU.ContainsKey(item.GTUId))
                {
                    assignedGTU.Add(item.GTUId, new ViewGTU {
                        Company = item.Company,
                        Auditor = item.Auditor,
                        Role = item.Role,
                        UserColor = item.TaskName
                    });
                }
            }

            var gtuInfo = await db.GTUs
                .Select(i => new
                {
                    i.Id,
                    i.UniqueID,
                    i.ShortUniqueID
                })
                .OrderByDescending(i=>i.Id)
                .ToListAsync();
            return Json(gtuInfo.Select(i => new
            {
                Id = i.Id,
                Code = i.ShortUniqueID,
                FullCode = i.UniqueID,
                IsAssign = assignedGTU.ContainsKey(i.Id),
                TaskName = assignedGTU.ContainsKey(i.Id) ? assignedGTU[i.Id].UserColor : null,
                Company = assignedGTU.ContainsKey(i.Id) ? assignedGTU[i.Id].Company : null,
                Auditor = assignedGTU.ContainsKey(i.Id) ? assignedGTU[i.Id].Auditor : null,
                Role = assignedGTU.ContainsKey(i.Id) ? assignedGTU[i.Id].Role : null,
                HaveLocation = gtuLocation.ContainsKey(i.UniqueID),
                IsOnline = gtuLocation.ContainsKey(i.UniqueID) ? gtuLocation[i.UniqueID].Key : false,
                Latitude = gtuLocation.ContainsKey(i.UniqueID) ? gtuLocation[i.UniqueID].Value.Latitude : null,
                Longitude = gtuLocation.ContainsKey(i.UniqueID) ? gtuLocation[i.UniqueID].Value.Longitude : null
            }).ToList()
            );
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
