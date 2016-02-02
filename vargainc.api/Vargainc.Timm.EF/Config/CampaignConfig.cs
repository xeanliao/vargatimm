using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class CampaignConfig : EntityTypeConfiguration<Campaign>
    {
        public CampaignConfig()
        {
            HasKey(i => i.Id).ToTable("campaigns");

            HasMany(i => i.CampaignBlockGroupImporteds)
                .WithRequired()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.CampaignCRouteImporteds)
                .WithRequired()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.CampaignFiveZipImporteds)
                .WithRequired()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.CampaignTractImporteds)
                .WithRequired()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.CampaignClassifications)
                .WithRequired()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.Status)
                .WithRequired(i=>i.Campaign)
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.SubMaps)
                .WithRequired(i => i.Campaign)
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.Addresses)
                .WithRequired(i => i.Campaign)
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.CampaignPercentageColors)
                .WithOptional()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.CampaignRecords)
                .WithOptional()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(true);

            HasMany(i => i.DistributionJob)
                .WithOptional()
                .HasForeignKey(i => i.CampaignId);

            Ignore(i => i.LogoPath);
            Ignore(i => i.DistributionMap);
        }
    }

    
}
