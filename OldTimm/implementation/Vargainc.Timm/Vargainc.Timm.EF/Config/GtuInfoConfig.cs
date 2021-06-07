using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{ 
    class GtuInfoConfig : EntityTypeConfiguration<GtuInfo>
    {
        public GtuInfoConfig()
        {
            HasKey(i => i.Id).ToTable("gtuinfo");

            #region these property is direct used by T-SQL don't remove modified by jacob.chen
            Ignore(i => i.UserColor);
            Ignore(i => i.Latitude);
            Ignore(i => i.Longitude);
            #endregion
        }
    }
}
