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
    public class PrivilegeRepository : RepositoryBase, GPS.DataLayer.IPrivilegeRepository
    {
        public PrivilegeRepository() { }

        public PrivilegeRepository(ISession session) : base(session) { }

        public IList<Privilege> GetAllPrivileges()
        {
            return InternalSession.Linq<Privilege>().ToList<Privilege>();
        }

        public IList<Privilege> GetPrivilegeList(int[] pids)
        {
            //const string queryFormat = "select plist from Privilege plist where plist.Id in (:pids)";
            //return InternalSession.CreateQuery(queryFormat).SetParameterList("pids", pids).List<Privilege>();
            return InternalSession.CreateCriteria(typeof(Privilege)).Add(Expression.In("Id", pids)).List<Privilege>();
        }
    }
}
