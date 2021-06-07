using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DomainLayer.Area.AreaMerge;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.DomainLayer.Area")]
    public class ToMergeResult
    {
        [DataMember]
        public double Total { get; set; }

        [DataMember]
        public double Count { get; set; }

        [DataMember]
        public double[][] Points { get; set; }

        [DataMember]
        public List<double[][]> Holes { get; set; }

        [DataMember]
        public List<NotIncludeArea> NotIncludeInSubMapArea { get; set; }

        [DataMember]
        public bool IsLastMultiParts { get; set; }

        /// <summary>
        /// for add multiple select area to dmap. some ares maybe included in dmap or out side in submap.
        /// should used this property to add in map.
        /// </summary>
        [DataMember]
        public List<ToAreaRecord> ValidAreas { get; set; }

        /// <summary>
        /// Only used by the Otis transformer to transform the role property.
        /// </summary>
        /// <param name="target">A <see cref="ToUser"/>, the DTO.</param>
        /// <param name="source">A <see cref="User"/>, the source.</param>
        public static void Convert(ref ToMergeResult target, ref MergeResult source)
        {
            if (source.Locs.Count == 1)
            {
                target.Points = source.Locs[0];
            }
            else
            {
                target.Points = new double[0][];
            }

            if (source.Holes != null)
            {
                target.Holes = source.Holes;
            }
            else
            {
                target.Holes = new List<double[][]>();
            }
        }
    }

    [DataContract(Namespace = "TIMM.LatLngLocation")]
    public class LatLngLocation
    {
        [DataMember]
        public double Lat { get; set; }
        [DataMember]
        public double Lng { get; set; }
    }

    [DataContract(Namespace = "TIMM.NotIncludeArea")]
    public class NotIncludeArea
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public List<LatLngLocation[]> Areas { get; set; }

    }
}
