using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class UserConfig : EntityTypeConfiguration<User>
    {
        /// <summary>
        /// Initializes a new instance of the UserConfig class.
        /// </summary>
        public UserConfig()
        {
            HasKey(i => i.Id).ToTable("users");

            HasMany(i => i.Status).WithRequired(i => i.User).HasForeignKey(i => i.UserId);
        }
    }
}
