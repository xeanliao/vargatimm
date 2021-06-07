using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    class NdAddressCoordinateConfig : EntityTypeConfiguration<NdAddressCoordinate>
    {
        public NdAddressCoordinateConfig()
        {
            HasKey(i => i.Id).ToTable("ndaddresscoordinates");
        }
    }
}
