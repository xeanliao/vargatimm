using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using TIMM.GPS.Model;

namespace TIMM.GPS.RIAService
{
    public class TIMMContext : DbContext
    {
        public DbSet<Campaign> Campaigns { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Campaign>().ToTable("campaigns");
            modelBuilder.Entity<Campaign>().HasKey(i => i.Id);
        }
    }
}
