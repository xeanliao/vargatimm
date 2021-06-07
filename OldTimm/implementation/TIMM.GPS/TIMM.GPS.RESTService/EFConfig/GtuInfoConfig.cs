using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
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
