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
using Vargainc.Timm.EF;
using Vargainc.Timm.REST.ViewModel.ControlCenter;
using System.Net.Http;
using System.IO;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("task")]
    public class TaskController : ApiController
    {
        private TimmContext db = new TimmContext();

        [Route("{taskId:int}")]
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
            var taskStatus = await db.TaskTimes.Where(i => i.TaskId == taskId).OrderByDescending(i => i.Id).FirstOrDefaultAsync();
            return Json(new {
                CampaignId = dbSubMap.CampaignId,
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
                Status = taskStatus == null ? null : new Nullable<int>(taskStatus.TimeType)
            });
        }

        [Route("{taskId:int}")]
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

        [Route("{taskId:int}/finish")]
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
            return await SetTaskStop(taskId);
        }

        [Route("{taskId:int}/reopen")]
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

        [Route("{taskId:int}/import")]
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

        [Route("{taskId:int}/gtu")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGtuListByTaskId(int taskId)
        {
            var result = await db.TaskGtuInfoMappings.Where(i => i.TaskId == taskId && i.UserId != null).Select(i => new
            {
                i.GTU.Id,
                i.GTU.ShortUniqueID,
                i.UserColor
            }).ToListAsync();
            return Json(result);
        }

        [Route("{taskId:int}/dots")]
        [HttpPost]
        public async Task<IHttpActionResult> AddGtuDotsToTask(int taskId, [FromBody] List<ViewModel.CustomGTUPoint> dots)
        {
            var taskDic = await db.TaskGtuInfoMappings.Where(i => i.TaskId == taskId).Select(i => new
            {
                i.GTUId,
                TaskgtuinfoId = i.Id,

            }).ToDictionaryAsync(i => i.GTUId);

            List<Models.GtuInfo> newDots = new List<Models.GtuInfo>();
            foreach (var item in dots)
            {
                newDots.Add(new Models.GtuInfo {
                    dtReceivedTime = item.Date,
                    dtSendTime = item.Date,
                    //0-original ,1-added ,2-removed
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

        [Route("{taskId:int}/start")]
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

        [Route("{taskId:int}/pause")]
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

        [Route("{taskId:int}/stop")]
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
