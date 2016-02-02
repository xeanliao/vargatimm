using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class CampaignBackupConfig : EntityTypeConfiguration<CampaignBackup>
    {
        /// <summary>
        /// Initializes a new instance of the CampaignBackupConfig class.
        /// </summary>
        public CampaignBackupConfig()
        {
            HasKey(i => i.Id).ToTable("campaigns_backup");
            Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}
