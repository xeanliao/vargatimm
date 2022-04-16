using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class TractConfig : EntityTypeConfiguration<Tract>
    {
        public TractConfig()
        {
            ToTable("tracts");
            HasKey(i => i.Id);
            Property(i => i._IsEnabled).HasColumnName("IsEnabled");
            Ignore(i => i.IsEnabled);
            Ignore(i => i.HasMultipleParts);

            //HasMany(i => i.Locations)
            //    .WithRequired()
            //    .HasForeignKey(i => i.TractId);
        }
    }
}
