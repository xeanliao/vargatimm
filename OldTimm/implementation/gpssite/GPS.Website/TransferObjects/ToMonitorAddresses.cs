using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GPS.DomainLayer.Area.Addresses;
using System.Runtime.Serialization;
using GPS.DomainLayer.Entities;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class ToMonitorAddresses
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Address1 { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
        [DataMember]
        public double OriginalLatitude { get; set; }
        [DataMember]
        public double OriginalLongitude { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public string Picture { get; set; }
        //[DataMember]
        //public ToAddressRadius[] Radiuses { get; set; }

        /// <summary>
        /// Only used by the Otis transformer to transform the Relations property.
        /// </summary>
        /// <param name="target">A <see cref="ToAddressRadius"/>, the DTO.</param>
        /// <param name="source">A <see cref="MapAddressRadius"/>, the source.</param>
        //public static void Convert(ref ToAddress target, ref Address source)
        //{
        //    target.Street = source.Address1;
        //}

    }
    //[DataContract(Namespace = "TIMM.Website.AreaServices")]
    //public class ToAddressRadius
    //{
    //    [DataMember]
    //    public int Id { get; set; }
    //    [DataMember]
    //    public double Length { get; set; }
    //    [DataMember]
    //    public int LengthMeasuresId { get; set; }
    //    [DataMember]
    //    public bool IsDisplay { get; set; }
    //    [DataMember]
    //    public Dictionary<int, Dictionary<int, string>> Relations { get; set; }

    //    /// <summary>
    //    /// Only used by the Otis transformer to transform the Relations property.
    //    /// </summary>
    //    /// <param name="target">A <see cref="ToAddressRadius"/>, the DTO.</param>
    //    /// <param name="source">A <see cref="MapAddressRadius"/>, the source.</param>
    //    public static void Convert(ref ToAddressRadius target, ref MapAddressRadius source)
    //    {
    //        target.Relations = source.Relations;
    //    }

    //    /// <summary>
    //    /// Only used by the Otis transformer to transform the Relations property.
    //    /// </summary>
    //    /// <param name="target">A <see cref="ToAddressRadius"/>, the DTO.</param>
    //    /// <param name="source">A <see cref="MapAddressRadius"/>, the source.</param>
    //    public static void Convert(ref ToAddressRadius target, ref Radiuse source)
    //    {
    //        Dictionary<int, Dictionary<int, string>> relations = new Dictionary<int, Dictionary<int, string>>();
    //        foreach (RadiusRecord record in source.RadiusRecords)
    //        {
    //            if(!relations.ContainsKey((int)record.Classification))
    //            {
    //                relations.Add((int)record.Classification, new Dictionary<int, string>());
    //            }
    //            if (!relations[(int)record.Classification].ContainsKey(record.AreaId))
    //            {
    //                relations[(int)record.Classification].Add(record.AreaId, record.AreaId.ToString());
    //            }
    //        }
    //        target.Relations = relations;
    //    }

    //}
}
