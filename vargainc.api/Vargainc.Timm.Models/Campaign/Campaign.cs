using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    public class Campaign
    {
        #region Property
        [DefaultValue(0)]
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string CreatorName { get; set; }
        [Newtonsoft.Json.JsonConverter(typeof (Extentions.CustomDateTimeConverter))]
        public DateTime? Date { get; set; }
        public string CustemerName { get; set; }
        [DefaultValue(0.0)]
        public double? Latitude { get; set; }
        [DefaultValue(0.0)]
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
        #endregion

        public Campaign()
        {
            //Status = new List<StatusInfo>();
            //CampaignBlockGroupImporteds = new List<CampaignBlockGroupImported>();
            //CampaignFiveZipImporteds = new List<CampaignFiveZipImported>();
            //CampaignTractImporteds = new List<CampaignTractImported>();
            //CampaignCRouteImporteds = new List<CampaignCRouteImported>();
            //CampaignClassifications = new List<CampaignClassification>();
            //CampaignRecords = new List<CampaignRecord>();
            //SubMaps = new List<SubMap>();
            //Addresses = new List<Address>();
            //CampaignPercentageColors = new List<CampaignPercentageColor>();
            //DistributionMap = new List<DistributionMap>();
            
        }

        #region Relationship
        public virtual ICollection<StatusInfo> Status { get; set; }
        public virtual ICollection<CampaignBlockGroupImported> CampaignBlockGroupImporteds { get; set; }
        public virtual ICollection<CampaignFiveZipImported> CampaignFiveZipImporteds { get; set; }
        public virtual ICollection<CampaignTractImported> CampaignTractImporteds { get; set; }
        public virtual ICollection<CampaignCRouteImported> CampaignCRouteImporteds { get; set; }

        public virtual ICollection<CampaignClassification> CampaignClassifications { get; set; }
        public virtual ICollection<CampaignRecord> CampaignRecords { get; set; }
        public virtual ICollection<SubMap> SubMaps { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<CampaignPercentageColor> CampaignPercentageColors { get; set; }

        public virtual ICollection<DistributionMap> DistributionMap { get; set; }

        public virtual ICollection<DistributionJob> DistributionJob { get; set; }
        #endregion

        public string LogoPath { get; set; }

        
    }
}
