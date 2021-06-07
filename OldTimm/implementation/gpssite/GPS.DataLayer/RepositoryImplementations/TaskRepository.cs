using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;
using NHibernate.Criterion;
using System.Data.SqlClient;

namespace GPS.DataLayer.RepositoryImplementations
{
    public class TaskRepository : RepositoryBase, GPS.DataLayer.ITaskRepository
    {
        public TaskRepository() { }

        public TaskRepository(ISession session) : base(session) { }

        public IList<Task> GetAllTasks()
        {
            //return InternalSession.Linq<Task>().ToList<Task>();
            string queryFormat = "select * from task as t, distributionmaps as dm, submaps as s, campaigns as c where t.DmId=dm.Id and dm.SubMapId=s.Id and s.CampaignId=c.Id and t.Status=0 order by c.Id";
            IList<Task> list = InternalSession.CreateSQLQuery(queryFormat).AddEntity("t", typeof(Task)).List<Task>();

            //string myconn = System.Configuration.ConfigurationSettings.AppSettings["DataBasePath"];
            //SqlConnection myconnectionS = new SqlConnection(myconn);
            //myconnectionS.Open();

            //SqlCommand mycommandS = new SqlCommand(queryFormat, myconnectionS);

            //SqlDataReader myreaderS = mycommandS.ExecuteReader();

            //while (myreaderS.Read())
            //{
            //    list = myreaderS.GetEnumerator();
            //}
            //myreaderS.Close();
            //myconnectionS.Close();


            return list;
        }

        public int GetStartOrStopByTaskId(int tid)
        {
            int status = 0;
            string sql = "select * from tasktime as tt where tt.TaskId=" + tid + " order by tt.Time desc";
            IList<TaskTime> tasttimes = InternalSession.CreateSQLQuery(sql).AddEntity("tt", typeof(TaskTime)).List<TaskTime>();
            if (tasttimes.Count > 0)
            {
                status = tasttimes[0].TimeType;
            }
            return status;
        }

        public IList<Task> GetAllReports()
        {
            //return InternalSession.Linq<Task>().ToList<Task>();
            string queryFormat = "select top 200 * from task as t, distributionmaps as dm, submaps as s, campaigns as c where t.DmId=dm.Id and dm.SubMapId=s.Id and s.CampaignId=c.Id and t.Status=1 order by t.Date desc";
            if(InternalSession.Connection is MySql.Data.MySqlClient.MySqlConnection)
                queryFormat = "select * from task as t, distributionmaps as dm, submaps as s, campaigns as c where t.DmId=dm.Id and dm.SubMapId=s.Id and s.CampaignId=c.Id and t.Status=1 order by t.Date desc limit 200";
            
            IList<Task> list = InternalSession.CreateSQLQuery(queryFormat).AddEntity("t", typeof(Task)).List<Task>();

            //string myconn = System.Configuration.ConfigurationSettings.AppSettings["DataBasePath"];
            //SqlConnection myconnectionS = new SqlConnection(myconn);
            //myconnectionS.Open();

            //SqlCommand mycommandS = new SqlCommand(queryFormat, myconnectionS);

            //SqlDataReader myreaderS = mycommandS.ExecuteReader();

            //while (myreaderS.Read())
            //{
            //    list = myreaderS.GetEnumerator();
            //}
            //myreaderS.Close();
            //myconnectionS.Close();


            return list;
        }

        public IList<Task> GetTasks(int[] ids)
        {
            return InternalSession.CreateCriteria(typeof(Task)).Add(Expression.In("Id", ids)).List<Task>();
        }


        public IEnumerable<Task> GetTasksByGtu(Gtu gtu)
        {
            var list = from t in InternalSession.Linq<Task>()
                       from m in t.Taskgtuinfomappings
                       where m.GTU.Id == gtu.Id && (t.Date == System.DateTime.Now || t.Date > System.DateTime.Now)
                       //orderby c.Date descending, c.CustemerName, c.UserName, c.Sequence descending
                       select t;
            return list;
            //return InternalSession.CreateCriteria(typeof(Task)).Add(Expression.In("Id", ids)).List<Task>();
        }

        public IList<Task> GetTaskGtuMappingsByUser(int userid)
        {
            const string queryFormat =
                  " select distinct t from Task t join t.Taskgtuinfomappings tg where"
                + " (t.Id= tg.TaskId and tg.UserId =(:userid))";
            return InternalSession.CreateQuery(queryFormat)
                .SetInt32("userid", userid)
                .List<Task>();
            ////submap = 0
            //ICriteria criteria = InternalSession.CreateCriteria(typeof(Taskgtuinfomapping), "taskgtu")
            //    .Add(Restrictions.Eq("taskgtu.UserId",user.Id))
            //    .CreateCriteria("Task", "task")
            //    .CreateAlias
            //    .Add(Restrictions.Eq("users.Status", 0))
            //    .AddOrder(new Order("cam.Date", false))
            //    .AddOrder(new Order("cam.CustemerName", true))
            //    .AddOrder(new Order("cam.UserName", true))
            //    .AddOrder(new Order("cam.Sequence", false));
        }

        public IList<string> GetUserColorByTaskId(int tid)
        {
            const string queryFormat =
                  " select distinct UserColor from Taskgtuinfomapping where"
                + " TaskId =(:taskid)";
            return InternalSession.CreateQuery(queryFormat)
                .SetInt32("taskid", tid)
                .List<string>();
        }

        public IList<Task> GetTasks(DateTime date)
        {
            //return InternalSession.Linq<Task>().Where(t => t.Date.Equals(date)).ToList<Task>();
            var query = from t in InternalSession.Linq<Task>()
                        where t.Status == 0
                        select t;

            return query.ToList<Task>();

        }

        public Task GetTask(int gid)
        {
            return InternalSession.Get<Task>(gid);
        }

        public Task AddTask(Task Task)
        {
            base.Insert(Task);
            return Task;
        }

        public void DeleteTask(int TaskID)
        {
            Task Task = this.GetTask(TaskID);
            base.Delete(Task);
        }

        public void MarkFinish(int TaskID)
        {
            Task task = this.GetTask(TaskID);
            task.Status = 1;
            base.Update(task);
        }

        public void MoveReportBackToTask(int TaskID)
        {
            Task task = this.GetTask(TaskID);
            task.Status = 0;
            base.Update(task);
        }

        public Task UpdateTask(Task Task)
        {
            Task u = this.GetTask(Task.Id);

            base.Update(u);
            return u;
        }


        public Task UpdateTaskWithChildren(Task Task)
        {
            //Task u = this.GetTask(Task.Id);
            //if (u != null)
            //{
            //    u.Taskgtuinfomappings = Task.Taskgtuinfomappings;
            //}
            base.Update(Task);
            return Task;
        }

        public Task UpdateTaskWithChildrens(Task Task)
        {
            //Task u = this.GetTask(Task.Id);
            //if (u != null)
            //{
            //    u.Taskgtuinfomappings = Task.Taskgtuinfomappings;
            //}
            base.UpdateCopy(Task);
            return Task;
        }
        public IList<Address> GetAddressByTaskId(int tid)
        {
            //return InternalSession.Linq<Task>().ToList<Task>();
            string queryFormat = "select * from addresses as ad, task as t, distributionmaps as dm, submaps as s, campaigns as c where ad.CampaignId=c.Id and dm.SubMapId=s.Id and s.CampaignId=c.Id and t.DmId=dm.Id and t.Id=(:taskid) and t.Status=1";
            IList<Address> list = InternalSession.CreateSQLQuery(queryFormat).AddEntity("ad", typeof(Address)).SetInt32("taskid", tid).List<Address>();

            return list;
        }

        public IDictionary<int, List<int>> GetCampTaskMapping(int[] taskids)
        {
            Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
            ICriteria criteria = InternalSession.CreateCriteria(typeof(Campaign), "cam")
                .CreateCriteria("SubMaps", "submaps")
                .CreateCriteria("DistributionMaps", "distributionmaps")
                .CreateCriteria("Tasks", "tasks")
                .Add(Restrictions.In("tasks.Id", taskids.ToArray()));
            IList<Campaign> clist = criteria.List<Campaign>();
            foreach (Campaign camp in clist)
            {
                if (!result.ContainsKey(camp.Id))
                    result.Add(camp.Id, GetTaskByCamp(camp.Id));
            }
            return result;
        }

        public List<int> GetTaskByCamp(int cid)
        {
            List<int> tids = new List<int>();
            string queryFormat = "select * from task as t, distributionmaps as dm, submaps as s where dm.SubMapId=s.Id and s.CampaignId=:cid and t.DmId=dm.Id and t.Id in (select taskid from tasktime)";
            IList<Task> list = InternalSession.CreateSQLQuery(queryFormat).AddEntity("t", typeof(Task)).SetInt32("cid", cid).List<Task>();
            //int count = InternalSession.CreateSQLQuery(queryFormat).UniqueResult<int>();
            if (list.Count == 0)
            {
                string queryFormat2 = "select * from task as t, distributionmaps as dm, submaps as s where dm.SubMapId=s.Id and s.CampaignId=(:cpid) and t.DmId=dm.Id";
                IList<Task> list2 = InternalSession.CreateSQLQuery(queryFormat2).AddEntity("t", typeof(Task)).SetInt32("cpid", cid).List<Task>();
                if (list2 != null && list2.Count > 0)
                {
                    list2.ToList().ForEach(t => tids.Add(t.Id));
                }
            }
            else//Bug 390 Add an [else] code block,to add the taskid to the list
            {
                foreach (Task task in list)
                {
                    tids.Add(task.Id);
                }
            }
            return tids;
        }

        public IList<MonitorAddresses> GetMonitorAddressByDMId(int did)
        {
            //return InternalSession.Linq<Task>().ToList<Task>();
            string queryFormat = "select * from monitoraddresses as ad where ad.DmId=" + did;
            IList<MonitorAddresses> list = InternalSession.CreateSQLQuery(queryFormat).AddEntity("ad", typeof(MonitorAddresses)).List<MonitorAddresses>();

            return list;
        }
    }
}
