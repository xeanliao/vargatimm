using log4net;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Vargainc.Timm.EF.Config;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF
{
    public class TimmContext : DbContext
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TimmContext));
        public TimmContext() : base("TIMMContext")
        {
            this.Database.Log = (sql) =>
            {
                logger.Debug(sql);
            };
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            
            modelBuilder.Configurations.Add(new FiveZipAreaConfig());
            modelBuilder.Configurations.Add(new FiveZipCoordinateConfig());
            modelBuilder.Configurations.Add(new BlockGroupConfig());
            modelBuilder.Configurations.Add(new BlockGroupCoordinateConfig());
            modelBuilder.Configurations.Add(new PremiumCRouteConfig());
            modelBuilder.Configurations.Add(new PremiumCRouteCoordinateConfig());
            modelBuilder.Configurations.Add(new TractConfig());
            modelBuilder.Configurations.Add(new TractCoordinateConfig());

            modelBuilder.Configurations.Add(new CampaignBlockGroupImportedConfig());
            modelBuilder.Configurations.Add(new CampaignCRouteImportedConfig());
            modelBuilder.Configurations.Add(new CampaignFiveZipImportedConfig());
            modelBuilder.Configurations.Add(new CampaignTractImportedConfig());
            modelBuilder.Configurations.Add(new CampaignPercentageColorConfig());
            modelBuilder.Configurations.Add(new CampaignClassificationConfig());
            modelBuilder.Configurations.Add(new CampaignRecordConfig());
            modelBuilder.Configurations.Add(new CampaignConfig());
            modelBuilder.Configurations.Add(new CampaignBackupConfig());

            

            modelBuilder.Configurations.Add(new StatusInfoConfig());
            modelBuilder.Configurations.Add(new SubMapConfig());
            modelBuilder.Configurations.Add(new SubMapCoordinateConfig());
            modelBuilder.Configurations.Add(new SubMapRecordConfig());

            modelBuilder.Configurations.Add(new RadiusesConfig());
            modelBuilder.Configurations.Add(new AddressConfig());


            modelBuilder.Configurations.Add(new DistributionMapConfig());
            modelBuilder.Configurations.Add(new DistributionMapCoordinateConfig());
            modelBuilder.Configurations.Add(new DistributionMapRecordConfig());
            modelBuilder.Configurations.Add(new TaskGtuInfoMappingConfig());
            modelBuilder.Configurations.Add(new GtuInfoConfig());
            modelBuilder.Configurations.Add(new NdAddressCoordinateConfig());
            modelBuilder.Configurations.Add(new NdAddressConfig());

            modelBuilder.Configurations.Add(new TaskConfig());

            modelBuilder.Configurations.Add(new UserConfig());
            modelBuilder.Configurations.Add(new GroupConfig());
        }

        public DbSet<FiveZipArea> FiveZipAreas { get; set; }
        public DbSet<FiveZipCoordinate> FiveZipCoordinates { get; set; }
        public DbSet<BlockGroup> BlockGroups { get; set; }
        public DbSet<BlockGroupCoordinate> BlockGroupCoordinates { get; set; }
        public DbSet<PremiumCRoute> PremiumCRoutes { get; set; }
        public DbSet<PremiumCRouteCoordinate> PremiumCRouteCoordinates { get; set; }
        public DbSet<Tract> Tracts { get; set; }
        public DbSet<TractCoordinate> TractCoordinates { get; set; }

        public DbSet<CampaignFiveZipImported> CampaignFiveZipImporteds { get; set; }
        public DbSet<CampaignCRouteImported> CampaignCRouteImporteds { get; set; }
        public DbSet<CampaignTractImported> CampaignTractImporteds { get; set; }
        public DbSet<CampaignBlockGroupImported> CampaignBlockGroupImporteds { get; set; }
        public DbSet<CampaignClassification> CampaignClassifications { get; set; }
        public DbSet<CampaignPercentageColor> CampaignPercentageColors { get; set; }
        public DbSet<StatusInfo> Status { get; set; }
        public DbSet<CampaignRecord> CampaignRecords { get; set; }
        public DbSet<SubMap> SubMaps { get; set; }
        public DbSet<SubMapRecord> SubMapRecords { get; set; }
        public DbSet<SubMapCoordinate> SubMapCoordinates { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Radiuses> Radiuses { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CampaignBackup> CampaignBackups { get; set; }
        public DbSet<DistributionMap> DistributionMaps { get; set; }
        public DbSet<DistributionMapCoordinate> DistributionMapCoordinates { get; set; }
        public DbSet<DistributionMapRecord> DistributionMapRecords { get; set; }
        public DbSet<TaskGtuInfoMapping> TaskGtuInfoMappings { get; set; }
        public DbSet<GtuInfo> GtuInfos { get; set; }
        public DbSet<NdAddress> NdAddresses { get; set; }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}
