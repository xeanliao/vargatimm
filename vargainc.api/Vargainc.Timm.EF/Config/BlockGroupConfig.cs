using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class BlockGroupConfig : EntityTypeConfiguration<BlockGroup>
    {
        public BlockGroupConfig()
        {
            HasKey(i => i.Id).ToTable("blockgroups");
            Property(i => i._IsEnabled).HasColumnName("IsEnabled");
            Ignore(i => i.IsEnabled);
            Ignore(i => i.HasMultipleParts);

            HasMany(i => i.Locations)
                .WithRequired()
                .HasForeignKey(i => i.BlockGroupId);
        }
    }
}
