using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    public class GroupConfig : EntityTypeConfiguration<Group>
    {
        public GroupConfig()
        {
            HasKey(i => i.Id).ToTable("groups");

            HasMany(i => i.Users)
                .WithMany(i => i.Groups)
                .Map(m => m.MapLeftKey("GroupId").MapRightKey("UserId").ToTable("usergroupmappings"));
        }
    }
}
