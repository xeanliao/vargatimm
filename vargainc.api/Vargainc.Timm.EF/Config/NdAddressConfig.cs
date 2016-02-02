using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
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