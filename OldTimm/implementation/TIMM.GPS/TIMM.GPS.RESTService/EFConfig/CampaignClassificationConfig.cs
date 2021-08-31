﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
{
    class CampaignClassificationConfig : EntityTypeConfiguration<CampaignClassification>
    {
        public CampaignClassificationConfig()
        {
            HasKey(i => i.Id).ToTable("campaignclassifications");
        }
    }
}