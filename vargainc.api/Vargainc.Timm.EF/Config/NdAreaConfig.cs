using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class NdAreaConfig : EntityTypeConfiguration<NdArea>
    {
        public NdAreaConfig()
        {
            HasKey(i => i.Id).ToTable("customareas");
        }
    }
}
