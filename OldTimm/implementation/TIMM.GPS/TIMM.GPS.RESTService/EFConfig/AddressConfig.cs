using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
{
    public class AddressConfig : EntityTypeConfiguration<Address>
    {
        public AddressConfig()
        {
            HasKey(i => i.Id).ToTable("addresses");

            HasMany(i => i.Radiuses)
                .WithRequired(i => i.Address)
                .HasForeignKey(i => i.AddressId)
                .WillCascadeOnDelete(false);

            Property(i => i.AddressName).HasColumnName("Address");
        }
    }
}
