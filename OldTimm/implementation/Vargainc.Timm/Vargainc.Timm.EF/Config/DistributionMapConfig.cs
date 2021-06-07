using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class DistributionMapConfig : EntityTypeConfiguration<DistributionMap>
    {
        public DistributionMapConfig()
        {
            HasKey(i => i.Id).ToTable("distributionmaps");
            HasMany(i => i.Tasks)
                .WithRequired(i => i.DistributionMap)
                .HasForeignKey(i => i.DistributionMapId)
                .WillCascadeOnDelete(false);

            HasMany(i => i.DistributionMapCoordinates)
                .WithRequired(i => i.DistributionMap)
                .HasForeignKey(i => i.DistributionMapId)
                .WillCascadeOnDelete(false);

            HasMany(i => i.DistributionMapRecords)
                .WithRequired(i => i.DistributionMap)
                .HasForeignKey(i => i.DistributionMapId)
                .WillCascadeOnDelete(false);

            Ignore(i => i.Locations);
            Ignore(i => i.GtuInfo);
            Ignore(i => i.NdAddress);
        }
    }
}
