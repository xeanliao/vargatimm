using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class DistributionMapRecord
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0)]
        public int? Classification { get; set; }
        [DefaultValue(0)]
        public int? AreaId { get; set; }
        public string Code { get; set; }
        [DefaultValue(false)]
        public bool? Value { get; set; }
        [DefaultValue(0)]
        public int DistributionMapId { get; set; }
        public virtual DistributionMap DistributionMap { get; set; }
    }
}
