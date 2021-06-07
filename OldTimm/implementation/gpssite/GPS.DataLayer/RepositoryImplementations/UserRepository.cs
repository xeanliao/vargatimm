using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;
using NHibernate.Criterion;

namespace GPS.DataLayer
{
    public class UserRepository : RepositoryBase, GPS.DataLayer.IUserRepository
    {
        public UserRepository() { }

        public UserRepository(ISession session) : base(session) { }

        public User GetUser(string userName, string password)
        {
            return InternalSession.Linq<User>().Where(u => u.UserName == userName && u.Password == password).FirstOrDefault();
        }

        /// <summary>
        /// Return a <see cref="User"/> by its user name.
        /// </summary>
        /// <param name="userName">The user name of the user to be fetched.</param>
        /// <returns>A <see cref="User"/> object matching the specified user name.</returns>
        public User GetUser(string userName)
        {
            return InternalSession.Linq<User>().Where(u => u.UserName == userName).FirstOrDefault();
        }

        /// <summary>
        /// Return a <see cref="User"/> by its user id.
        /// </summary>
        /// <param name="userName">The user name of the user to be fetched.</param>
        /// <returns>A <see cref="User"/> object matching the specified user name.</returns>
        public User GetUser(int id)
        {
            return InternalSession.Get<User>(id);
        }

        public User GetUserByToken(string token)
        {
            var timeout = DateTime.Now.AddHours(-12);
            return InternalSession.Linq<User>().Where(i => i.Token == token && i.LastLoginTime > timeout).FirstOrDefault();
        }

        /// <summary>
        /// Return a list of all users.
        /// </summary>
        /// <returns>
        /// An <see cref="IList"/> containing all <see cref="User"/>s.
        /// </returns>
        public IList<User> GetAllUsers()
        {
            return InternalSession.Linq<User>().OrderBy(u => u.UserName).ToList<User>();
        }

        public IList<User> GetAllUsersByPrivilege(int privilege)
        {
            //var user = from a in InternalSession.Linq<User>()
            //          // where a.Groups.Where<Group>(g => g.Name == groupname).Count<Group>() > 0
            //           select a;
            //return user.ToList();
            var userid = (from u in InternalSession.Linq<User>()
                          from g in u.Groups
                          from p in g.Privileges
                          where p.Value == privilege
                          orderby u.UserName
                          select u.Id).Distinct();



            IList<int> userIds = userid.ToArray<int>();
            IList<User> userList = new List<User>();
            foreach (int id in userIds)
            {
                userList.Add(GetUser(id));
            }
            return userList;
        }

        public IList<User> GetAllUsersByGroups(int[] gs)
        {
            //var user = from a in InternalSession.Linq<User>()
            //          // where a.Groups.Where<Group>(g => g.Name == groupname).Count<Group>() > 0
            //           select a;
            //return user.ToList();
            var userid = (from u in InternalSession.Linq<User>()
                          from g in u.Groups
                          where (gs).Contains(g.Id)
                          //orderby u.UserName
                          select u.Id).Distinct();



            IList<int> userIds = userid.ToArray<int>();
            List<User> userList = new List<User>();
            foreach (int id in userIds)
            {
                userList.Add(GetUser(id));
            }

            var orderedUserList = userList.OrderBy(x => x.UserName).ToList();

            return orderedUserList; //userList;
        }


        public IList<User> GetAllUsersByGroup(int groupId)
        {
            var userlist = from u in InternalSession.Linq<User>()
                           from g in u.Groups
                           where g.Id == groupId
                           orderby u.UserName
                           select u;

            return userlist.ToList<User>();
        }

        /// <summary>
        /// Add a user to the database.
        /// </summary>
        /// <param name="user">A <see cref="User"/> object.</param>
        /// <returns>The <see cref="User"/> just added successfully.</returns>
        public User AddUser(User user)
        {
            base.Insert(user);
            return user;
        }

        /// <summary>
        /// Delete a user specified by user name.
        /// </summary>
        /// <param name="userName">The user name of the user to be deleted.</param>
        public void DeleteUser(string userName)
        {
            User user = this.GetUser(userName);
            base.Delete(user);
        }

        /// <summary>
        /// Update a user.
        /// </summary>
        /// <param name="userName">The user to be updated.</param>
        public User UpdateUser(User user)
        {
            User u = this.GetUser(user.UserName);
            if (u != null)
            {
                u.Password = user.Password;
                u.Role = user.Role;
                u.FullName = user.FullName;
                u.UserCode = user.UserCode;
                u.Enabled = user.Enabled;
                u.Email = user.Email;
                u.Groups = user.Groups;
                //u.Campaigns = user.Campaigns;
            }
            base.Update(u);
            return u;
        }

        public IList<User> GetWalkersByTaskid(int tId)
        {
            List<int> userList = new List<int>();
            User userTem = null;
            Task t = InternalSession.Linq<Task>().Where(ta => ta.Id == tId).FirstOrDefault();
            if (t != null && t.Taskgtuinfomappings != null)
            {
                foreach (Taskgtuinfomapping m in t.Taskgtuinfomappings)
                {
                    if ((m.UserId != 0) && (!userList.Contains(m.UserId)))
                    {

                        userTem = GetUser(m.UserId);
                        if (userTem.Groups.Count > 0)
                        {
                            var wflag = false;
                            foreach (Group g in userTem.Groups)
                            {
                                if (g.Id == 1)
                                {
                                    wflag = true;
                                    break;
                                }
                            }
                            if (wflag)
                            {
                                userList.Add(m.UserId);
                            }
                        }
                    }
                }
            }

            IList<User> list = InternalSession.CreateCriteria(typeof(User)).Add(Expression.In("Id", userList)).List<User>();
            return list;

        }

        public IList<User> GetDriversByTaskid(int tId)
        {
            List<int> userList = new List<int>();
            User userTem = null;
            Task t = InternalSession.Linq<Task>().Where(ta => ta.Id == tId).FirstOrDefault();
            if (t != null && t.Taskgtuinfomappings != null)
            {
                foreach (Taskgtuinfomapping m in t.Taskgtuinfomappings)
                {
                    if ((m.UserId != 0) && (!userList.Contains(m.UserId)))
                    {

                        userTem = GetUser(m.UserId);
                        if (userTem.Groups.Count > 0)
                        {
                            var wflag = false;
                            foreach (Group g in userTem.Groups)
                            {
                                if (g.Id == 48)
                                {
                                    wflag = true;
                                    break;
                                }
                            }
                            if (wflag)
                            {
                                userList.Add(m.UserId);
                            }
                        }
                    }
                }
            }

            IList<User> list = InternalSession.CreateCriteria(typeof(User)).Add(Expression.In("Id", userList)).List<User>();
            return list;

        }

        public IList<User> GetAuditorsByTaskid(int tId)
        {
            List<int> userList = new List<int>();
            User userTem = null;
            Task t = InternalSession.Linq<Task>().Where(ta => ta.Id == tId).FirstOrDefault();
            if (t != null && t.Taskgtuinfomappings != null)
            {
                foreach (Taskgtuinfomapping m in t.Taskgtuinfomappings)
                {
                    if ((m.UserId != 0) && (!userList.Contains(m.UserId)))
                    {

                        userTem = GetUser(m.UserId);
                        if (userTem.Groups.Count > 0)
                        {
                            var wflag = false;
                            foreach (Group g in userTem.Groups)
                            {
                                if (g.Id == 50)
                                {
                                    wflag = true;
                                    break;
                                }
                            }
                            if (wflag)
                            {
                                userList.Add(m.UserId);
                            }
                        }
                    }
                }
            }

            IList<User> list = InternalSession.CreateCriteria(typeof(User)).Add(Expression.In("Id", userList)).List<User>();
            return list;

        }


        public IList<User> GetWalkersByCampaignid(int cId)
        {
            var tasklist = (from c in InternalSession.Linq<Campaign>().Where(c => c.Id == cId)
                            from sm in c.SubMaps
                            from dm in sm.DistributionMaps
                            from t in dm.Tasks
                            select t).Distinct<Task>();

            List<int> userList = new List<int>();


            List<Task> taskList = tasklist.ToList<Task>();
            User userTem = null;
            foreach (Task t in tasklist)
            {
                if (t != null && t.Taskgtuinfomappings != null)
                {
                    foreach (Taskgtuinfomapping m in t.Taskgtuinfomappings)
                    {
                        if ((m.UserId != 0) && (!userList.Contains(m.UserId)))
                        {

                            userTem = GetUser(m.UserId);
                            if (userTem.Groups.Count > 0)
                            {
                                var wflag = false;
                                foreach (Group g in userTem.Groups)
                                {
                                    if (g.Id == 1)
                                    {
                                        wflag = true;
                                        break;
                                    }
                                }
                                if (wflag)
                                {
                                    userList.Add(m.UserId);
                                }
                            }
                        }
                    }
                }
            }

            IList<User> list = InternalSession.CreateCriteria(typeof(User)).Add(Expression.In("Id", userList)).List<User>();
            return list;

        }

        public IList<User> GetDriversByCampaignid(int cId)
        {
            var tasklist = (from c in InternalSession.Linq<Campaign>().Where(c => c.Id == cId)
                            from sm in c.SubMaps
                            from dm in sm.DistributionMaps
                            from t in dm.Tasks
                            select t).Distinct<Task>();

            List<int> userList = new List<int>();


            List<Task> taskList = tasklist.ToList<Task>();
            User userTem = null;
            foreach (Task t in tasklist)
            {
                if (t != null && t.Taskgtuinfomappings != null)
                {
                    foreach (Taskgtuinfomapping m in t.Taskgtuinfomappings)
                    {
                        if ((m.UserId != 0) && (!userList.Contains(m.UserId)))
                        {

                            userTem = GetUser(m.UserId);
                            if (userTem.Groups.Count > 0)
                            {
                                var wflag = false;
                                foreach (Group g in userTem.Groups)
                                {
                                    if (g.Id == 48)
                                    {
                                        wflag = true;
                                        break;
                                    }
                                }
                                if (wflag)
                                {
                                    userList.Add(m.UserId);
                                }
                            }
                        }
                    }
                }
            }

            IList<User> list = InternalSession.CreateCriteria(typeof(User)).Add(Expression.In("Id", userList)).List<User>();
            return list;

        }

        public IList<User> GetAuditorsByCampaignid(int cId)
        {
            var tasklist = (from c in InternalSession.Linq<Campaign>().Where(c => c.Id == cId)
                            from sm in c.SubMaps
                            from dm in sm.DistributionMaps
                            from t in dm.Tasks
                            select t).Distinct<Task>();

            List<int> userList = new List<int>();


            List<Task> taskList = tasklist.ToList<Task>();
            User userTem = null;
            foreach (Task t in tasklist)
            {
                if (t != null && t.Taskgtuinfomappings != null)
                {
                    foreach (Taskgtuinfomapping m in t.Taskgtuinfomappings)
                    {
                        if ((m.UserId != 0) && (!userList.Contains(m.UserId)))
                        {

                            userTem = GetUser(m.UserId);
                            if (userTem.Groups.Count > 0)
                            {
                                var wflag = false;
                                foreach (Group g in userTem.Groups)
                                {
                                    if (g.Id == 50)
                                    {
                                        wflag = true;
                                        break;
                                    }
                                }
                                if (wflag)
                                {
                                    userList.Add(m.UserId);
                                }
                            }
                        }
                    }
                }
            }

            IList<User> list = InternalSession.CreateCriteria(typeof(User)).Add(Expression.In("Id", userList)).List<User>();
            return list;

        }

        public IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByTaskidUserid(int tId, int uId)
        {
            var tasktimeS = (from tt in InternalSession.Linq<TaskTime>()
                             where tt.TaskId == tId && tt.TimeType == 0
                             orderby tt.Time
                             select tt.Time).FirstOrDefault();
            var tasktimeE = (from tt in InternalSession.Linq<TaskTime>()
                             where tt.TaskId == tId && tt.TimeType == 1
                             orderby tt.Time descending
                             select tt.Time).FirstOrDefault();


            var gtuinfolist = from t in InternalSession.Linq<Task>()
                              from tt in t.Tasktimes
                              from tm in t.Taskgtuinfomappings
                              from ginfo in tm.Gtuinfos
                              where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                              select ginfo;

            List<Gtuinfo> gtuinfoList = gtuinfolist.ToList<Gtuinfo>();
            return gtuinfoList;
        }

        public IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByUseridYear(int uId)
        {
            var timeNow = DateTime.Now;
            var timeBYear = new DateTime().AddYears(timeNow.Year - 1);
            var timeEYear = new DateTime().AddYears(timeNow.Year - 1).AddMonths(11).AddDays(30).AddHours(23).AddMinutes(59).AddSeconds(59);
            var gtuinfolist = from t in InternalSession.Linq<Task>()
                              from tt in t.Tasktimes
                              from tm in t.Taskgtuinfomappings
                              from ginfo in tm.Gtuinfos
                              where tm.UserId == uId && timeBYear < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeEYear
                              select ginfo;

            List<Gtuinfo> gtuinfoList = gtuinfolist.ToList<Gtuinfo>();
            return gtuinfoList;
        }

        public IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByUseridAll(int uId)
        {

            var gtuinfolist = from t in InternalSession.Linq<Task>()
                              from tt in t.Tasktimes
                              from tm in t.Taskgtuinfomappings
                              from ginfo in tm.Gtuinfos
                              where tm.UserId == uId
                              select ginfo;

            List<Gtuinfo> gtuinfoList = gtuinfolist.ToList<Gtuinfo>();
            return gtuinfoList;
        }

        public IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByCampaignidUserid(int cId, int uId)
        {
            var tasklist = (from c in InternalSession.Linq<Campaign>().Where(c => c.Id == cId)
                            from sm in c.SubMaps
                            from dm in sm.DistributionMaps
                            from t in dm.Tasks
                            select t).Distinct<Task>();


            List<Task> taskList = tasklist.ToList<Task>();
            List<Gtuinfo> gtuinfoList = new List<Gtuinfo>();
            List<Gtuinfo> gtuinfoListTem = new List<Gtuinfo>();
            foreach (Task t in tasklist)
            {
                var tasktimeS = (from tt in InternalSession.Linq<TaskTime>()
                                 where tt.TaskId == t.Id && tt.TimeType == 0
                                 orderby tt.Time
                                 select tt.Time).FirstOrDefault();
                var tasktimeE = (from tt in InternalSession.Linq<TaskTime>()
                                 where tt.TaskId == t.Id && tt.TimeType == 1
                                 orderby tt.Time descending
                                 select tt.Time).FirstOrDefault();


                var gtuinfolist = from ta in InternalSession.Linq<Task>()
                                  from tt in ta.Tasktimes
                                  from tm in ta.Taskgtuinfomappings
                                  from ginfo in tm.Gtuinfos
                                  where ta.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                                  select ginfo;

                gtuinfoListTem = gtuinfolist.ToList<Gtuinfo>();
                gtuinfoList.AddRange(gtuinfoListTem);


            }

            return gtuinfoList;
        }
        //public IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByCampaignidUseridYear(int cId, int uId)
        //{
        //    var tasklist = (from c in InternalSession.Linq<Campaign>().Where(c => c.Id == cId)
        //                    from sm in c.SubMaps
        //                    from dm in sm.DistributionMaps
        //                    from t in dm.Tasks
        //                    select t).Distinct<Task>();


        //    List<Task> taskList = tasklist.ToList<Task>();
        //    List<Gtuinfo> gtuinfoList = new List<Gtuinfo>();
        //    List<Gtuinfo> gtuinfoListTem = new List<Gtuinfo>();
        //    foreach (Task t in tasklist)
        //    {
        //        var tasktimeS = (from tt in InternalSession.Linq<TaskTime>()
        //                         where tt.TaskId == t.Id && tt.TimeType == 0
        //                         orderby tt.Time
        //                         select tt.Time).FirstOrDefault();
        //        var tasktimeE = (from tt in InternalSession.Linq<TaskTime>()
        //                         where tt.TaskId == t.Id && tt.TimeType == 1
        //                         orderby tt.Time descending
        //                         select tt.Time).FirstOrDefault();


        //        var gtuinfolist = from ta in InternalSession.Linq<Task>()
        //                          from tt in ta.Tasktimes
        //                          from tm in ta.Taskgtuinfomappings
        //                          from ginfo in tm.Gtuinfos
        //                          where ta.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
        //                          select ginfo;

        //        gtuinfoListTem = gtuinfolist.ToList<Gtuinfo>();
        //        gtuinfoList.AddRange(gtuinfoListTem);


        //    }

        //    return gtuinfoList;
        //}
        //public IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByCampaignidUseridAll(int cId, int uId)
        //{
        //    var tasklist = (from c in InternalSession.Linq<Campaign>().Where(c => c.Id == cId)
        //                    from sm in c.SubMaps
        //                    from dm in sm.DistributionMaps
        //                    from t in dm.Tasks
        //                    select t).Distinct<Task>();


        //    List<Task> taskList = tasklist.ToList<Task>();
        //    List<Gtuinfo> gtuinfoList = new List<Gtuinfo>();
        //    List<Gtuinfo> gtuinfoListTem = new List<Gtuinfo>();
        //    foreach (Task t in tasklist)
        //    {
        //        var tasktimeS = (from tt in InternalSession.Linq<TaskTime>()
        //                         where tt.TaskId == t.Id && tt.TimeType == 0
        //                         orderby tt.Time
        //                         select tt.Time).FirstOrDefault();
        //        var tasktimeE = (from tt in InternalSession.Linq<TaskTime>()
        //                         where tt.TaskId == t.Id && tt.TimeType == 1
        //                         orderby tt.Time descending
        //                         select tt.Time).FirstOrDefault();


        //        var gtuinfolist = from ta in InternalSession.Linq<Task>()
        //                          from tt in ta.Tasktimes
        //                          from tm in ta.Taskgtuinfomappings
        //                          from ginfo in tm.Gtuinfos
        //                          where ta.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
        //                          select ginfo;

        //        gtuinfoListTem = gtuinfolist.ToList<Gtuinfo>();
        //        gtuinfoList.AddRange(gtuinfoListTem);


        //    }

        //    return gtuinfoList;
        //}

        public IList<decimal> GetSpeedByTaskidUserid(int tId, int uId)
        {
            var tasktimeS = (from tt in InternalSession.Linq<TaskTime>()
                             where tt.TaskId == tId && tt.TimeType == 0
                             orderby tt.Time
                             select tt.Time).FirstOrDefault();
            var tasktimeE = (from tt in InternalSession.Linq<TaskTime>()
                             where tt.TaskId == tId && tt.TimeType == 1
                             orderby tt.Time descending
                             select tt.Time).FirstOrDefault();


            var tem = (from t in InternalSession.Linq<Task>()
                       from tt in t.Tasktimes
                       from tm in t.Taskgtuinfomappings
                       from ginfo in tm.Gtuinfos
                       where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                       select ginfo);

            var avgSpeed = tem.Count() == 0 ? 0 : tem.Average(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });

            var hignSpeed = tem.Count() == 0 ? 0 : tem.Max(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });

            var lowSpeed = tem.Count() == 0 ? 0 : tem.Min(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });


            //var avgSpeed = (from t in InternalSession.Linq<Task>()
            //                  from tt in t.Tasktimes
            //                  from tm in t.Taskgtuinfomappings
            //                  from ginfo in tm.Gtuinfos
            //                  where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
            //                  select ginfo).Average(ginfo=>ginfo.dwSpeed);
            //var hignSpeed = (from t in InternalSession.Linq<Task>()
            //                 from tt in t.Tasktimes
            //                 from tm in t.Taskgtuinfomappings
            //                 from ginfo in tm.Gtuinfos
            //                 where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
            //                 select ginfo).Max(ginfo => ginfo.dwSpeed);
            //var lowSpeed = (from t in InternalSession.Linq<Task>()
            //                from tt in t.Tasktimes
            //                from tm in t.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
            //                select ginfo).Min(ginfo => ginfo.dwSpeed);

            List<decimal> speedList = new List<decimal>();
            speedList.Add(avgSpeed);
            speedList.Add(hignSpeed);
            speedList.Add(lowSpeed);
            return speedList;
        }
        public IList<decimal> GetGroundByTaskidUserid(int tId, int uId)
        {
            var tasktimeS = (from tt in InternalSession.Linq<TaskTime>()
                             where tt.TaskId == tId && tt.TimeType == 0
                             orderby tt.Time
                             select tt.Time).FirstOrDefault();
            var tasktimeE = (from tt in InternalSession.Linq<TaskTime>()
                             where tt.TaskId == tId && tt.TimeType == 1
                             orderby tt.Time descending
                             select tt.Time).FirstOrDefault();

            var tem = (from t in InternalSession.Linq<Task>()
                       from tt in t.Tasktimes
                       from tm in t.Taskgtuinfomappings
                       from ginfo in tm.Gtuinfos
                       where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                       select ginfo);

            var avgDistance = tem.Count() == 0 ? 0 : tem.Average(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });

            var hignDistance = tem.Count() == 0 ? 0 : tem.Max(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });

            var lowDistance = tem.Count() == 0 ? 0 : tem.Min(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });


            //var avgDistance = (from t in InternalSession.Linq<Task>()
            //                from tt in t.Tasktimes
            //                from tm in t.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
            //                select ginfo).Average(ginfo => ginfo.Distance);
            //var hignDistance = (from t in InternalSession.Linq<Task>()
            //                 from tt in t.Tasktimes
            //                 from tm in t.Taskgtuinfomappings
            //                 from ginfo in tm.Gtuinfos
            //                 where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
            //                 select ginfo).Max(ginfo => ginfo.Distance);
            //var lowDistance = (from t in InternalSession.Linq<Task>()
            //                from tt in t.Tasktimes
            //                from tm in t.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
            //                select ginfo).Min(ginfo => ginfo.Distance);

            List<decimal> DistanceList = new List<decimal>();
            DistanceList.Add(avgDistance);
            DistanceList.Add(hignDistance);
            DistanceList.Add(lowDistance);
            return DistanceList;
        }
        public IList<double> GetStopByTaskidUserid(int tId, int uId)
        {
            var tasktimeS = (from tt in InternalSession.Linq<TaskTime>()
                             where tt.TaskId == tId && tt.TimeType == 0
                             orderby tt.Time
                             select tt.Time).FirstOrDefault();
            var tasktimeE = (from tt in InternalSession.Linq<TaskTime>()
                             where tt.TaskId == tId && tt.TimeType == 1
                             orderby tt.Time descending
                             select tt.Time).FirstOrDefault();

            var tem = (from t in InternalSession.Linq<Task>()
                       from tt in t.Tasktimes
                       from tm in t.Taskgtuinfomappings
                       from ginfo in tm.Gtuinfos
                       where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE && ginfo.Status == 1
                       select ginfo);

            var avgStop = tem.Count() == 0 ? 0 : tem.Count();

            var hignStop = tem.Count() == 0 ? 0 : tem.Count();

            var lowStop = tem.Count() == 0 ? 0 : tem.Count();


            //var avgStop = (from t in InternalSession.Linq<Task>()
            //                from tt in t.Tasktimes
            //                from tm in t.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
            //                select ginfo).Count(ginfo => ginfo.Status == 1);
            //var hignStop = (from t in InternalSession.Linq<Task>()
            //                 from tt in t.Tasktimes
            //                 from tm in t.Taskgtuinfomappings
            //                 from ginfo in tm.Gtuinfos
            //                 where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
            //                 select ginfo).Count(ginfo => ginfo.Status == 1);
            //var lowStop = (from t in InternalSession.Linq<Task>()
            //                from tt in t.Tasktimes
            //                from tm in t.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where t.Id == tId && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
            //                select ginfo).Count(ginfo => ginfo.Status == 1);

            List<double> stopList = new List<double>();
            stopList.Add(avgStop);
            stopList.Add(hignStop);
            stopList.Add(lowStop);
            return stopList;
        }
        public IList<decimal> GetSpeedByCampaignidUserid(int cId, int uId)
        {
            //decimal aa = InternalSession.Linq<Task>().Average(n => (decimal)n.Id);

            //var bb = InternalSession.Linq<Task>().Average(delegate(Task t) { return Convert.ToDecimal(t.Id); });

            //decimal aave = (from task in InternalSession.Linq<Task>()
            //            select task.Id).Average();

            //double bb = (from task in InternalSession.Linq<Task>()
            //             select task.Id).Average();


            //var mmax = (from task in InternalSession.Linq<Task>()
            //            select task.Id).Max(n => (decimal)n);


            var tasklist = (from c in InternalSession.Linq<Campaign>().Where(c => c.Id == cId)
                            from sm in c.SubMaps
                            from dm in sm.DistributionMaps
                            from t in dm.Tasks
                            select t).Distinct<Task>();


            List<Task> taskList = tasklist.ToList<Task>();
            List<Gtuinfo> gtuinfoList = new List<Gtuinfo>();
            List<Gtuinfo> gtuinfoListTem = new List<Gtuinfo>();
            var i = 0;
            List<decimal> speedList = new List<decimal>();
            foreach (Task t in tasklist)
            {
                var tasktimeS = (from tt in InternalSession.Linq<TaskTime>()
                                 where tt.TaskId == t.Id && tt.TimeType == 0
                                 orderby tt.Time
                                 select tt.Time).FirstOrDefault();
                var tasktimeE = (from tt in InternalSession.Linq<TaskTime>()
                                 where tt.TaskId == t.Id && tt.TimeType == 1
                                 orderby tt.Time descending
                                 select tt.Time).FirstOrDefault();


                var tem = (from task in InternalSession.Linq<Task>()
                           from tt in task.Tasktimes
                           from tm in task.Taskgtuinfomappings
                           from ginfo in tm.Gtuinfos
                           where task.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                           select ginfo);

                var avgSpeed = tem.Count() == 0 ? 0 : tem.Average(delegate(Gtuinfo f)
                { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });

                var hignSpeed = tem.Count() == 0 ? 0 : tem.Max(delegate(Gtuinfo f)
                { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });

                var lowSpeed = tem.Count() == 0 ? 0 : tem.Min(delegate(Gtuinfo f)
                { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });

                //var a = avgSpeedGtu.Average(delegate(Gtuinfo t) { return Convert.ToDecimal(t.dwSpeed); });
                //var avgSpeed = avgSpeedGtu.Average(ginfo => ginfo.dwSpeed);
                //var avgSpeed = avgSpeedGtu.Select(ginfo => ginfo.dwSpeed).Average();

                if (i == 0)
                {
                    speedList.Add(avgSpeed);
                    speedList.Add(hignSpeed);
                    speedList.Add(lowSpeed);
                }
                else
                {
                    speedList[0] = (speedList[0] + avgSpeed) / 2;
                    if (hignSpeed > speedList[1]) { speedList[1] = hignSpeed; }
                    if (lowSpeed < speedList[2]) { speedList[2] = lowSpeed; }
                }
                i++;
            }
            return speedList;
        }

        public IList<decimal> GetGroundByCampaignidUserid(int cId, int uId)
        {
            var tasklist = (from c in InternalSession.Linq<Campaign>().Where(c => c.Id == cId)
                            from sm in c.SubMaps
                            from dm in sm.DistributionMaps
                            from t in dm.Tasks
                            select t).Distinct<Task>();


            List<Task> taskList = tasklist.ToList<Task>();
            List<Gtuinfo> gtuinfoList = new List<Gtuinfo>();
            List<Gtuinfo> gtuinfoListTem = new List<Gtuinfo>();
            var i = 0;
            List<decimal> groundList = new List<decimal>();
            foreach (Task t in tasklist)
            {
                var tasktimeS = (from tt in InternalSession.Linq<TaskTime>()
                                 where tt.TaskId == t.Id && tt.TimeType == 0
                                 orderby tt.Time
                                 select tt.Time).FirstOrDefault();
                var tasktimeE = (from tt in InternalSession.Linq<TaskTime>()
                                 where tt.TaskId == t.Id && tt.TimeType == 1
                                 orderby tt.Time descending
                                 select tt.Time).FirstOrDefault();


                var tem = (from task in InternalSession.Linq<Task>()
                           from tt in task.Tasktimes
                           from tm in task.Taskgtuinfomappings
                           from ginfo in tm.Gtuinfos
                           where task.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                           select ginfo);

                var avgGround = tem.Count() == 0 ? 0 : tem.Average(delegate(Gtuinfo f)
                { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });

                var hignGround = tem.Count() == 0 ? 0 : tem.Max(delegate(Gtuinfo f)
                { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });

                var lowGround = tem.Count() == 0 ? 0 : tem.Min(delegate(Gtuinfo f)
                { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });



                //var avgGround = (from task in InternalSession.Linq<Task>()
                //                from tt in task.Tasktimes
                //                from tm in task.Taskgtuinfomappings
                //                from ginfo in tm.Gtuinfos
                //                where task.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                //                select ginfo).Average(ginfo => ginfo.Distance);
                //var hignGround = (from task in InternalSession.Linq<Task>()
                //                 from tt in task.Tasktimes
                //                 from tm in task.Taskgtuinfomappings
                //                 from ginfo in tm.Gtuinfos
                //                 where task.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                //                 select ginfo).Max(ginfo => ginfo.Distance);
                //var lowGround = (from task in InternalSession.Linq<Task>()
                //                from tt in task.Tasktimes
                //                from tm in task.Taskgtuinfomappings
                //                from ginfo in tm.Gtuinfos
                //                where task.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                //                select ginfo).Min(ginfo => ginfo.Distance);

                if (i == 0)
                {
                    groundList.Add(avgGround);
                    groundList.Add(hignGround);
                    groundList.Add(lowGround);
                }
                else
                {
                    groundList[0] = (groundList[0] + avgGround) / 2;
                    if (hignGround > groundList[1]) { groundList[1] = hignGround; }
                    if (lowGround < groundList[2]) { groundList[2] = lowGround; }
                }
                i++;
            }
            return groundList;
        }
        public IList<double> GetStopByCampaignidUserid(int cId, int uId)
        {
            var tasklist = (from c in InternalSession.Linq<Campaign>().Where(c => c.Id == cId)
                            from sm in c.SubMaps
                            from dm in sm.DistributionMaps
                            from t in dm.Tasks
                            select t).Distinct<Task>();


            List<Task> taskList = tasklist.ToList<Task>();
            List<Gtuinfo> gtuinfoList = new List<Gtuinfo>();
            List<Gtuinfo> gtuinfoListTem = new List<Gtuinfo>();
            var i = 0;
            List<double> stopList = new List<double>();
            foreach (Task t in tasklist)
            {
                var tasktimeS = (from tt in InternalSession.Linq<TaskTime>()
                                 where tt.TaskId == t.Id && tt.TimeType == 0
                                 orderby tt.Time
                                 select tt.Time).FirstOrDefault();
                var tasktimeE = (from tt in InternalSession.Linq<TaskTime>()
                                 where tt.TaskId == t.Id && tt.TimeType == 1
                                 orderby tt.Time descending
                                 select tt.Time).FirstOrDefault();



                var tem = (from task in InternalSession.Linq<Task>()
                           from tt in task.Tasktimes
                           from tm in task.Taskgtuinfomappings
                           from ginfo in tm.Gtuinfos
                           where task.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE && ginfo.Status == 1
                           select ginfo);

                var avgStop = tem.Count() == 0 ? 0 : tem.Count();
                var hignStop = tem.Count() == 0 ? 0 : tem.Count();
                var lowStop = tem.Count() == 0 ? 0 : tem.Count();

                //var hignGround = tem.Count() == 0 ? 0 : tem.Max(delegate(Gtuinfo f)
                //{ if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });

                //var lowGround = tem.Count() == 0 ? 0 : tem.Min(delegate(Gtuinfo f)
                //{ if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });




                //var avgStop = (from task in InternalSession.Linq<Task>()
                //                from tt in task.Tasktimes
                //                from tm in task.Taskgtuinfomappings
                //                from ginfo in tm.Gtuinfos
                //                where task.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                //                select ginfo).Count(ginfo => ginfo.Status == 1);
                //var hignStop = (from task in InternalSession.Linq<Task>()
                //                 from tt in task.Tasktimes
                //                 from tm in task.Taskgtuinfomappings
                //                 from ginfo in tm.Gtuinfos
                //                 where task.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                //                 select ginfo).Count(ginfo => ginfo.Status == 1);
                //var lowStop = (from task in InternalSession.Linq<Task>()
                //                from tt in task.Tasktimes
                //                from tm in task.Taskgtuinfomappings
                //                from ginfo in tm.Gtuinfos
                //                where task.Id == t.Id && tm.UserId == uId && tasktimeS < ginfo.dtReceivedTime && ginfo.dtReceivedTime < tasktimeE
                //                select ginfo).Count(ginfo => ginfo.Status == 1);

                if (i == 0)
                {
                    stopList.Add(avgStop);
                    stopList.Add(hignStop);
                    stopList.Add(lowStop);
                }
                else
                {
                    stopList[0] = (stopList[0] + avgStop) / 2;
                    if (hignStop > stopList[1]) { stopList[1] = hignStop; }
                    if (lowStop < stopList[2]) { stopList[2] = lowStop; }
                }
                i++;
            }
            return stopList;
        }
        public IList<decimal> GetSpeedByUseridYear(int uId)
        {
            var timeNow = DateTime.Now;
            var timeYearB = new DateTime().AddYears(timeNow.Year - 1);
            var timeYearE = new DateTime().AddYears(timeNow.Year - 1).AddMonths(11).AddDays(30).AddHours(23).AddMinutes(59).AddSeconds(59);



            var tem = (from t in InternalSession.Linq<Task>()
                       from tt in t.Tasktimes
                       from tm in t.Taskgtuinfomappings
                       from ginfo in tm.Gtuinfos
                       where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE && ginfo.Status == 1
                       select ginfo);

            var avgSpeed = tem.Count() == 0 ? 0 : tem.Average(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });

            var hignSpeed = tem.Count() == 0 ? 0 : tem.Max(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });

            var lowSpeed = tem.Count() == 0 ? 0 : tem.Min(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });




            //var avgSpeed = (from t in InternalSession.Linq<Task>()
            //               from tt in t.Tasktimes
            //               from tm in t.Taskgtuinfomappings
            //               from ginfo in tm.Gtuinfos
            //               where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE
            //               select ginfo).Average(ginfo => ginfo.dwSpeed);
            //var hignSpeed = (from task in InternalSession.Linq<Task>()
            //                 from tt in task.Tasktimes
            //                 from tm in task.Taskgtuinfomappings
            //                 from ginfo in tm.Gtuinfos
            //                 where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE
            //                 select ginfo).Max(ginfo => ginfo.dwSpeed);
            //var lowSpeed = (from task in InternalSession.Linq<Task>()
            //                from tt in task.Tasktimes
            //                from tm in task.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE
            //                select ginfo).Min(ginfo => ginfo.dwSpeed);

            List<decimal> speedList = new List<decimal>();
            speedList.Add(avgSpeed);
            speedList.Add(hignSpeed);
            speedList.Add(lowSpeed);
            return speedList;
        }
        public IList<decimal> GetGroundByUseridYear(int uId)
        {
            var timeNow = DateTime.Now;
            var timeYearB = new DateTime().AddYears(timeNow.Year - 1);
            var timeYearE = new DateTime().AddYears(timeNow.Year - 1).AddMonths(11).AddDays(30).AddHours(23).AddMinutes(59).AddSeconds(59);


            var tem = (from t in InternalSession.Linq<Task>()
                       from tt in t.Tasktimes
                       from tm in t.Taskgtuinfomappings
                       from ginfo in tm.Gtuinfos
                       where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE
                       select ginfo);

            var avgGround = tem.Count() == 0 ? 0 : tem.Average(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });

            var hignGround = tem.Count() == 0 ? 0 : tem.Max(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });

            var lowGround = tem.Count() == 0 ? 0 : tem.Min(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });



            //var avgGround = (from t in InternalSession.Linq<Task>()
            //                from tt in t.Tasktimes
            //                from tm in t.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE
            //                select ginfo).Average(ginfo => ginfo.Distance);
            //var hignGround = (from task in InternalSession.Linq<Task>()
            //                 from tt in task.Tasktimes
            //                 from tm in task.Taskgtuinfomappings
            //                 from ginfo in tm.Gtuinfos
            //                 where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE
            //                 select ginfo).Max(ginfo => ginfo.Distance);
            //var lowGround = (from task in InternalSession.Linq<Task>()
            //                from tt in task.Tasktimes
            //                from tm in task.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE
            //                select ginfo).Min(ginfo => ginfo.Distance);

            List<decimal> groundList = new List<decimal>();
            groundList.Add(avgGround);
            groundList.Add(hignGround);
            groundList.Add(lowGround);
            return groundList;
        }
        public IList<double> GetStopByUseridYear(int uId)
        {
            var timeNow = DateTime.Now;
            var timeYearB = new DateTime().AddYears(timeNow.Year - 1);
            var timeYearE = new DateTime().AddYears(timeNow.Year - 1).AddMonths(11).AddDays(30).AddHours(23).AddMinutes(59).AddSeconds(59);

            var tem = (from t in InternalSession.Linq<Task>()
                       from tt in t.Tasktimes
                       from tm in t.Taskgtuinfomappings
                       from ginfo in tm.Gtuinfos
                       where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE && ginfo.Status == 1
                       select ginfo);

            var avgStop = tem.Count() == 0 ? 0 : tem.Count();
            var hignStop = tem.Count() == 0 ? 0 : tem.Count();
            var lowStop = tem.Count() == 0 ? 0 : tem.Count();




            //var avgStop = (from t in InternalSession.Linq<Task>()
            //                from tt in t.Tasktimes
            //                from tm in t.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE
            //                select ginfo).Count(ginfo => ginfo.Status == 1);
            //var hignStop = (from task in InternalSession.Linq<Task>()
            //                 from tt in task.Tasktimes
            //                 from tm in task.Taskgtuinfomappings
            //                 from ginfo in tm.Gtuinfos
            //                 where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE
            //                 select ginfo).Count(ginfo => ginfo.Status == 1);
            //var lowStop = (from task in InternalSession.Linq<Task>()
            //                from tt in task.Tasktimes
            //                from tm in task.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where tm.UserId == uId && timeYearB < ginfo.dtReceivedTime && ginfo.dtReceivedTime < timeYearE
            //                select ginfo).Count(ginfo => ginfo.Status == 1);

            List<double> stopList = new List<double>();
            stopList.Add(avgStop);
            stopList.Add(hignStop);
            stopList.Add(lowStop);
            return stopList;
        }

        public IList<decimal> GetSpeedByUseridAll(int uId)
        {

            var tem = (from t in InternalSession.Linq<Task>()
                       from tt in t.Tasktimes
                       from tm in t.Taskgtuinfomappings
                       from ginfo in tm.Gtuinfos
                       where tm.UserId == uId
                       select ginfo);

            var avgSpeed = tem.Count() == 0 ? 0 : tem.Average(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });

            var hignSpeed = tem.Count() == 0 ? 0 : tem.Max(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });

            var lowSpeed = tem.Count() == 0 ? 0 : tem.Min(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.dwSpeed); } });




            //var avgSpeed = (from t in InternalSession.Linq<Task>()
            //                from tt in t.Tasktimes
            //                from tm in t.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where tm.UserId == uId
            //                select ginfo).Average(ginfo => ginfo.dwSpeed);
            //var hignSpeed = (from task in InternalSession.Linq<Task>()
            //                 from tt in task.Tasktimes
            //                 from tm in task.Taskgtuinfomappings
            //                 from ginfo in tm.Gtuinfos
            //                 where tm.UserId == uId
            //                 select ginfo).Max(ginfo => ginfo.dwSpeed);
            //var lowSpeed = (from task in InternalSession.Linq<Task>()
            //                from tt in task.Tasktimes
            //                from tm in task.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where tm.UserId == uId
            //                select ginfo).Min(ginfo => ginfo.dwSpeed);

            List<decimal> speedList = new List<decimal>();
            speedList.Add(avgSpeed);
            speedList.Add(hignSpeed);
            speedList.Add(lowSpeed);
            return speedList;
        }
        public IList<decimal> GetGroundByUseridAll(int uId)
        {

            var tem = (from t in InternalSession.Linq<Task>()
                       from tt in t.Tasktimes
                       from tm in t.Taskgtuinfomappings
                       from ginfo in tm.Gtuinfos
                       where tm.UserId == uId
                       select ginfo);

            var avgGround = tem.Count() == 0 ? 0 : tem.Average(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });

            var hignGround = tem.Count() == 0 ? 0 : tem.Max(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });

            var lowGround = tem.Count() == 0 ? 0 : tem.Min(delegate(Gtuinfo f)
            { if (f == null) { return 0; } else { return Convert.ToDecimal(f.Distance); } });


            //var avgGround = (from t in InternalSession.Linq<Task>()
            //                 from tt in t.Tasktimes
            //                 from tm in t.Taskgtuinfomappings
            //                 from ginfo in tm.Gtuinfos
            //                 where tm.UserId == uId
            //                 select ginfo).Average(ginfo => ginfo.Distance);
            //var hignGround = (from task in InternalSession.Linq<Task>()
            //                  from tt in task.Tasktimes
            //                  from tm in task.Taskgtuinfomappings
            //                  from ginfo in tm.Gtuinfos
            //                  where tm.UserId == uId
            //                  select ginfo).Max(ginfo => ginfo.Distance);
            //var lowGround = (from task in InternalSession.Linq<Task>()
            //                 from tt in task.Tasktimes
            //                 from tm in task.Taskgtuinfomappings
            //                 from ginfo in tm.Gtuinfos
            //                 where tm.UserId == uId
            //                 select ginfo).Min(ginfo => ginfo.Distance);

            List<decimal> groundList = new List<decimal>();
            groundList.Add(avgGround);
            groundList.Add(hignGround);
            groundList.Add(lowGround);
            return groundList;
        }
        public IList<double> GetStopByUseridAll(int uId)
        {
            var tem = (from t in InternalSession.Linq<Task>()
                       from tt in t.Tasktimes
                       from tm in t.Taskgtuinfomappings
                       from ginfo in tm.Gtuinfos
                       where tm.UserId == uId && ginfo.Status == 1
                       select ginfo);

            var avgStop = tem.Count() == 0 ? 0 : tem.Count();
            var hignStop = tem.Count() == 0 ? 0 : tem.Count();
            var lowStop = tem.Count() == 0 ? 0 : tem.Count();



            //var avgStop = (from t in InternalSession.Linq<Task>()
            //                from tt in t.Tasktimes
            //                from tm in t.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where tm.UserId == uId
            //                select ginfo).Count(ginfo => ginfo.Status==1);
            //var hignStop = (from task in InternalSession.Linq<Task>()
            //                 from tt in task.Tasktimes
            //                 from tm in task.Taskgtuinfomappings
            //                 from ginfo in tm.Gtuinfos
            //                 where tm.UserId == uId
            //                 select ginfo).Count(ginfo => ginfo.Status == 1);
            //var lowStop = (from task in InternalSession.Linq<Task>()
            //                from tt in task.Tasktimes
            //                from tm in task.Taskgtuinfomappings
            //                from ginfo in tm.Gtuinfos
            //                where tm.UserId == uId
            //                select ginfo).Count(ginfo => ginfo.Status == 1);

            List<double> stopList = new List<double>();
            stopList.Add(avgStop);
            stopList.Add(hignStop);
            stopList.Add(lowStop);
            return stopList;
        }

        public IList<string> GetUserNameById(int userId)
        {
            const string queryFormat = " select distinct UserName from User where Id = :userId";
            return InternalSession.CreateQuery(queryFormat)
                .SetInt32("userId", userId)
                .List<string>();
        }

    }
}
