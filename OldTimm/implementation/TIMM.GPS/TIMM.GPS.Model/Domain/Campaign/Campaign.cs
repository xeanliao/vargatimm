using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;

namespace TIMM.GPS.Model
{
    public class Campaign
    {
        #region Property
        [DefaultValue(0)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string CreatorName { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string CustemerName { get; set; }
        [DefaultValue(0.0)]
        public double Latitude { get; set; }
        [DefaultValue(0.0)]
        public double Longitude { get; set; }
        [DefaultValue(0)]
        public int ZoomLevel { get; set; }
        [DefaultValue(0)]
        public int Sequence { get; set; }
        public string ContactName { get; set; }
        public string ClientCode { get; set; }
        public string Logo { get; set; }
        public string AreaDescription { get; set; }
        public string ClientName { get; set; }
        #endregion

        public Campaign()
        {
            Status = new List<StatusInfo>();
            CampaignBlockGroupImporteds = new List<CampaignBlockGroupImported>();
            CampaignFiveZipImporteds = new List<CampaignFiveZipImported>();
            CampaignTractImporteds = new List<CampaignTractImported>();
            CampaignCRouteImporteds = new List<CampaignCRouteImported>();
            CampaignClassifications = new List<CampaignClassification>();
            CampaignRecords = new List<CampaignRecord>();
            SubMaps = new List<SubMap>();
            Addresses = new List<Address>();
            CampaignPercentageColors = new List<CampaignPercentageColor>();
            DistributionMap = new List<DistributionMap>();
            
        }

        #region Relationship
        public List<StatusInfo> Status { get; set; }
        public virtual List<CampaignBlockGroupImported> CampaignBlockGroupImporteds { get; set; }
        public virtual List<CampaignFiveZipImported> CampaignFiveZipImporteds { get; set; }
        public virtual List<CampaignTractImported> CampaignTractImporteds { get; set; }
        public virtual List<CampaignCRouteImported> CampaignCRouteImporteds { get; set; }

        public virtual List<CampaignClassification> CampaignClassifications { get; set; }
        public virtual List<CampaignRecord> CampaignRecords { get; set; }
        public virtual List<SubMap> SubMaps { get; set; }
        public virtual List<Address> Addresses { get; set; }
        public virtual List<CampaignPercentageColor> CampaignPercentageColors { get; set; }

        public virtual List<DistributionMap> DistributionMap { get; set; }
        #endregion

        public string LogoPath { get; set; }

        
    }
}
