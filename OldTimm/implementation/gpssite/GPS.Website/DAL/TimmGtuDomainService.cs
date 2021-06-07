
namespace GPS.Website.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using System.Data.Common;
    using System.Data.SqlClient;

    // Implements application logic using the timmEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class TimmGtuDomainService : LinqToEntitiesDomainService<timmEntities>
    {

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'gtus' query.
        public IQueryable<gtu> GetGtus()
        {
            return this.ObjectContext.gtus;
        }

        public List<ViewGtuInTask> GetGtusInTask(int iTaskID)
        {
            var query = (from g in this.ObjectContext.ViewGtuInTasks
                         where g.TaskId == iTaskID
                         orderby g.GtuID
                         select g);
            List<ViewGtuInTask> gtus = query.ToList();
            return gtus;
        }

        public distributionmap GetDistributionMapByTaskID(int iTaskID)
        {
            var dmap = (from t in this.ObjectContext.tasks
                        join d in this.ObjectContext.distributionmaps on t.DmId equals d.Id
                        where t.Id == iTaskID
                        select d).SingleOrDefault();
            return dmap;
        }

        public List<distributionmapcoordinate> GetDistributionmapCoordinatesByTaskID(int iTaskID)
        {
            task t = this.ObjectContext.tasks.Where(it=> it.Id == iTaskID).SingleOrDefault();
            if (t == null) return null;

            return GetDistributionmapCoordinatesByDMapID(t.DmId);
        }

        public List<distributionmapcoordinate> GetDistributionmapCoordinatesByDMapID(int iDistributionMapID)
        {
            return (from c in this.ObjectContext.distributionmapcoordinates
                    where c.DistributionMapId == iDistributionMapID
                    select c).ToList();
        }

        public List<DAL.monitoraddress> GetMonitorAddressListByTaskID(int iTaskID)
        {
            try
            {
                DAL.task oTask = this.ObjectContext.tasks.Where(it => it.Id == iTaskID).FirstOrDefault();
                return GetMonitorAddressListByDMapID(oTask.DmId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<monitoraddress> GetMonitorAddressListByDMapID(int iDMapID)
        {
            return MonitorAddressDB.GetMonitorAddressListByDMapID(iDMapID);
        }

        public List<ViewGtuLocation> BatchGetGtuHistoryByGtuID(params int[] gtuID)
        {
            return PagedBatchGetGtuHistoryByGtuID(0, 10000, gtuID);
        }

        public int GetGtuHistoryPageSizeByGtuID(params int[] gtuID)
        {
            try
            {
                string sql = @"
                SELECT  COUNT(*) AS [Count]
                FROM    [dbo].[taskgtuinfomapping] AS [a]
                        INNER JOIN [dbo].[gtuinfo] AS [b] ON [a].Id = [b].TaskgtuinfoId
                WHERE   [a].[GTUId] IN ( {0} )
            ";

                ObjectContext.Connection.Open();
                sql = string.Format(sql, string.Join(",", gtuID));
                var result = ObjectContext.ExecuteStoreQuery<int>(sql).First();
                return result;
            }
            finally
            {
                ObjectContext.Connection.Close();
            }
        }

        public List<ViewGtuLocation> PagedBatchGetGtuHistoryByGtuID(int pageIndex, int pageSize, params int[] gtuID)
        {
            List<ViewGtuLocation> result = new List<ViewGtuLocation>();
            try
            {
                #region sql
//                string sql = @"
//                    SELECT  [b].[id] ,
//                            [b].[dwLatitude] ,
//                            [b].[dwLongitude] ,
//                            [a].[GTUId]
//                    FROM    [dbo].[taskgtuinfomapping] AS [a]
//                            INNER JOIN [dbo].[gtuinfo] AS [b] ON [a].Id = [b].TaskgtuinfoId
//                    WHERE   [a].[GTUId] IN ({0})
//                    ORDER BY [b].[dtSendTime] DESC
//                ";
                string sql = @"
                    SELECT  [Id] ,
                            [dwLatitude] ,
                            [dwLongitude] ,
                            [GTUId]
                    FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY [b].[dtSendTime] DESC ) AS [Serial] ,
                                        [b].[Id] ,
                                        [b].[dwLatitude] ,
                                        [b].[dwLongitude] ,
                                        [a].[GTUId]
                              FROM      [dbo].[taskgtuinfomapping] AS [a]
                                        INNER JOIN [dbo].[gtuinfo] AS [b] ON [a].Id = [b].TaskgtuinfoId
                              WHERE     [a].[GTUId] IN ({0})
                            ) AS [T]
                    WHERE   [T].[Serial] > @RowLower
                            AND [T].[Serial] <= @RowUpper
                    ";
                #endregion
                ObjectContext.Connection.Open();
                sql = string.Format(sql, string.Join(",", gtuID));
                int rowLower = pageIndex * pageSize;
                int rowUpper = rowLower + pageSize;
                SqlParameter [] param = new SqlParameter []
                { 
                    new SqlParameter("@RowLower", rowLower), 
                    new SqlParameter("@RowUpper", rowUpper)
                };
                return ObjectContext.ExecuteStoreQuery<ViewGtuLocation>(sql, param).ToList();
            }
            finally
            {
                ObjectContext.Connection.Close();
            }
        }

        public List<ViewGtuLocation> GetGtuHistoryByGtuID(int iGtuID)
        {
            taskgtuinfomapping m = GetTaskGtuInfoByGtuID(iGtuID);
            if (m == null)
                return null;

            return GetGtuHistory(m.Id);
        }

        public List<gtuinfo> GetNewGtuInfo(int iGtuID, Int64 iGtuInfoIdFrom)
        {
            taskgtuinfomapping m = GetTaskGtuInfoByGtuID(iGtuID);
            List<gtuinfo> gtuInfoList = (from i in this.ObjectContext.gtuinfoes
                    where i.TaskgtuinfoId == m.Id && i.Id > iGtuInfoIdFrom
                    orderby i.Id
                    select i).ToList();

            return gtuInfoList;
        }

        public taskgtuinfomapping GetTaskGtuInfoByGtuID(int iGtuID)
        {
            taskgtuinfomapping map = (from m in this.ObjectContext.taskgtuinfomappings
                                      where m.GTUId == iGtuID
                                      orderby m.Id descending
                                      select m).FirstOrDefault();
            return map;
        }



        public List<ViewGtuLocation> GetGtuHistory(int iTaskGtuInfoID)
        {
            List<gtuinfo> gtuinfoList = (from i in this.ObjectContext.gtuinfoes
                                         where i.TaskgtuinfoId == iTaskGtuInfoID
                                         orderby i.dtSendTime descending
                                         select i).ToList();

            List<ViewGtuLocation> gtuLocations = new List<ViewGtuLocation>();
            foreach (gtuinfo i in gtuinfoList)
            {
                ViewGtuLocation point = new ViewGtuLocation();
                point.Id = i.Id;
                point.dwLatitude = i.dwLatitude;
                point.dwLongitude = i.dwLongitude;
                gtuLocations.Add(point);
            }

            return gtuLocations;
        }

        public ViewGtuLocation GetGtuInfoByGtuID(int iGtuID)
        {
            taskgtuinfomapping map = GetTaskGtuInfoByGtuID(iGtuID);
            if (map == null) return null;
            return GetGtuInfo(map.Id);
        }

        public ViewGtuLocation GetGtuInfo(int iTaskGtuInfoID)
        {
            gtuinfo info = (from i in this.ObjectContext.gtuinfoes
                            where i.TaskgtuinfoId == iTaskGtuInfoID
                            orderby i.Id descending
                            select i).FirstOrDefault();
            return NewGtuLocation(info);
        }
        
        private ViewGtuLocation NewGtuLocation(gtuinfo oGtuInfo)
        {
            if (oGtuInfo == null) return null;

            ViewGtuLocation point = new ViewGtuLocation();
            point.Id = oGtuInfo.Id;
            point.dwLatitude = oGtuInfo.dwLatitude;
            point.dwLongitude = oGtuInfo.dwLongitude;
            return point;
        }


        [Invoke]
        public List<int> GetNdAddressByTask(int iTaskId)
        {
            List<int> idList = new List<int>();
            try
            {
                DAL.task t = this.ObjectContext.tasks.Where(it => it.Id == iTaskId).FirstOrDefault();
                DAL.distributionmap dmap = this.ObjectContext.distributionmaps.Where(it => it.Id == t.DmId).FirstOrDefault();
                double maxLatitude = this.ObjectContext.distributionmapcoordinates.Max(it => it.Latitude);
                ViewDistributionRange dmapRange = this.ObjectContext.ViewDistributionRanges.Where(it => it.DistributionMapId == dmap.Id).FirstOrDefault();

                List<DAL.ViewNdAddressRange> ndList = this.ObjectContext.ViewNdAddressRanges.Where(it => it.MinLatitude <= dmapRange.MaxLatitude
                    && it.MaxLatitude >= dmapRange.MinLatitude
                    && it.MinLongitude <= dmapRange.MaxLongitude
                    && it.MaxLongitude >= dmapRange.MinLongitude).ToList();

                foreach (DAL.ViewNdAddressRange nd in ndList)
                    idList.Add(nd.NdAddressId);
                return idList;
            }
            catch (Exception ex)
            {
                GPS.Utilities.DBLog.LogError(ex.ToString());
                return idList;
            }
        }

        public List<ndaddresscoordinate> GetNdAddressCoordinates(int ndAddressID)
        {
            return this.ObjectContext.ndaddresscoordinates
                .Where(it => it.NdAddressId == ndAddressID)
                .ToList();
        }

        [Invoke]
        public List<int> GetCustomAreaByTask(int iTaskId)
        {
            List<int> idList = new List<int>();
            try
            {
                DAL.task t = this.ObjectContext.tasks.Where(it => it.Id == iTaskId).FirstOrDefault();
                DAL.distributionmap dmap = this.ObjectContext.distributionmaps.Where(it => it.Id == t.DmId).FirstOrDefault();
                double maxLatitude = this.ObjectContext.distributionmapcoordinates.Max(it => it.Latitude);
                ViewDistributionRange dmapRange = this.ObjectContext.ViewDistributionRanges.Where(it => it.DistributionMapId == dmap.Id).FirstOrDefault();

                List<DAL.ViewCustomAreaRange> ndList = this.ObjectContext.ViewCustomAreaRanges
                    .Where(it => it.MinLatitude <= dmapRange.MaxLatitude
                    && it.MaxLatitude >= dmapRange.MinLatitude
                    && it.MinLongitude <= dmapRange.MaxLongitude
                    && it.MaxLongitude >= dmapRange.MinLongitude).ToList();

                foreach (DAL.ViewCustomAreaRange addr in ndList)
                    idList.Add(addr.CustomAreaId);
                return idList;
            }
            catch (Exception ex)
            {
                GPS.Utilities.DBLog.LogError(ex.ToString());
                return idList;
            }
        }

        public List<DAL.customareacoordinate> GetCustomAreaCoordinates(int customAreaID)
        {
            return this.ObjectContext.customareacoordinates
                .Where(it => it.CustomAreaId == customAreaID)
                .ToList();
        }

    }
}


