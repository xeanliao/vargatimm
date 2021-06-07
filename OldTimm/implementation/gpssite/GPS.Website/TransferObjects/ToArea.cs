using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DomainLayer.Area;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class ToArea
    {
        [DataMember]
        public Int32 Classification { get; set; }

        [DataMember]
        public String Id { get; set; }

        [DataMember]
        public String Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public ToCoordinate[] Locations { get; set; }

        [DataMember]
        public Dictionary<int, Dictionary<int, bool>> Relations { get; set; }

        [DataMember]
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Only used by the Otis transformer to transform the Relations property.
        /// </summary>
        /// <param name="target">A <see cref="ToAddressRadius"/>, the DTO.</param>
        /// <param name="source">A <see cref="MapAddressRadius"/>, the source.</param>
        public static void Convert(ref ToArea target, ref MapArea source)
        {
            target.Classification = (int)source.Classification;
            target.Relations = source.Relations;
            target.Attributes = source.Attributes;
        }
    }
}
