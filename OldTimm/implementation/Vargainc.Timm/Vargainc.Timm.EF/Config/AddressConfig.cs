using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
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
