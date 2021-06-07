using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;
using System.Data.Linq;
using GPS.DomainLayer.Interfaces;
using GPS.DataLayer.ValueObjects;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;
using NHibernate.Criterion;

using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace GPS.DataLayer
{
    /// <summary>
    /// Class <see cref="CampaignRepository"/> is responsible for reading and 
    /// writing campaign entities into the database.
    /// </summary>
    public class CampaignRepository : RepositoryBase, GPS.DataLayer.ICampaignRepository
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CampaignRepository() { }

        public CampaignRepository(ISession session) : base(session) { }

        /// <summary>
        /// Add specified campaign to the database.
        /// </summary>
        /// <param name="campaign">The campaign to add.</param>
        public void Create(Campaign campaign)
        {
            base.Insert(campaign);
        }

        public IList<int> GetBoxIDs(int iCampaignID)
        {
            string sQuery = @"
select distinct b.BoxId
from premiumcrouteboxmappings b
join premiumcroutes p on b.PreminumCRouteId = p.Id
join campaigncrouteimported i on i.PremiumCRouteId = p.Id
join campaigns c on i.CampaignId = c.Id
where c.Id = {0}
";
            sQuery = string.Format(sQuery, iCampaignID);

            System.Data.DataTable dtID = new System.Data.DataTable("ID");
            List<int> idList = new List<int>();

            if (InternalSession.Connection is MySqlConnection)
            {
                MySqlDataAdapter myAdapter = new MySqlDataAdapter(sQuery, (MySqlConnection)InternalSession.Connection);
                myAdapter.Fill(dtID);
            }
            else if (InternalSession.Connection is System.Data.SqlClient.SqlConnection)
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sQuery,
                    (System.Data.SqlClient.SqlConnection)InternalSession.Connection);
                sqlAdapter.Fill(dtID);
            }

            foreach (System.Data.DataRow row in dtID.Rows)
                idList.Add(Convert.ToInt32(row[0]));

            return idList;
            //return InternalSession.CreateQuery(sQuery).List<int>();
        }

        /// <summary>
        /// Return the specified campaign entity.
        /// </summary>
        /// <param name="id">The Id of the campaign to return.</param>
        /// <returns>The campaign entity</returns>
        public Campaign GetEntity(int id)
        {
            return InternalSession.Get<Campaign>(id);
        }

        /// <summary>
        /// Return all campaigns.
        /// </summary>
        /// <returns>All campaigns.</returns>
        public IEnumerable<Campaign> GetAllEntities()
        {
            return InternalSession.Linq<Campaign>()
                .OrderByDescending(c => c.Date)
                .ThenBy(c => c.ClientCode)
                .ThenBy(c => c.AreaDescription)
                .ThenByDescending(c => c.Sequence);
        }

        /// <summary>
        /// Return all campaigns created by the specified user. If the user is 
        /// an administrator, return all campaigns. Returned campaignss should
        /// be ordered by date(descending), customer name(ascending), user name
        /// (ascending), and sequence(descending).
        /// </summary>
        /// <param name="user">The user whose campaigns should be returned</param>
        /// <returns>The campaigns created by the specified user.</returns>
        public IEnumerable<Campaign> GetAllByUser(User user)
        {
            IEnumerable<Campaign> campaigns = null;

            if ((user.Role & UserRoles.Admin) == UserRoles.Admin)
            {
                campaigns = InternalSession.Linq<Campaign>().Where(c => c.UserName == user.UserName);
            }
            else
            {
                campaigns = InternalSession.Linq<Campaign>()
                    .OrderByDescending(c => c.Date, new SameDayComparer())
                    .ThenBy(c => c.CustemerName)
                    .ThenBy(c => c.UserName)
                    .ThenByDescending(c => c.Sequence);
            }

            return campaigns;
        }

        public IEnumerable<Campaign> GetAllBySubmapStatus(User user)
        {
            ////submap = 0
            ICriteria criteria = InternalSession.CreateCriteria(typeof(Campaign), "cam")
                .CreateCriteria("Users", "users")
                //.Add(Restrictions.Eq("users.Status", 0))
                .AddOrder(new Order("cam.Date", false))
                .AddOrder(new Order("cam.CustemerName", true))
                .AddOrder(new Order("cam.UserName", true))
                .AddOrder(new Order("cam.Sequence", false));
            return criteria.List<Campaign>().Where<Campaign>(
                delegate(Campaign campaign)
                {
                    foreach (KeyValuePair<User, StatusInfo> kvp in campaign.Users)
                    {
                        if (kvp.Key.Id == user.Id && kvp.Value.Status == 0)
                            return true;
                    }
                    return false;
                }

                );
            //IList<Campaign> finalList = new List<Campaign>();
            //User temUser = new User();
            //foreach(Campaign cam in origianlList){
            //    temUser = (cam.Users.Keys).First();
            //    if (temUser.Id == user.Id)
            //    {
            //        finalList.Add(cam);
            //    }
            //}
            //return finalList;


            //var cam = from c in InternalSession.Linq<Campaign>()
            //          from u in c.Users
            //          where u.Key == user && u.Value == new StatusInfo(0)
            //          //orderby c.Name
            //          select c;
            //return cam.ToList<Campaign>();
            //ICriteria criteria = InternalSession.CreateCriteria(typeof(Campaign), "cm").CreateCriteria("Users", "users")
            //    .Add(Restrictions.Eq("users.Status", 0)).Add(Restrictions.Eq("users.userId", user.Id));
            //return criteria.List<Campaign>();


            //var list = from c in InternalSession.Linq<Campaign>()
            //           from u in c.Users
            //           where u.Key.Id == currentUserId && u.Value.Status == 0
            //           //orderby c.Date descending,c.CustemerName,c.UserName,c.Sequence descending
            //           select c;
            //return list;

            //const string queryFormat = "select distinct t from Campaign c join c.BlockGroupBoxMappings tbm where tbm.BoxId = :boxId";
            //return InternalSession.CreateQuery(queryFormat).SetInt32("boxId", boxId).List<BlockGroup>();


        }

        public IEnumerable<Campaign> GetAllBySubmapStatusWithoutUser()
        {
            ////submap = 0
            ICriteria criteria = InternalSession.CreateCriteria(typeof(Campaign), "cam")
                .CreateCriteria("Users", "users")
                //.Add(Restrictions.Eq("users.Status", 0))
                .AddOrder(new Order("cam.Date", false))
                .AddOrder(new Order("cam.CustemerName", true))
                .AddOrder(new Order("cam.UserName", true))
                .AddOrder(new Order("cam.Sequence", false));
            return criteria.List<Campaign>().Where<Campaign>(
                delegate(Campaign campaign)
                {
                    foreach (KeyValuePair<User, StatusInfo> kvp in campaign.Users)
                    {
                        if (kvp.Value.Status == 0)
                            return true;
                    }
                    return false;
                }

                );


        }

        public Campaign GetCamNameByTaskId(int tid)
        {
            ICriteria criteria = InternalSession.CreateCriteria(typeof(Campaign), "cam")
                .CreateCriteria("SubMaps", "submaps")
                .CreateCriteria("DistributionMaps", "distributionmaps")
                .CreateCriteria("Tasks", "tasks")
                .Add(Restrictions.Eq("tasks.Id", tid));
            IList<Campaign> clist = criteria.List<Campaign>();
            return clist[0];

        }

        public string[] GetCamNameByTasks(int[] taskids)
        {
            string[] nameList = new string[taskids.Length];
            for (int i = 0; i < taskids.Length; i++)
            {
                ICriteria criteria = InternalSession.CreateCriteria(typeof(Campaign), "cam")
                .CreateCriteria("SubMaps", "submaps")
                .CreateCriteria("DistributionMaps", "distributionmaps")
                .CreateCriteria("Tasks", "tasks")
                .Add(Restrictions.Eq("tasks.Id", taskids[i]));
                IList<Campaign> clist = criteria.List<Campaign>();



                User u = null;
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    u = ws.Repositories.UserRepository.GetUser(clist[0].CreatorName);
                }
                string userCode = null != u ? u.UserCode : string.Empty;
                nameList[i] = "<br><b>" + Campaign.ConstructCompositeName(clist[0].Date, clist[0].ClientCode, clist[0].AreaDescription, userCode, clist[0].Sequence) + "</b></br>";
            }
            for (int x = taskids.Length - 1; x - 1 >= 0; x--)
            {
                if (nameList[x] == nameList[x - 1])
                {
                    nameList[x] = "";
                }
            }
            return nameList;

        }

        public string GetCamNameByReports(int taskid)
        {
            string[] nameList = GetCamNameByReports(new int[] { taskid });
            return nameList[0];
        }

        public string[] GetCamNameByReports(int[] taskids)
        {
            string[] nameList = new string[taskids.Length];
            for (int i = 0; i < taskids.Length; i++)
            {
                ICriteria criteria = InternalSession.CreateCriteria(typeof(Campaign), "cam")
                .CreateCriteria("SubMaps", "submaps")
                .CreateCriteria("DistributionMaps", "distributionmaps")
                .CreateCriteria("Tasks", "tasks")
                .Add(Restrictions.Eq("tasks.Id", taskids[i]));
                IList<Campaign> clist = criteria.List<Campaign>();



                User u = null;
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    u = ws.Repositories.UserRepository.GetUser(clist[0].CreatorName);
                }
                string userCode = null != u ? u.UserCode : string.Empty;
                //nameList[i] = "<br><b>" + Campaign.ConstructCompositeName(clist[0].Date, 
                //    clist[0].ClientCode, clist[0].AreaDescription, userCode, clist[0].Sequence) + 
                //    "</b>" + "&nbsp;&nbsp;" + "<a href='Reports.aspx?cid=" + clist[0].Id + 
                //    "' target='_blank'>[Report]</a></br>";

                //"NewControlCenter.aspx?id=" + campaign.Id + "#PrintPreview"
                nameList[i] = "<br><b>" + Campaign.ConstructCompositeName(clist[0].Date,
                    clist[0].ClientCode, clist[0].AreaDescription, userCode, clist[0].Sequence) +
                    "</b>" + "&nbsp;&nbsp;" + "<a href='Handler/PhantomjsPrintHandler.ashx?campaignId=" + clist[0].Id + "&type=print" +
                    "' target='_blank'>[Print]</a></br>";
            }
            for (int x = taskids.Length - 1; x - 1 >= 0; x--)
            {
                if (nameList[x] == nameList[x - 1])
                {
                    nameList[x] = "";
                }
            }
            return nameList;

        }


        public IEnumerable<Campaign> GetAllByDMStatus(User user)
        {
            //distributionmap = 1
            //ICriteria criteria = InternalSession.CreateCriteria(typeof(Campaign), "cam")
            //    .CreateCriteria("Users", "users")
            //    .Add(Restrictions.Eq("users.Status", 1))
            //    .Add(Restrictions.Eq("users.Id", currentUserId))
            //    .AddOrder(new Order("cam.Date",false))
            //    .AddOrder(new Order("cam.CustemerName",true))
            //    .AddOrder(new Order("cam.UserName",true))
            //    .AddOrder(new Order("cam.Sequence",false));

            //return criteria.List<Campaign>();
            //var list = from c in InternalSession.Linq<Campaign>()
            //           from u in c.Users
            //           //where u.Key.Id == currentUserId && u.Value.Status == 1
            //           //orderby c.Date descending, c.CustemerName, c.UserName, c.Sequence descending
            //           select c;
            //return list;

            //distributionmap = 1
            ICriteria criteria = InternalSession.CreateCriteria(typeof(Campaign), "cam")
                .CreateCriteria("Users", "users")
                //.Add(Restrictions.Eq("users.Status", 1))
                .AddOrder(new Order("cam.Date", false))
                .AddOrder(new Order("cam.CustemerName", true))
                .AddOrder(new Order("cam.UserName", true))
                .AddOrder(new Order("cam.Sequence", false));
            return criteria.List<Campaign>().Where<Campaign>(
                delegate(Campaign campaign)
                {
                    foreach (KeyValuePair<User, StatusInfo> kvp in campaign.Users)
                    {
                        if (kvp.Key.Id == user.Id && kvp.Value.Status == 1)
                            return true;
                    }
                    return false;
                }

                );
            //IList<Campaign> finalList = new List<Campaign>();
            //User temUser = new User();
            //foreach (Campaign cam in origianlList)
            //{
            //    temUser = (cam.Users.Keys).First();
            //    if (temUser.Id==user.Id)
            //    {
            //        finalList.Add(cam);
            //    }
            //}
            //return finalList;

        }

        public IEnumerable<Campaign> GetAllCampByDMStatusWithoutUser()
        {
            //distributionmap = 1
            ICriteria criteria = InternalSession.CreateCriteria(typeof(Campaign), "cam")
                .CreateCriteria("Users", "users")
                //.Add(Restrictions.Eq("users.Status", 1))
                .AddOrder(new Order("cam.Date", false))
                .AddOrder(new Order("cam.CustemerName", true))
                .AddOrder(new Order("cam.UserName", true))
                .AddOrder(new Order("cam.Sequence", false));
            return criteria.List<Campaign>().Where<Campaign>(
                delegate(Campaign campaign)
                {
                    foreach (KeyValuePair<User, StatusInfo> kvp in campaign.Users)
                    {
                        if (kvp.Value.Status == 1)
                            return true;
                    }
                    return false;
                }

                );
        }


        /// <summary>
        /// Return the maximum Sequence value of those campaigns created by the 
        /// specified user, for the specified customer, and in the specified date.
        /// </summary>
        /// <param name="date">The date when those campaigns are created. Only 
        /// year, month, and date counts; hours, minutes, and seconds will be ignored.</param>
        /// <param name="userName">The user name of the campaigns.</param>
        /// <param name="clientCode">The client code of the new campaign.</param>
        /// <param name="areaDescription">The area description of the new campaign.</param>
        /// <returns>The maximum sequence value if any campaigns satisfying the 
        /// conditions exist, or 0 if not.</returns>
        public int GetMaxSequence(DateTime date, String userName, String clientCode, String areaDescription)
        {
            Campaign campaign = InternalSession.Linq<Campaign>()
                .Where(c => c.Date >= new DateTime(date.Year, date.Month, date.Day) && c.Date <= new DateTime(date.Year, date.Month, date.Day, 23, 59, 59))
                .Where(c => c.CreatorName == userName)
                .Where(c => c.ClientCode == clientCode)
                .Where(c => c.AreaDescription == areaDescription)
                .OrderByDescending(c => c.Sequence)
                .FirstOrDefault<Campaign>();

            int maxSequence = 0;
            if (null != campaign)
            {
                maxSequence = campaign.Sequence;
            }

            return maxSequence;
        }

        /// <summary>
        /// Update the database with specified campaign entity if the campaign exists.
        /// </summary>
        /// <param name="campaign">The campaign entity.</param>
        public void Update(Campaign campaign)
        {
            base.Update(campaign);
        }

        public void UpdateCopy(Campaign campaign)
        {
            base.UpdateCopy(campaign);
        }

        public void NewMonitorAddress(MonitorAddresses ma)
        {
            base.Insert(ma);
        }

        /// <summary>
        /// Delete the specified campaign from the database.
        /// </summary>
        /// <param name="campaignId">The Id of the campaign to delete.</param>
        public void Delete(int campaignId)
        {
            Campaign campaign = GetEntity(campaignId);
            this.Delete(campaign);
        }

        /// <summary>
        /// Delete the specified campaign from the database.
        /// </summary>
        /// <param name="camp">The campaign to delete.</param>
        public void Delete(Campaign camp)
        {
            base.Delete(camp);
        }
    }

    /// <summary>
    /// <see cref="SameDayComparer"/> is reponsible for comparing if two 
    /// <see cref="DateTime"/>s are within the same day.
    /// </summary>
    class SameDayComparer : IComparer<DateTime>
    {
        #region IComparer<DateTime> Members

        public int Compare(DateTime x, DateTime y)
        {
            int res = 0;
            bool greater = (x.Year > y.Year)
                || (x.Year == y.Year && x.Month > y.Month)
                || (x.Year == y.Year && x.Month == y.Month && x.Day > y.Day);
            bool equal = x.Year == y.Year && x.Month == y.Month && x.Day == y.Day;
            if (greater)
            {
                res = 1;
            }
            else if (!equal)
            {
                res = -1;
            }
            return res;
        }

        #endregion
    }

    public class MonitorAddressRepository : RepositoryBase
    {
        public MonitorAddresses GetEntity(int id)
        {
            return InternalSession.Get<MonitorAddresses>(id);
        }

        public void DeleteEntity(int id)
        {
            MonitorAddresses address = InternalSession.Get<MonitorAddresses>(id);
            base.Delete(address);
        }

        public void DeleteEntity(MonitorAddresses address)
        {
            base.Delete(address);
        }
    }

}
