using System;
using System.Collections.Generic;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public interface IGroupRepository
    {
        Group AddGroup(Group group);
        void DeleteGroup(int id);
        IList<Group> GetAllGroups();
        Group GetGroup(int id);
        Group GetGroupForValidate(Group group);
        Group UpdateGroup(Group group);
        IList<Group> GetGroupList(int[] gids);
    }
}