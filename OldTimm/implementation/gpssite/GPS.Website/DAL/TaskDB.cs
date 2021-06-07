using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPS.Website.DAL
{
    public class TaskDB
    {
        private static timmEntities context
        {
            get
            {
                return new timmEntities();
            }
        }

        public static DAL.task GetTaskByID(int iTaskID)
        {
            return context.tasks.Where(it => it.Id == iTaskID).SingleOrDefault();
        }

        public static tasktime GetTaskTimeByTaskID(int iTaskID)
        {
            timmEntities timm = new timmEntities();
            tasktime ttime = (from tt in timm.tasktimes
                                  where tt.TaskId == iTaskID
                                  orderby tt.Id descending
                                  select tt).FirstOrDefault();
            return ttime;
        }

        public static void StartTask(int iTaskID)
        {
            SetTaskStatus(iTaskID, 0);
        }

        public static void PauseTask(int iTaskID)
        {
            SetTaskStatus(iTaskID, 2);
        }

        public static void StopTask(int iTaskID)
        {
            SetTaskStatus(iTaskID, 1);
        }

        private static void SetTaskStatus(int iTaskID, int iStatus)
        {
            tasktime ttime = new tasktime();
            ttime.Time = DateTime.Now;
            ttime.TimeType = iStatus;
            ttime.TaskId = iTaskID;

            // the the last one
            using (System.Transactions.TransactionScope trans = new System.Transactions.TransactionScope())
            {
                timmEntities context = new timmEntities();
                context.AddTotasktimes(ttime);

                if (iStatus == 1)
                {
                    List<ViewGtuInTask> taskGtuList = context.ViewGtuInTasks.Where(it => it.TaskId == iTaskID).ToList();
                    foreach (ViewGtuInTask taskGtu in taskGtuList)
                    {
                        DAL.taskgtuinfomapping mappingG = context.taskgtuinfomappings.Where(it => it.GTUId == taskGtu.GtuID).OrderByDescending(it => it.Id).FirstOrDefault();
                        if (mappingG != null)
                            mappingG.UserId = null;
                    }
                }

                context.SaveChanges();
                trans.Complete();
            }
        }

        public static string GetCampaignDisplayByTaskID(int iTaskID)
        {
            DAL.task oTask = context.tasks.Where(it=>it.Id == iTaskID).FirstOrDefault();
            DAL.campaign camp = oTask.distributionmap.submap.campaign;

            string userCode = camp.UserName;
            DAL.user oUser = context.users.Where(it => it.UserName == camp.UserName).FirstOrDefault();
            if (oUser != null) userCode = oUser.UserCode;

            string sDisplayName = string.Format("{0:MMddyy}-{1}-{2}-{3}-{4}", camp.Date, camp.ClientCode, camp.AreaDescription, userCode, camp.Sequence);
            return sDisplayName;
        }
    }
}