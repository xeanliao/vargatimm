using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UnitTest.Entities;

namespace UnitTest.Mapping
{
	public class UserMap : EntityTypeConfiguration<User>
	{
		public UserMap()
		{
			// Primary Key
			this.HasKey(t => t.Id);

			// Properties
			this.Property(t => t.Id)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
				
			this.Property(t => t.Code)
				.HasMaxLength(50);
				
			// Table & Column Mappings
			this.ToTable("User");
			this.Property(t => t.Id).HasColumnName("Id");
			this.Property(t => t.Code).HasColumnName("Code");
		}
	}
}

