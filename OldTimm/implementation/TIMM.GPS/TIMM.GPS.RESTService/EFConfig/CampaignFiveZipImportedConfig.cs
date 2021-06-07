using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    class CampaignFiveZipImportedConfig : EntityTypeConfiguration<CampaignFiveZipImported>
    {
        public CampaignFiveZipImportedConfig()
        {
            ToTable("campaignfivezipimported");
            HasKey(i => i.Id);
            HasRequired(i => i.FiveZipArea)
                .WithMany()
                .HasForeignKey(i => i.FiveZipAreaId);
            Ignore(i => i.IsPartModified);
        }
    }
}
