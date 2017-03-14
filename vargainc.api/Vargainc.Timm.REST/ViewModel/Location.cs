using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class Location
    {
        public Location()
        {

        }
        public Location(double? lat, double? lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
        }

        public long? Id { get; set; }

        [JsonProperty("lat")]
        public double? Latitude { get; set; }

        [JsonProperty("lng")]
        public double? Longitude { get; set; }

        [JsonProperty("out")]
        public bool? OutOfBoundary { get; set; }

        /// <summary>
        /// nCellId: 0-original ,1-added ,2-removed, 3-merged
        /// only the added gtu returned gtuinfo.id
        /// </summary>
        [JsonProperty("cellId")]
        public int? nCellId { get; set; }
        /// <summary>
        /// gtu record id in table gtuinfo
        /// </summary>
        [JsonProperty("gtuInfoId")]
        public long? GtuInfoId { get; set; }
        
    }
}