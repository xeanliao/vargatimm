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
                .WillCascadeOnDelete(true);

            HasMany(i => i.DistributionMapCoordinates)
                .WithRequired(i => i.DistributionMap)
                .HasForeignKey(i => i.DistributionMapId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.DistributionMapRecords)
                .WithRequired(i => i.DistributionMap)
                .HasForeignKey(i => i.DistributionMapId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.DistributionJob)
                .WithMany()
                .Map(k => {
                    k.MapLeftKey("DistributionMapId");
                    k.MapRightKey("DistributionJobId");
                    k.ToTable("distributionjobmaps");
                });

            Ignore(i => i.Locations);
            Ignore(i => i.GtuInfo);
            Ignore(i => i.NdAddress);
        }
    }
}
