using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    public class DistributionMapRecordConfig : EntityTypeConfiguration<DistributionMapRecord>
    {
        public DistributionMapRecordConfig()
        {
            HasKey(i => i.Id).ToTable("distributionmaprecords");
        }
    }
}
