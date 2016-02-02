using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class GroupConfig : EntityTypeConfiguration<Group>
    {
        public GroupConfig()
        {
            HasKey(i => i.Id).ToTable("groups");

            HasMany(i => i.Users)
                .WithMany(i => i.Groups)
                .Map(m => m.MapLeftKey("GroupId").MapRightKey("UserId").ToTable("usergroupmappings"));
        }
    }
}
