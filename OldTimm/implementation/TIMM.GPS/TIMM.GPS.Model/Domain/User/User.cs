using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TIMM.GPS.Model
{
    public class User
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        [DefaultValue(false)]
        public bool Enabled { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public User()
        {
            Role = new UserRoles();
            Groups = new List<Group>();
            Status = new List<StatusInfo>();
        }

        public UserRoles Role { get; set; }

        public List<Group> Groups { get; set; }

        public virtual List<StatusInfo> Status { get; set; }
    }
}
