using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class ImportCampaign
    {
        public string AreaDescription { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ContactName { get; set; }
        public string CreatorName { get; set; }
        public string CustemerName { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public int? ZoomLevel { get; set; }
        public string UserName { get; set; }
        public string UserStatus { get; set; }

        public List<ImportSubMap> SubMap { get; set; }
        public List<ImportAddress> Address { get; set; }
        public List<int?> Classifications { get; set; }
        public List<ImportPenetration> Penetration { get; set; }
        public List<ImportPercentageColors> PercentageColors { get; set; }
    }

    public class ImportSubMap
    {
        public string Name { get; set; }
        public int? ColorR { get; set; }
        public int? ColorG { get; set; }
        public int? ColorB { get; set; }
        public string ColorString { get; set; }
        public int? CountAdjustment { get; set; }
        public int? TotalAdjustment { get; set; }
        public List<ImportCRoute> CRoutes { get; set; }
        
        [JsonIgnore]
        public List<int> PremiumCRoutes { get; set; }
        [JsonIgnore]
        public List<Models.Location> Boundary { get; set; }
        public List<ImportDMap> DMap { get; set; }

    }

    public class ImportDMap
    {
        public string Name { get; set; }
        public int? ColorR { get; set; }
        public int? ColorG { get; set; }
        public int? ColorB { get; set; }
        public string ColorString { get; set; }
        public List<string> CRoutes { get; set; }
        [JsonIgnore]
        public List<int> PremiumCRoutes { get; set; }
        [JsonIgnore]
        public List<Models.Location> Boundary { get; set; }

    }

    public class ImportAddress
    {
        public string AddressName { get; set; }
        public string Color { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public float? OriginalLatitude { get; set; }
        public float? OriginalLongitude { get; set; }
        public string Picture { get; set; }
        public string ZipCode { get; set; }
        public List<ImportAddressRadiuses> Radiuses { get; set; }
    }

    public class ImportAddressRadiuses
    {
        public float? Length { get; set; }
        public int? LengthMeasuresId { get; set; }
        public bool? IsDisplay { get; set; }
    }

    public class ImportCRoute
    {
        public int? Id { get; set; }
        public string GEOCODE { get; set; }
        public int? PartCount { get; set; }
        public int? Total { get; set; }
        public int? APT_COUNT { get; set; }
        public int? HOME_COUNT { get; set; }
    }

    public class ImportPenetration
    {
        public string GEOCODE { get; set; }
        public int? Total { get; set; }
        public int? Penetration { get; set; }
        public bool? IsPartModified { get; set; }
        public float? PartPercentage { get; set; }
    }

    public class ImportPercentageColors
    {
        public int? ColorId { get; set; }
        public float? Max { get; set; }
        public float? Min { get; set; }

    }
   
}