using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPS.Website.DAL
{
    public class GtuDB
    {
        private static timmEntities context
        {
            get
            {
                return new timmEntities();
            }
        }
        
        public static gtu GetGtuByID(int iGtuID)
        {
            return context.gtus.Where(g => g.Id == iGtuID).FirstOrDefault();
        }

        public static gtu GetGtuByUserID(int iUserID)
        {
            DAL.taskgtuinfomapping taskGtu = context.taskgtuinfomappings
                .Where(it => it.UserId == iUserID)
                .SingleOrDefault();

            if (taskGtu == null)
                return null;
            return GetGtuByID(Convert.ToInt32(taskGtu.GTUId));
        }

        public static string GetShortGtuNo(string gtuUniqueID)
        {
            if (gtuUniqueID.Length <= 6)
                return gtuUniqueID;

            return gtuUniqueID.Substring(gtuUniqueID.Length - 6);
        }

        public static List<gtu> GetGtuListByUserID(int iUserID)
        {
            timmEntities context = new timmEntities();
            var query = (from g in context.gtus
                         join bag in context.gtubags on g.GTUBagId equals bag.Id
                         where bag.UserId == iUserID
                         select g);
            return query.ToList();
        }

        public static List<DAL.gtu> GetGtus_NoSignal(int iUserID)
        {
            List<DAL.gtu> gtuList = GetGtuListByUserID(iUserID);
            List<DAL.gtu> gtusNoSignal = new List<gtu>();

            foreach (DAL.gtu g in gtuList)
            {
                if(DAL.GtuDB.IsTransmitting(g.UniqueID) == false)
                    gtusNoSignal.Add(g);
                //int iLastGtuStatus = GetGtuCurrentStatusID(g.Id);
                //if (iLastGtuStatus == (int)GtuStatusEum.No_GTU_Signal)
                //    gtusNoSignal.Add(g);
            }

            return gtusNoSignal;
        }

        public static List<gtustatushistory> GetGtuLastStatusHistoryByUserID(int iUserID)
        {
            return context.gtustatushistories.Where(h => h.UserId == iUserID)
                .OrderByDescending(it=>it.Id).Take(100).ToList();
        }

        public static DAL.gtustatushistory GetGtuLastStatusHistory(int iGtuID)
        {
            return context.gtustatushistories.Where(h => h.GTUId == iGtuID).OrderByDescending(h => h.InsertedTime).FirstOrDefault();
        }

        public static bool IsTransmitting(string sUniqueID)
        {
            DAL.gtuinfo message = context.gtuinfoes.Where(it => it.Code == sUniqueID)
                .OrderByDescending(it => it.Id)
                .FirstOrDefault();

            if (message == null) return false;

            int iMinutes = 5;   // default to 5 minutes
            string sMinutes = ConfigUtils.GetConfiguration("MinutuesToTreatGtuAsStopped");
            if (sMinutes != "")
                iMinutes = Convert.ToInt32(sMinutes);

            if (message.dtSendTime.Value.AddMinutes(iMinutes) < DateTime.Now)
                return false;

            return true;
        }

        public static bool IsTransmitting(int iGtuId)
        {
            DAL.gtu g = GetGtuByID(iGtuId);
            if (g == null) return false;
    
            return IsTransmitting(g.UniqueID);
        }

        public static int GetGtuCurrentStatusID(int iGtuId)
        {
            DAL.gtustatushistory gtuEvent = GetGtuLastStatusHistory(iGtuId);
            if (gtuEvent == null) return 0;
            if (gtuEvent.InsertedTime.AddMinutes(5) < DateTime.Now)
                return 0;
            return gtuEvent.StatusId;
        }

        public static string GetGtuCurrentStatus(int iGtuId)
        {
            DAL.gtustatushistory gtuEvent = GetGtuLastStatusHistory(iGtuId);
            return Lookups.GtuStatus[ gtuEvent.StatusId ];
        }
        /*
        public static void SetGtuUserMapping(int iGtuID, int iUserID, int iTaskID)
        { 
            SetGtuUserMapping(iGtuID, iUserID, iTaskID, "#00FF00");
        }
        
        public static void SetGtuUserMapping(int iGtuID, int iUserID, int iTaskID, string userColor)
        {
            timmEntities context = new timmEntities();
            DAL.taskgtuinfomapping mapping = context.taskgtuinfomappings.Where(it => it.GTUId == iGtuID).FirstOrDefault();

            if (mapping == null)
            {
                mapping = new taskgtuinfomapping();
                context.AddTotaskgtuinfomappings(mapping);
            }

            mapping.TaskId = iTaskID;
            mapping.UserId = iUserID;
            mapping.UserColor = userColor;

            context.SaveChanges();
        }
        */
        public static void DisconectGtuToEmployee(int iGtuID, int iUserID)
        {
            if (iGtuID == 0 | iUserID == 0)
                return;

            timmEntities context = new timmEntities();
            DAL.taskgtuinfomapping mappingG = context.taskgtuinfomappings.Where(it => it.GTUId == iGtuID).OrderByDescending(it => it.Id).FirstOrDefault();
            if (mappingG != null)
            {
                mappingG.UserId = null;
                context.SaveChanges();
            }
        }

        public static taskgtuinfomapping GetTaskGtuInfoMapping_Last(int iGtuID)
        {
            return context.taskgtuinfomappings.Where(it => it.GTUId == iGtuID)
                .OrderByDescending(it => it.Id)
                .FirstOrDefault();
        }

        public static void AssignGtuToEmployee(int iGtuID, int iUserID, int iTaskID, string userColor)
        {
            if (iGtuID == 0 | iUserID == 0)
                throw new Exception("Please specify GTU and user.");

            timmEntities context = new timmEntities();
            // change userColor only
            taskgtuinfomapping lastMap = GetTaskGtuInfoMapping_Last(iGtuID);
            if (lastMap != null)
            {
                if (lastMap.UserId == iUserID)
                {
                    // the mapping already exist, then only change color
                    if (lastMap.UserColor != userColor)
                    {
                        lastMap.UserColor = userColor;
                        context.SaveChanges();
                    }
                    return;
                }
                else
                { 
                    if(lastMap.UserId != null && lastMap.UserId != 0)
                        throw new Exception("This gtu is in-use");
                }
            }

            // check whether the User is avaialble
            lastMap = context.taskgtuinfomappings.Where(it => it.UserId == iUserID).OrderByDescending(it => it.Id).FirstOrDefault();
            if (lastMap != null)
            {
                if (lastMap.GTUId != null && lastMap.GTUId != 0)
                {
                    string sUniqueID = DAL.GtuDB.GetGtuByID( Convert.ToInt32(lastMap.GTUId) ).ShortUniqueID;
                    string sMessage = string.Format("This user is holding GTU {0} in task #{1}", sUniqueID, lastMap.TaskId);
                    throw new Exception(sMessage);
                }
            }

            // add a new mapping
            taskgtuinfomapping newMap = new taskgtuinfomapping();
            newMap.TaskId = iTaskID;
            newMap.GTUId = iGtuID;
            newMap.UserId = iUserID;
            newMap.UserColor = userColor;

            context.AddTotaskgtuinfomappings(newMap);
            context.SaveChanges();
        }

    }
}