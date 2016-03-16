using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
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

            HasRequired(i => i.GTU).WithMany().HasForeignKey(i => i.GTUId);
            HasOptional(i => i.Auditor).WithMany().HasForeignKey(i => i.UserId);
        }
    }
}
