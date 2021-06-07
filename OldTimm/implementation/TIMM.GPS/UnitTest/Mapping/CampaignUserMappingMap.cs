using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UnitTest.Entities;

namespace UnitTest.Mapping
{
	public class CampaignUserMappingMap : EntityTypeConfiguration<CampaignUserMapping>
	{
		public CampaignUserMappingMap()
		{
			// Primary Key
			this.HasKey(t => t.Id);

			// Properties
			this.Property(t => t.Id)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
				
			// Table & Column Mappings
			this.ToTable("CampaignUserMapping");
			this.Property(t => t.Id).HasColumnName("Id");
			this.Property(t => t.CampaignId).HasColumnName("CampaignId");
			this.Property(t => t.UserId).HasColumnName("UserId");
			this.Property(t => t.Status).HasColumnName("Status");

			// Relationships
			this.HasOptional(t => t.Campaign)
				.WithMany(t => t.CampaignUserMappings)
				.HasForeignKey(d => d.CampaignId);
				
			this.HasRequired(t => t.CampaignUserMapping2)
				.WithOptional(t => t.CampaignUserMapping1);
				
			this.HasOptional(t => t.User)
				.WithMany(t => t.CampaignUserMappings)
				.HasForeignKey(d => d.UserId);
				
		}
	}
}

