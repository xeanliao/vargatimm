using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class StatusInfoConfig : EntityTypeConfiguration<StatusInfo>
    {
        public StatusInfoConfig()
        {
            HasKey(i => i.Id);
            ToTable("campaignusermappings");

            HasRequired(i => i.User).WithMany().HasForeignKey(i => i.UserId);
        }
    }
}
