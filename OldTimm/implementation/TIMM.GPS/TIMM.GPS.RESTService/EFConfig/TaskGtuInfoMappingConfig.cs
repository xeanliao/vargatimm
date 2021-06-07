using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    class TaskGtuInfoMappingConfig : EntityTypeConfiguration<TaskGtuInfoMapping>
    {
        public TaskGtuInfoMappingConfig()
        {
            HasKey(i => i.Id).ToTable("taskgtuinfomapping");

            HasMany(i => i.GtuInfos)
                .WithRequired(i => i.TaskGtuInfoMapping)
                .HasForeignKey(i => i.TaskgtuinfoId)
                .WillCascadeOnDelete(false);
        }
    }
}
