using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class GTUBagConfig : EntityTypeConfiguration<GTUBag>
    {
        public GTUBagConfig()
        {
            HasKey(i => i.Id).ToTable("gtubags");

            HasOptional(i => i.Aditor).WithMany().HasForeignKey(i => i.UserId);
        }
    }
}
