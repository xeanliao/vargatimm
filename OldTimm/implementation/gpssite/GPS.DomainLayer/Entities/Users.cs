using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using GPS.DataLayer.ValueObjects;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class User
    {
        public User() { }

        public virtual int Id { get; set; }

        public virtual bool Enabled { get; set; }

        public virtual string FullName { get; set; }

        public virtual string Password { get; set; }

        public virtual string UserCode { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Email { get; set; }

        public virtual string Token { get; set; }

        public virtual DateTime? LastLoginTime { get; set; }

        public virtual UserRoles Role { get; set; }

        //public virtual IDictionary<Campaign, StatusInfo> Campaigns { get; set; }

        public virtual IList<Group> Groups  { get; set; }
        //public virtual IList<Taskgtuinfomapping> Taskgtuinfomappings { get; set; }
        //public virtual IList<Task> Tasks { get; set; }
    }

    [Serializable]
    public class StatusInfo
    {
        public StatusInfo() { }

        public StatusInfo(int status)
        {
            this.Status = status;            
        }

        public int Status { get; set; }
       
    }
}
