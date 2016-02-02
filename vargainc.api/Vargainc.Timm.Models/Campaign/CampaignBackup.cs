using System;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class CampaignBackup
    {
        [DefaultValue(0)]
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public DateTime? Date { get; set; }
        public string CustemerName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [DefaultValue(0)]
        public int? ZoomLevel { get; set; }
        [DefaultValue(0)]
        public int? Sequence { get; set; }
        public string ContactName { get; set; }
        public string ClientCode { get; set; }
        public string Logo { get; set; }
        public string AreaDescription { get; set; }
        public string ClientName { get; set; }
        public string IPAddress { get; set; }
        public DateTime? OperationTime { get; set; }
        public string OperationUser { get; set; }
    }
}
