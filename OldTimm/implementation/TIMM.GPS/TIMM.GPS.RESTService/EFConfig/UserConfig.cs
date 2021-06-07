using System;
using System.Collections.Generic;
using System.Linq;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
{
    class UserConfig : EntityTypeConfiguration<User>
    {
        /// <summary>
        /// Initializes a new instance of the UserConfig class.
        /// </summary>
        public UserConfig()
        {
            HasKey(i => i.Id).ToTable("users");

            HasMany(i => i.Status).WithRequired(i => i.User).HasForeignKey(i => i.UserId);
        }
    }
}
