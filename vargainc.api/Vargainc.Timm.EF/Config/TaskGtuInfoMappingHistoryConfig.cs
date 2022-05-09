using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class TaskGtuInfoMappingHistoryConfig : EntityTypeConfiguration<TaskGtuInfoMappingHistory>
    {
        public TaskGtuInfoMappingHistoryConfig()
        {
            HasKey(i => i.HistId).ToTable("taskgtuinfomappinghistory");

            HasMany(i => i.GtuInfos)
                .WithRequired(i => i.TaskGtuInfoMappingHistory)
                .HasForeignKey(i => i.TaskgtuinfoId)
                .WillCascadeOnDelete(false);

            HasRequired(i => i.GTU).WithMany().HasForeignKey(i => i.GTUId);
            HasOptional(i => i.Auditor).WithMany().HasForeignKey(i => i.UserId);
        }
    }
}
