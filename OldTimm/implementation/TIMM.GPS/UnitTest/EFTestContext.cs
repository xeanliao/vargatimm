using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using UnitTest.Entities;
using UnitTest.Mapping;
using System.Data;
using System.Data.SqlClient;

namespace UnitTest
{
	public class EFTestContext : DbContext
	{
		static EFTestContext()
		{ 
			Database.SetInitializer<EFTestContext>(null);
		}

		public DbSet<Campaign> Campaigns { get; set; }
		public DbSet<CampaignUserMapping> CampaignUserMappings { get; set; }
		public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
			modelBuilder.Configurations.Add(new CampaignMap());
			modelBuilder.Configurations.Add(new CampaignUserMappingMap());
			modelBuilder.Configurations.Add(new UserMap());
		}
	}
}

