﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    class CampaignBlockGroupImportedConfig : EntityTypeConfiguration<CampaignBlockGroupImported>
    {
        public CampaignBlockGroupImportedConfig()
        {
            ToTable("campaignblockgroupimported");
            HasKey(i => i.Id);
            HasRequired(i => i.BlockGroup)
                .WithMany()
                .HasForeignKey(i => i.BlockGroupId);
        }
    }
}
