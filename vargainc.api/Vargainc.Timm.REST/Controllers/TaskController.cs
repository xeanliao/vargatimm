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
using System.Net.Http;
using System.IO;
using EntityFramework.Extensions;

using NetTopologySuite.Geometries;

using Vargainc.Timm.EF;
using Vargainc.Timm.REST.ViewModel.ControlCenter;
using Vargainc.Timm.Extentions;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("task")]
    [Helper.PublicAccessFilter]
    public class TaskController : ApiController
    {
        private TimmContext db = new TimmContext();

        [Route("{taskId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetById(int taskId)
        {
            var dbTask = await db.Tasks.FindAsync(taskId);
            if(dbTask == null)
            {
                return NotFound();
            }
            var dbDMap = await db.DistributionMaps.FindAsync(dbTask.DistributionMapId);
            var dbSubMap = await db.SubMaps.FindAsync(dbDMap.SubMapId);
            var dbCampaign = await db.Campaigns.FindAsync(dbSubMap.CampaignId);
            var taskStatus = await db.TaskTimes.Where(i => i.TaskId == taskId).OrderByDescending(i => i.Id).FirstOrDefaultAsync();
            
            return Json(new {
                CampaignId = dbSubMap.CampaignId,
                ClientName = dbCampaign.ClientName,
                ClientCode = dbCampaign.ClientCode,
                SubMapId = dbSubMap.Id,
                DMapId = dbDMap.Id,
                dbTask.Id,
                dbTask.Name,
                dbTask.Date,
                dbTask.DistributionMapId,
                AuditorName = dbTask.Auditor.FullName,
                AuditorId = dbTask.Auditor.Id,
                dbTask.Email,
                dbTask.Telephone,
                HaveGtuInfoMapping = dbTask.TaskGtuInfoMappings.Count() > 0 ? true : false,
                Status = taskStatus == null ? null : taskStatus.TimeType,
                PublicUrl = taskId.ToString().DesEncrypt()
            });
        }

        [Route("{taskId}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTask(int taskId, [FromBody] TaskViewModel taskInfo)
        {
            var dbTask = await db.Tasks.FindAsync(taskId);
            if(dbTask == null)
            {
                return NotFound();
            }
            dbTask.Date = taskInfo.Date ?? DateTime.Now;
            dbTask.Email = taskInfo.Email;
            dbTask.Telephone = string.Format("{0}{1}", taskInfo.Telephone, taskInfo.Operator);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [Route("{taskId}/finish")]
        [HttpPut]
        public async Task<IHttpActionResult> MarkFinished(int taskId)
        {
            var dbTask = await db.Tasks.FindAsync(taskId);
            if(dbTask == null)
            {
                return NotFound();
            }

            //check tasktime have end
            var lastTime = dbTask.TaskTimes.OrderByDescending(i => i.Time).FirstOrDefault();
            if(lastTime == null || lastTime.TimeType != 1)
            {
                return Json(new { success = false, error = "You could not mark finish for this task because you have not stop monitor(s) yet!" });
            }
            await SetTaskStop(taskId);
            dbTask.Status = 1;
            await db.SaveChangesAsync();
            return Json(new { success = true, status = 1 });
        }

        [Route("{taskId}/reopen")]
        [HttpPut]
        public async Task<IHttpActionResult> ReOpen(int taskId)
        {
            var dbTask = await db.Tasks.FindAsync(taskId);
            if (dbTask == null)
            {
                return NotFound();
            }
            dbTask.Status = 0;
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [Route("{taskId}/import")]
        [HttpPut]
        public async Task<IHttpActionResult> ImportGtuInfo(int taskId)
        {
            var dbTask = await db.Tasks.FindAsync(taskId);
            if (dbTask == null)
            {
                return NotFound();
            }

            if (!Request.Content.IsMimeMultipartContent())
            {
                return NotFound();
            }


            var mapping = new SortedList<string, int>();
            foreach(var item in dbTask.TaskGtuInfoMappings.Select(i => new { i.GTU.UniqueID, i.Id }))
            {
                mapping.Add(item.UniqueID.Substring(item.UniqueID.Length - 6), item.Id.Value);
            }

            // Extract data from request
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            var fileContent = provider.Contents.FirstOrDefault();
            using (var uploadFileStream = await fileContent.ReadAsStreamAsync())
            using (var reader = new StreamReader(uploadFileStream))
            {
                var error = new List<string>();
                int lineNumber = 0;
                var importedGtuInfo = new List<Models.GtuInfo>();
                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    var content = await reader.ReadLineAsync();
                    char? spliter = null;
                    if (content.IndexOf(',') > -1)
                    {
                        spliter = ',';
                    }
                    else if (content.IndexOf('\t') > -1)
                    {
                        spliter = '\t';
                    }

                    if (!spliter.HasValue)
                    {
                        error.Add(string.Format("line {0} parse failed. Please seperate column with ',' or TAB skip", lineNumber));
                        continue;
                    }

                    var data = content.Split(spliter.Value);

                    if(data.Length < 4)
                    {
                        error.Add(string.Format("line {0} parse failed. no enough data. skip", lineNumber));
                        continue;
                    }

                    string gtuNumber = data[0];
                    if (mapping.ContainsKey(gtuNumber) == false)
                    {
                        error.Add(string.Format("line {0} parse failed. gtu number: {1} do not exist in gtuinfomapping. skip", lineNumber, gtuNumber));
                        continue;
                    }

                    var gtuInfo = db.GtuInfos.Create();
                    gtuInfo.TaskgtuinfoId = mapping[gtuNumber];
                    gtuInfo.dwLongitude = Convert.ToDouble(data[1]);
                    gtuInfo.dwLatitude = Convert.ToDouble(data[2]);
                    gtuInfo.dtSendTime = Convert.ToDateTime(data[3]);

                    importedGtuInfo.Add(gtuInfo);
                }

                if(importedGtuInfo.Count > 0)
                {
                    db.GtuInfos.AddRange(importedGtuInfo);
                    await db.SaveChangesAsync();
                }

                
                return Json(new { success = true, error = error});
            }



            //try
            //{
            //    StringReader reader = new StringReader(sFileContent);
            //    List<string> GtuInfoLines = new List<string>();
            //    string sLine = "";
            //    while ((sLine = reader.ReadLine()) != null)
            //        GtuInfoLines.Add(sLine);

            //    if (GtuInfoLines.Count == 0)
            //        return 0;

            //    char seperator;
            //    sLine = GtuInfoLines[0];
            //    if (sLine.IndexOf("\t") > 0)
            //        seperator = '\t';
            //    else if (sLine.IndexOf(",") > 0)
            //        seperator = ',';
            //    else
            //        throw new Exception("Please seperate column with ',' or TAB");

            //    List<Models.GtuInfo> gtuInfoList = new List<Models.GtuInfo>();

            //            for (int i = 0; i < GtuInfoLines.Count; i++)
            //            {
            //                string[] gtuInfoData = GtuInfoLines[i].Split(seperator);
            //                if (gtuInfoData.Length < 4)
            //                    continue;

            //                string gtuNumber = gtuInfoData[0];
            //                if (taskGtuMap.ContainsKey(gtuNumber) == false)
            //                    continue;


            //                oGtuInfo.TaskgtuinfoId = taskGtuMap[gtuNumber];
            //                oGtuInfo.dwLongitude = Convert.ToDouble(gtuInfoData[1]);
            //                oGtuInfo.dwLatitude = Convert.ToDouble(gtuInfoData[2]);
            //                oGtuInfo.dtSendTime = Convert.ToDateTime(gtuInfoData[3]);

            //                gtuInfoList.Add(oGtuInfo);
            //            }

            //            GtuInfoRepository gtuRep = new GtuInfoRepository();
            //            gtuRep.AddGtuInfo(gtuInfoList);

            //            return gtuInfoList.Count;
            //        }
            //    }
        }

        [Route("{taskId}/gtu")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGtuListByTaskId(int taskId)
        {
            var result = await db.TaskGtuInfoMappings.Where(i => i.TaskId == taskId).Select(i => new
            {
                i.GTU.Id,
                i.GTU.ShortUniqueID,
                i.UserColor
            }).ToListAsync();
            return Json(result);
        }

        class GtuIdTaskGtuInfoId
        {
            public int? GTUId { get; set; }
            public int? TaskgtuinfoId { get; set; }
        }

        [Route("{taskId}/dots")]
        [HttpPost]
        public async Task<IHttpActionResult> AddGtuDotsToTask(int taskId, [FromBody] List<ViewModel.CustomGTUPoint> dots)
        {
            #region Temp Fix
            var taskList = await db.TaskGtuInfoMappings.Where(i => i.TaskId == taskId).Select(i => new GtuIdTaskGtuInfoId
            {
                GTUId = i.GTUId,
                TaskgtuinfoId = i.Id,

            }).ToListAsync();
            var taskDic = new Dictionary<int?, GtuIdTaskGtuInfoId>();
            foreach(var item in taskList)
            {
                if (!taskDic.ContainsKey(item.GTUId))
                {
                    taskDic.Add(item.GTUId, item);
                }
            }
            #endregion

            List<Models.GtuInfo> newDots = new List<Models.GtuInfo>();
            foreach (var item in dots)
            {
                newDots.Add(new Models.GtuInfo {
                    dtReceivedTime = item.Date,
                    dtSendTime = item.Date,
                    //0-original ,1-added ,2-removed, 3-merged
                    nCellID = 1,
                    dwLatitude = item.Location.Latitude,
                    dwLongitude = item.Location.Longitude,
                    TaskgtuinfoId = taskDic[item.GtuId].TaskgtuinfoId,
                    dwSpeed = 0,
                    nHeading = 0,
                    sIPAddress = string.Empty,
                    nAreaCode = 0,
                    nNetworkCode = 0,
                    nGPSFix = 0,
                    nAccuracy = 0,
                    nCount = 0,
                    nLocationID = 0,
                    sVersion = string.Empty,
                    dwAltitude = 0,
                    PowerInfo = 0,
                    Code = string.Empty,
                    Status = 0,
                    Distance = 0
                });
            }
            db.GtuInfos.AddRange(newDots);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [Route("{taskId}/dots")]
        [HttpPut]
        public async Task<IHttpActionResult> RemoveGtuDotsToTask(int taskId, [FromBody] List<long?> dots)
        {
            var task = await db.Tasks.FindAsync(taskId);
            if(task == null)
            {
                return NotFound();
            }

            await db.GtuInfos.Where(i => dots.Contains(i.Id))
                .UpdateAsync(t=> new Models.GtuInfo { nCellID = 2 });
            
            return Json(new { success = true });
        }

        [Route("{taskId}/start")]
        [HttpPut]
        public async Task<IHttpActionResult> SetTaskStart(int taskId)
        {
            var dbTask = await db.Tasks.FindAsync(taskId);
            if(dbTask == null)
            {
                return NotFound();
            }
            db.TaskTimes.Add(new Models.TaskTime {
                Task = dbTask,
                Time = DateTime.Now,
                TimeType = 0
            });
            await db.SaveChangesAsync();
            return Json(new { success = true, status = 0 });
        }

        [Route("{taskId}/pause")]
        [HttpPut]
        public async Task<IHttpActionResult> SetTaskPause(int taskId)
        {
            var dbTask = await db.Tasks.FindAsync(taskId);
            if (dbTask == null)
            {
                return NotFound();
            }
            db.TaskTimes.Add(new Models.TaskTime
            {
                Task = dbTask,
                Time = DateTime.Now,
                TimeType = 2
            });
            await db.SaveChangesAsync();
            return Json(new { success = true, status = 2 });
        }

        [Route("{taskId}/stop")]
        [HttpPut]
        public async Task<IHttpActionResult> SetTaskStop(int taskId)
        {
            var dbTask = await db.Tasks.FindAsync(taskId);
            if (dbTask == null)
            {
                return NotFound();
            }
            db.TaskTimes.Add(new Models.TaskTime
            {
                Task = dbTask,
                Time = DateTime.Now,
                TimeType = 1
            });

            var assignedGtuList = await db.TaskGtuInfoMappings
                .Where(i => i.TaskId == taskId && i.UserId != null)
                .ToListAsync();

            foreach(var item in assignedGtuList)
            {
                item.UserId = null;
            }

            await db.SaveChangesAsync();
            return Json(new { success = true, status = 1 });
        }

        [Route("merge/{originalTaskId}/to/{targetTaskId}")]
        [HttpGet]
        public async Task<IHttpActionResult> Merge(int originalTaskId, int targetTaskId)
        {
            var originalTask = await db.Tasks.FindAsync(originalTaskId);
            var targetTask = await db.Tasks.FindAsync(targetTaskId);
            if(originalTask == null || targetTask == null)
            {
                return Json(new { success = false, error = "task is not exists!" });
            }

            var originalTaskStatus = originalTask.TaskTimes.OrderByDescending(i => i.Id).FirstOrDefault();
            var targetTaskStatus = targetTask.TaskTimes.OrderByDescending(i => i.Id).FirstOrDefault();

            if(originalTaskStatus == null || targetTaskStatus == null || originalTaskStatus.TimeType != 1 || targetTaskStatus.TimeType != 1)
            {
                return Json(new { success = false, error = "you must stop task first!" });
            }

            var targetDMapLocations = db.DistributionMaps
                .FirstOrDefault(i => i.Id == targetTask.DistributionMapId)
                .DistributionMapCoordinates
                .OrderBy(i => i.Id)
                .Select(i => new GeoAPI.Geometries.Coordinate
                {
                    X = i.Longitude.Value,
                    Y = i.Latitude.Value
                }).ToList();
            //close polygon
            targetDMapLocations.Add(targetDMapLocations[0]);
            var dmapLineRing = new LinearRing(targetDMapLocations.ToArray());
            var dmapPolygon = new Polygon(dmapLineRing);
            dmapPolygon.Buffer(0);

            
            var needMergeGtu = new List<Models.GtuInfo>();
            var targetMapping = targetTask.TaskGtuInfoMappings.ToList();
            Dictionary<string, int> mapping = new Dictionary<string, int>();
            foreach(var item in targetMapping)
            {
                if (!mapping.ContainsKey(item.GTU.UniqueID))
                {
                    mapping.Add(item.GTU.UniqueID, item.Id.Value);
                }
            }
            var originalGtus = originalTask.TaskGtuInfoMappings.SelectMany(i => i.GtuInfos).OrderBy(i => i.GtuUniqueID).ToList();
            foreach (var gtu in originalGtus)
            {
                var point = new Point(gtu.dwLongitude ?? 0, gtu.dwLatitude ?? 0);
                if (dmapPolygon.Contains(point))
                {
                    int? taskGtuInfoId = null;
                    if (mapping.ContainsKey(gtu.Code))
                    {
                        taskGtuInfoId = mapping[gtu.Code];
                    }
                    else
                    {
                        var newMapping = db.TaskGtuInfoMappings.Create();
                        newMapping.GTUId = gtu.TaskGtuInfoMapping.GTUId;
                        newMapping.TaskId = targetTask.Id;
                        newMapping.UserColor = gtu.TaskGtuInfoMapping.UserColor;
                        db.TaskGtuInfoMappings.Add(newMapping);
                        await db.SaveChangesAsync();
                        taskGtuInfoId = newMapping.Id;
                        mapping.Add(gtu.Code, newMapping.Id.Value);
                    }
                    needMergeGtu.Add(new Models.GtuInfo
                    {
                        TaskgtuinfoId = taskGtuInfoId,
                        dtReceivedTime = gtu.dtReceivedTime,
                        dtSendTime = gtu.dtSendTime,
                        //0-original ,1-added ,2-removed, 3-merged
                        nCellID = 3,
                        dwLatitude = gtu.dwLatitude,
                        dwLongitude = gtu.dwLongitude,
                        dwSpeed = gtu.dwSpeed,
                        nHeading = gtu.nHeading,
                        sIPAddress = gtu.sIPAddress,
                        nAreaCode = gtu.nAreaCode,
                        nNetworkCode = gtu.nNetworkCode,
                        nGPSFix = gtu.nGPSFix,
                        nAccuracy = gtu.nAccuracy,
                        nCount = gtu.nCount,
                        nLocationID = gtu.nLocationID,
                        sVersion = gtu.sVersion,
                        dwAltitude = gtu.dwAltitude,
                        PowerInfo = gtu.PowerInfo,
                        Code = gtu.Code,
                        Status = gtu.Status,
                        Distance = gtu.Distance,
                        GtuUniqueID = gtu.GtuUniqueID
                    });
                }
            }
            db.GtuInfos.AddRange(needMergeGtu);
            await db.SaveChangesAsync();
            return Json(new
            {
                success = true
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
