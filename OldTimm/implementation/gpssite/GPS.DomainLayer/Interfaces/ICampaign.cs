using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GPS.DomainLayer.Interfaces {
    [JsonObject(MemberSerialization.OptIn)]
    public interface ICampaign {
        [JsonProperty]
        int Id { get; set; }

        [JsonProperty]
        string Name { get; set; }

        [JsonProperty]
        string Description { get; set; }

        [JsonProperty]
        string UserName { get; set; }

        [JsonProperty]
        string UserCode { get; set; }

        [JsonProperty]
        string UserFullName { get; set; }

        [JsonProperty]
        System.DateTime Date { get; set; }

        [JsonProperty]
        string CustemerName { get; set; }

        [JsonProperty]
        double Latitude { get; set; }

        [JsonProperty]
        double Longitude { get; set; }

        [JsonProperty]
        int ZoomLevel { get; set; }

        [JsonProperty]
        int Sequence { get; set; }

        [JsonProperty]
        string ClientName { get; set; }

        [JsonProperty]
        string ContactName { get; set; }

        [JsonProperty]
        string ClientCode { get; set; }

        [JsonProperty]
        string Logo { get; set; }

        [JsonProperty]
        string AreaDescription { get; set; }

        [JsonProperty]
        List<ISubMap> CampaignSubMaps { get; set; }

        [JsonProperty]
        List<ICircleAreaRelation> CircleRecords { get; set; }

        [JsonProperty]
        List<ICampaignRecords> GPSCampaignRecords { get; set; }

        [JsonProperty]
        List<IAddress> GPSAddresses { get; set; }

        [JsonProperty]
        List<ICampaignClassification> GPSCampaignClassification { get; set; }

        [JsonProperty]
        List<ICampaignPercentageColor> GPSPercentageColors { get; set; }
    }
}
