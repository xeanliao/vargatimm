using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class NdAddressConfig : EntityTypeConfiguration<NdAddress>
    {
        public NdAddressConfig()
        {
            HasKey(i => i.Id).ToTable("ndaddresses");
        }
    }
}