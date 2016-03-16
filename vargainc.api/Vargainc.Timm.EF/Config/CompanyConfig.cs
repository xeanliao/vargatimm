using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class CompanyConfig : EntityTypeConfiguration<Company>
    {
        /// <summary>
        /// Initializes a new instance of the UserConfig class.
        /// </summary>
        public CompanyConfig()
        {
            HasKey(i => i.Id).ToTable("companies");
        }
    }
}
