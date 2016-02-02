using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class NdAddressCoordinateConfig : EntityTypeConfiguration<NdAddressCoordinate>
    {
        public NdAddressCoordinateConfig()
        {
            HasKey(i => i.Id).ToTable("ndaddresscoordinates");
        }
    }
}
