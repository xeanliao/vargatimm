using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vargainc.Timm.Models.Attributes;

namespace Vargainc.Timm.EF.Conventions
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/conventions/custom
    /// </summary>
    public class DateTime2Convention : Convention
    {
        public DateTime2Convention()
        {
            this.Properties()
                .Where(x => x.GetCustomAttributes(false).OfType<DateTime2>().Any())
                .Configure(c => {
                    c.HasColumnType("datetime2");
                });
        }
    }
}
