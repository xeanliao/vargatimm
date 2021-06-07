using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vargainc.Timm.Models
{
    public abstract class AbstractArea
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        [DefaultValue(0)]
        public int? IsInnerShape { get; set; }
        [DefaultValue(0)]
        public int? PartCount { get; set; }
        [DefaultValue(0)]
        public byte? IsInnerRing { get; set; }
        [DefaultValue(0.0)]
        public double? Latitude { get; set; }
        [DefaultValue(0.0)]
        public double? Longitude { get; set; }
        [DefaultValue(0.0)]
        public double? MaxLatitude { get; set; }
        [DefaultValue(0.0)]
        public double? MaxLongitude { get; set; }
        [DefaultValue(0.0)]
        public double? MinLatitude { get; set; }
        [DefaultValue(0.0)]
        public double? MinLongitude { get; set; }

        [Column("IsEnabled")]
        public int? _IsEnabled { get; set; }

        [NotMapped]
        public bool? IsEnabled {
            get
            {
                return _IsEnabled.HasValue && _IsEnabled.Value == 1;
            }
            set
            {
                _IsEnabled = value.HasValue && value == true ? 1 : 0;
            }
        }

        public bool? HasMultipleParts
        {
            get
            {
                return PartCount > 1;
            }
        }
    }
}
