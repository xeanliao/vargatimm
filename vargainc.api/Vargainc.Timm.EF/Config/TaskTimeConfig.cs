using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class TaskTimeConfig : EntityTypeConfiguration<TaskTime>
    {
        public TaskTimeConfig()
        {
            HasKey(i => i.Id).ToTable("tasktime");

            Property(i => i.Time)
                .HasColumnType("datetime2")
                .HasPrecision(0);

        }
    }
}
