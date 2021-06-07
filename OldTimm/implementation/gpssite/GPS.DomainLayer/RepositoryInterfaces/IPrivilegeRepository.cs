using System;
using System.Collections.Generic;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public interface IPrivilegeRepository
    {
        IList<Privilege> GetAllPrivileges();
        IList<Privilege> GetPrivilegeList(int[] pids);
    }
}
