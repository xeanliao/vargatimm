using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
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


            //database datetime type is datetime2(0) have some problem with entity framework
            HasMany(i => i.TaskTimes)
                .WithOptional()
                .HasForeignKey(i => i.TaskId);


        }
    }
}