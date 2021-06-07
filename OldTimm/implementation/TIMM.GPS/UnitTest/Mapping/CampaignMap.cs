using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UnitTest.Entities;

namespace UnitTest.Mapping
{
	public class CampaignMap : EntityTypeConfiguration<Campaign>
	{
		public CampaignMap()
		{
			// Primary Key
			this.HasKey(t => t.Id);

			// Properties
			this.Property(t => t.Id)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
				
			this.Property(t => t.Name)
				.HasMaxLength(50);
				
			// Table & Column Mappings
			this.ToTable("Campaign");
			this.Property(t => t.Id).HasColumnName("Id");
			this.Property(t => t.Name).HasColumnName("Name");
		}
	}
}

