using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;

namespace TIMM.GPS.RESTService.EFConfig
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
