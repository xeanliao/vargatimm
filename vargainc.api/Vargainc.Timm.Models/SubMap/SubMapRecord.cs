using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class SubMapRecord
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0)]
        public int? Classification { get; set; }
        [DefaultValue(0)]
        public int? AreaId { get; set; }
        [DefaultValue(false)]
        public bool? Value { get; set; }
        [DefaultValue(0)]
        public int SubMapId { get; set; }
        public virtual SubMap SubMap { get; set; }
    }
}
