using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
{
    class CampaignTractImportedConfig : EntityTypeConfiguration<CampaignTractImported>
    {
        public CampaignTractImportedConfig()
        {
            ToTable("campaigntractimported");
            HasKey(i => i.Id);
            HasRequired(i => i.Tract)
                .WithMany()
                .HasForeignKey(i => i.TractId);
        }
    }
}
