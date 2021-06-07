
namespace GPS.Website.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Data.Objects.DataClasses;
    using System.Linq;

    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // Implements application logic using the timmEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class TimmDomainService : LinqToEntitiesDomainService<timmEntities>
    {

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'distributionmaps' query.
        [Query(IsDefault = true)]
        public IQueryable<distributionmap> GetDistributionmaps()
        {
            return this.ObjectContext.distributionmaps;
        }

        public distributionmap GetDistributionMapByDMapID(int iDistributionMapID)
        { 
            return (from d in this.ObjectContext.distributionmaps
                        where d.Id == iDistributionMapID
                        select d).FirstOrDefault();
        }

        public distributionmap GetDistributionMapByTaskID(int iTaskID)
        {
            var dmap = (from t in this.ObjectContext.tasks
                         join d in this.ObjectContext.distributionmaps on t.DmId equals d.Id
                         where t.Id == iTaskID
                         select d).SingleOrDefault();
            return dmap;
        }

        public List<distributionmap> GetDistributionMapByUserID(int iUserID)
        { 
            return (from t in this.ObjectContext.tasks
                        join d in this.ObjectContext.distributionmaps on t.DmId equals d.Id
                        where t.AuditorId == iUserID
                        select d).ToList();
        }

        public List<task> GetTasksByUserID(int iUserID)
        { 
            return (from t in this.ObjectContext.tasks
                        where t.AuditorId == iUserID
                        select t).ToList();
        }

        public List<distributionmapcoordinate> GetDistributionCoordinates(int iDistributionMapID)
        { 
            var coordinates = (from c in this.ObjectContext.distributionmapcoordinates
                        where c.DistributionMapId == iDistributionMapID
                    select c).ToList();
            return coordinates;
        }

        public DAL.task GetTaskByID(int iTaskID)
        {
            var oTask = (from t in this.ObjectContext.tasks
                         where t.Id == iTaskID
                         select t).First();
            return oTask;
        }

        public user GetTaskAuditor(int iTaskID)
        {
            var oTask = (from t in this.ObjectContext.tasks
                        where t.Id == iTaskID
                        select t).First();
            return (from u in this.ObjectContext.users
                   where u.Id == oTask.AuditorId
                   select u).First();
        }

        /*
        [Query(IsDefault = true)]
        public IQueryable<distributionmap> GetDistributionMapsByTaskID(int iTaskID)
        { 
            // From TaskId, get AuditorID
            var oTask = (from t in this.ObjectContext.tasks
                         where t.Id == iTaskID
                         select t).First();

            // From AuditoID, Get Tasks
            var campaignList = (from t in this.ObjectContext.tasks
                                join d in this.ObjectContext.distributionmaps on t.DmId equals d.Id
                                select d);
            return campaignList;
        }
        */


        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'users' query.
        [Query(IsDefault = true)]
        public IQueryable<user> GetUsers()
        {
            return this.ObjectContext.users;
        }

        public DAL.user GetUserByID(int id)
        {
            return this.ObjectContext.users.Where(u=> u.Id == id).FirstOrDefault();
        }

        public DAL.user GetUser(string userName, string password)
        { 
            return this.ObjectContext.users
                .Where(u => u.UserName == userName && u.Password == password)
                .FirstOrDefault();
        }

        public gtu GetGtuByID(int iGtuID)
        {
            return GtuDB.GetGtuByID(iGtuID);
        }

        public List<gtu> GetGtuList()
        {
            return this.ObjectContext.gtus.ToList();
        }

        // Get Gtu assigned to Auditor
        public List<gtu> GetGtuListByUserID(int iUserID)
        {
            return GtuDB.GetGtuListByUserID(iUserID);
        }

        // Get an Auditor's not assigned Gtus
        public List<gtu> GetGtuListByUserID_NotAssigned(int iUserID)
        {
            List<DAL.gtu> allGtu = GetGtuListByUserID(iUserID);
            List<DAL.gtu> gtusNotAssigned = new List<gtu>();
            foreach (gtu g in allGtu)
            {
                taskgtuinfomapping m = this.GetTaskGtuInfoMapping_Last(g.Id);
                if (m != null)
                {
                    if (m.UserId != null && m.UserId != 0 && m.TaskId != 0)
                        continue;
                }
                gtusNotAssigned.Add(g);
            }

            return gtusNotAssigned;
        }

        public IQueryable<gtubag> GetGtuBags()
        {
            return this.ObjectContext.gtubags;
        }

        public List<gtubag> GetUnassignedGtuBags()
        { 
            var bags = (from p in this.ObjectContext.gtubags
                             where p.UserId == null
                             select p);

            return bags.ToList();
        }

        public int GetGtuCountInBag(int iBagID)
        {
            return this.ObjectContext.gtus.Where(it => it.GTUBagId == iBagID).Count();
        }

        public List<gtubag> GetGtuBagsByAuditor(int? userID)
        {
            var bags = (from p in this.ObjectContext.gtubags
                         where p.UserId == userID
                         select p);

            return bags.ToList();
        }

        public void AssignGtuBagToAuditor(gtubag bag)
        {
            timmEntities context = new timmEntities();
            gtubag theBag = context.gtubags.SingleOrDefault(p => p.Id == bag.Id);
            theBag.UserId = bag.UserId;
            context.SaveChanges();
        }

        public void RemoveGtuBagFromAuditor(gtubag bag)
        {
            timmEntities context = new timmEntities();
            gtubag theBag = context.gtubags.SingleOrDefault(p => p.Id == bag.Id);
            theBag.UserId = null;
            context.SaveChanges();
        }

        public List<gtu> GetGtusByBagID(int iBagID)
        {
            return this.ObjectContext.gtus.Where(it=>it.GTUBagId == iBagID).ToList();
        }

        public List<gtu> GetGtusNotInBag()
        {
            var gtus = (from g in this.ObjectContext.gtus
                           where g.GTUBagId == null
                           select g);
            return gtus.ToList();
        }

        public void AddGtuToBag(DAL.gtu g)
        {
            timmEntities context = new timmEntities();
            gtu oGTU = context.gtus.Where(u=> u.Id == g.Id).FirstOrDefault();
            if (oGTU.GTUBagId == g.GTUBagId) return;

            oGTU.GTUBagId = g.GTUBagId;
            context.SaveChanges();
        }

        public void RemoveGtuFromBag(DAL.gtu g)
        {
            timmEntities context = new timmEntities();
            gtu oGTU = context.gtus.Where(it=>it.Id == g.Id).FirstOrDefault();
            if (oGTU.GTUBagId == null) return;

            oGTU.GTUBagId = null;
            context.SaveChanges();
        }
        
        public List<user> GetAuditors()
        {
            return this.ObjectContext.users.ToList();
        }
        
        public List<company> GetCompanies()
        {
            return this.ObjectContext.companies.ToList();
        }

        public List<DAL.user> GetUsersByCompanyID(int? iCompanyID)
        { 
            IList<int> employeeRoles = DAL.Lookups.EmployeeRoles.Keys;
            var users = (from u in this.ObjectContext.users
                             where u.CompanyId == iCompanyID && employeeRoles.Contains(u.Role)
                             select u);
            return users.ToList();
        }

        public List<DAL.user> GetAvailableUsersByCompanyID(int? iCompanyID)
        {
            IList<int> employeeRoles = DAL.Lookups.EmployeeRoles.Keys;
            var users = (from u in this.ObjectContext.users
                         where u.CompanyId == iCompanyID && employeeRoles.Contains(u.Role)
                         select u);
            List<DAL.user> uList = users.ToList();
            List<DAL.user> reuList = new List<user>();
            int iUserID = 0;
            foreach (DAL.user u in uList)
            {
                iUserID = u.Id;
                DAL.taskgtuinfomapping tgi =
                    ObjectContext.taskgtuinfomappings.Where(it => it.UserId == iUserID)
                    .OrderByDescending(it => it.Id).FirstOrDefault();
                if (tgi != null)
                {
                    if (tgi.GTUId == null && tgi.GTUId == 0)
                    {
                        reuList.Add(u);
                    }
                }
                else
                {
                    reuList.Add(u);
                }
            }

            return reuList;
        }

        public int AddCompany(DAL.company c)
        {
            this.ObjectContext.companies.AddObject(c);
            this.ObjectContext.SaveChanges();
            return c.Id;
        }

        public int AddEmployee(DAL.user u)
        {
            this.ObjectContext.users.AddObject(u);
            this.ObjectContext.SaveChanges();
            return u.Id;
        }

        public SortedList<int, string> GetEmployeeRoleList()
        {
            SortedList<int, string> roleList = new SortedList<int, string>();
            roleList.Add(1, "Walker");
            roleList.Add(48, "Driver");
            return roleList; 
        }

        public taskgtuinfomapping GetTaskGtuMappingByGtuID_Last(int iGtuID)
        {
            return (from m in this.ObjectContext.taskgtuinfomappings
                           where m.GTUId == iGtuID
                           orderby m.Id descending
                           select m).FirstOrDefault();
        }
        /*
        public void AssignGtuToEmployee(int iGtuID, int iUserID, int iTaskID)
        {
            // check whether the Gtu is re-assigned
            taskgtuinfomapping lastMap = GetTaskGtuMappingByGtuID_Last(iGtuID);
            if (lastMap == null)
            {
                if (iUserID == 0 | iTaskID == 0)
                    return;
            }
            else
            {
                if (lastMap.UserId == iUserID && lastMap.TaskId == iTaskID)
                    return;

                if((lastMap.UserId == 0 | lastMap.TaskId == 0) && (iUserID == 0 | iTaskID ==0))
                    return;
            }

            DAL.taskgtuinfomapping map = new taskgtuinfomapping();
            map.GTUId = iGtuID;
            map.UserId = iUserID;
            map.TaskId = iTaskID;

            timmEntities context = new timmEntities();
            context.AddTotaskgtuinfomappings(map);
            context.SaveChanges();
        }
        */
        public List<ViewGtuInTask> GetGtusInTask(int iTaskID)
        { 
            var query = (from g in this.ObjectContext.ViewGtuInTasks
                             where g.TaskId == iTaskID
                             orderby g.GtuID
                             select g);
            return query.ToList();
        }

        public List<ViewGtuInTask> GetGtuListByTaskID(int iTaskID)
        {
            return this.ObjectContext.ViewGtuInTasks.Where(it => it.TaskId == iTaskID).ToList();
            /*
            timmEntities myEntity = new timmEntities();
            System.Data.Objects.ObjectResult<GtuInTask> results = myEntity.GetGtusbyTaskID(iTaskID);

            List<GtuInTask> gtuList = new List<GtuInTask>();
            foreach (GtuInTask g in results)
            {
             * 
                    gtuList.Add(g);
            }

            return gtuList;
            */
        }

        public taskgtuinfomapping GetTaskGtuInfoMapping_Last(int iGtuID)
        {
            return this.ObjectContext.taskgtuinfomappings
                .Where(it => it.GTUId == iGtuID)
                .OrderByDescending(it => it.Id).FirstOrDefault();
        }

        public List<distributionmapcoordinate> GetDistributionmapCoordinatesByTaskID(int iTaskID)
        {
            
            task t = this.GetTaskByID(iTaskID);
            return GetDistributionmapCoordinatesByDMapID(t.DmId);
        }

        public List<distributionmapcoordinate> GetDistributionmapCoordinatesByDMapID(int iDistributionMapID)
        { 
            return (from c in this.ObjectContext.distributionmapcoordinates
                             where c.DistributionMapId == iDistributionMapID
                             select c).ToList();
        }

        public List<monitoraddress> GetMonitorAddressListByDMapID(int iDMapID)
        {
            return MonitorAddressDB.GetMonitorAddressListByDMapID(iDMapID);
        }


        public taskgtuinfomapping GetTaskGtuInfoByGtuID(int iGtuID)
        {
            taskgtuinfomapping map = (from m in this.ObjectContext.taskgtuinfomappings
                                      where m.GTUId == iGtuID
                                      orderby m.Id descending
                                      select m).FirstOrDefault();
            return map;
        }

        public gtuinfo GetGtuInfoByGtuID(int iGtuID)
        {
            taskgtuinfomapping map = GetTaskGtuInfoByGtuID(iGtuID);
            return GetGtuInfo(map.Id);
        }

        public gtuinfo GetGtuInfo(int iTaskGtuInfoID)
        { 
            gtuinfo info = (from i in this.ObjectContext.gtuinfoes
                        where i.TaskgtuinfoId == iTaskGtuInfoID
                        orderby i.Id descending
                        select i).FirstOrDefault();
            return info;
        }

        public List<gtuinfo> GetGtuHistoryByGtuID(int iGtuID)
        {
            taskgtuinfomapping m = GetTaskGtuInfoByGtuID(iGtuID);
            return GetGtuHistory(m.Id);
        }

        public List<gtuinfo> GetGtuHistory(int iTaskGtuInfoID)
        {
            return (from i in this.ObjectContext.gtuinfoes
                        where i.TaskgtuinfoId == iTaskGtuInfoID
                        orderby i.Id
                        select i).ToList();
        }


    }   // End of class
}


