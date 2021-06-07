using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
{
    class TaskConfig : EntityTypeConfiguration<Task>
    {
        public TaskConfig()
        {
            HasKey(i => i.Id).ToTable("task");
            Property(i => i.DistributionMapId).HasColumnName("DmId");

            HasRequired(i => i.Auditor)
                .WithMany()
                .HasForeignKey(i => i.AuditorId);
        }
    }
}