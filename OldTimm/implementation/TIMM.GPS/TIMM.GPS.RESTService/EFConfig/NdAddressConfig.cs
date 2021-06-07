using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    class NdAddressConfig : EntityTypeConfiguration<NdAddress>
    {
        public NdAddressConfig()
        {
            HasKey(i => i.Id).ToTable("ndaddresses");

            HasMany(i => i.NdAddressCoordinates)
                .WithRequired()
                .HasForeignKey(i => i.NdAddressId);

            Ignore(i => i.Locations);
        }
    }
}