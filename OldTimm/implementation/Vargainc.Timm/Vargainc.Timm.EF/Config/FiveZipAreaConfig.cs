using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class FiveZipAreaConfig : EntityTypeConfiguration<FiveZipArea>
    {
        public FiveZipAreaConfig()
        {
            HasKey(i => i.Id).ToTable("fivezipareas");
            Ignore(i => i.HasMultipleParts);
            Property(i => i._IsEnabled).HasColumnName("IsEnabled");
            Ignore(i => i.IsEnabled);
            HasMany(i => i.Locations)
                .WithRequired()
                .HasForeignKey(i => i.FiveZipAreaId);
        }
    }
}
