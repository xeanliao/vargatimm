using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class SubMapRecordConfig : EntityTypeConfiguration<SubMapRecord>
    {
        public SubMapRecordConfig()
        {
            HasKey(i => i.Id).ToTable("submaprecords");
        }
    }
}
