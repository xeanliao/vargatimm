using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class DistributionJobConfig : EntityTypeConfiguration<DistributionJob>
    {
        public DistributionJobConfig()
        {
            HasKey(i => i.Id).ToTable("DistributionJobs");
        }
        
    }
}
