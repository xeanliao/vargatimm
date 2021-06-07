using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    class FiveZipCoordinateConfig : EntityTypeConfiguration<FiveZipCoordinate>
    {
        public FiveZipCoordinateConfig()
        {
            HasKey(i => i.Id).ToTable("fivezipareacoordinates");
        }
    }
}
