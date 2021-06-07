using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class Radiuses
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0)]
        public int? AddressId { get; set; }
        public Address Address { get; set; }
        [DefaultValue(false)]
        public bool? IsDisplay { get; set; }
        [DefaultValue(0.0)]
        public double? Length { get; set; }
        [DefaultValue(0)]
        public int? LengthMeasuresId { get; set; }
    }
}
