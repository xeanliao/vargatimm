using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
{
    public class RadiusesConfig : EntityTypeConfiguration<Radiuses>
    {
        public RadiusesConfig()
        {
            HasKey(i => i.Id).ToTable("radiuses");
        }
    }
}
