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
    public class GroupRepository: RepositoryBase, GPS.DataLayer.IGroupRepository
    {
        public GroupRepository() { }

        public GroupRepository(ISession session) : base(session) { }

        public IList<Group> GetAllGroups()
        {
            return InternalSession.Linq<Group>().ToList<Group>();
        }

        public Group GetGroup(int gid)
        {
            return InternalSession.Get<Group>(gid);
        }

        public IList<Group> GetGroupList(int[] gids)
        {
            //var groups = from g in  InternalSession.Linq<Group>()
            //             where g.Id;
            //return groups;
           return  InternalSession.CreateCriteria(typeof(Group)).Add(Expression.In("Id", gids)).List<Group>();
        }

        public Group GetGroup(string gname)
        {
            return InternalSession.Linq<Group>().Where(g => g.Name == gname).FirstOrDefault();
        }

        public Group GetGroupForValidate(Group group)
        {
            return InternalSession.Linq<Group>().Where(u => u.Name == group.Name && u.Id!=group.Id).FirstOrDefault();
        }

        public Group AddGroup(Group group)
        {
            base.Insert(group);
            return group;
        }

        public void DeleteGroup(int groupID)
        {
            Group group = this.GetGroup(groupID);
            base.Delete(group);
        }

        public Group UpdateGroup(Group group)
        {
            Group u = this.GetGroup(group.Id);
            if (u != null)
            {
                u.Name = group.Name;
                u.Privileges = group.Privileges;
            }
            base.Update(u);
            return u;
        }

    }
}
