using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class GTUConfig : EntityTypeConfiguration<GTU>
    {
        public GTUConfig()
        {
            HasKey(i => i.Id).ToTable("Gtus");
            Property(i => i.BagId).HasColumnName("GTUBagId");
            HasOptional(i => i.Bag).WithMany(i => i.GTU).HasForeignKey(i => i.BagId);
            HasOptional(i => i.User).WithMany().HasForeignKey(i => i.UserId);
        }
    }
}
