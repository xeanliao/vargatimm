using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.Website.TransferObjects;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.DistributionMapServices")]
    public class ToSubMap
    {
        [DataMember]
        public int CampaignId
        {
            get;
            set;
        }
        [DataMember]
        public int ColorB
        {
            get;
            set;
        }
        [DataMember]
        public int ColorG
        {
            get;
            set;
        }

        [DataMember]
        public int ColorR
        {
            get;
            set;
        }
        [DataMember]
        public String ColorString
        {
            get;
            set;
        }
        [DataMember]
        public int Id
        {
            get;
            set;
        }
        [DataMember]
        public String Name
        {
            get;
            set;
        }

        [DataMember]
        public int OrderId
        {
            get;
            set;
        }
        [DataMember]
        public int Penetration
        {
            get;
            set;
        }
        [DataMember]
        public double Percentage
        {
            get;
            set;
        }
        [DataMember]
        public int Total
        {
            get;
            set;
        }

        [DataMember]
        public int TotalAdjustment
        {
            get;
            set;
        }

        [DataMember]
        public int CountAdjustment
        {
            get;
            set;
        }

        [DataMember]
        public double CenterLatitude
        {
            get;
            set;
        }

        [DataMember]
        public double CenterLongitude
        {
            get;
            set;
        }

        [DataMember]
        public Int32 Classification { get; set; }

        [DataMember]
        public ToCoordinate[] SubMapCoordinates
        {
            get;
            set;
        }

        [DataMember]
        public ToAreaRecord[] SubMapRecords
        {
            get;
            set;
        }

        [DataMember]
        public ToDistributionMap[] DistributionMaps
        {
            get;
            set;
        }

        [DataMember]
        public List<double[][]> Holes { get; set; }

        public static void Convert(ref ToSubMap target, ref SubMap source)
        {
            if (source.SubMapCoordinates != null && source.SubMapCoordinates.Count > 0)
            {
                double minlat, maxlat;
                minlat = maxlat = source.SubMapCoordinates[0].Latitude;
                double minlon, maxlon;
                minlon = maxlon = source.SubMapCoordinates[0].Longitude;

                foreach (SubMapCoordinate cd in source.SubMapCoordinates)
                {
                    if (cd.Latitude > maxlat)
                    {
                        maxlat = cd.Latitude;
                    }
                    else if (cd.Latitude < minlat)
                    {
                        minlat = cd.Latitude;
                    }

                    if (cd.Longitude > maxlon)
                    {
                        maxlon = cd.Longitude;
                    }
                    else if (cd.Longitude < minlon)
                    {
                        minlon = cd.Longitude;
                    }
                }

                target.CenterLatitude = (minlat + maxlat) / 2;
                target.CenterLongitude = (minlon + maxlon) / 2;
            }

            if (source.Holes != null && source.Holes.Count > 0)
            {
                target.Holes = source.Holes;
            }

            if (source.SubMapRecords != null && source.SubMapRecords.Count > 0)
            {
                target.Classification = (int)source.SubMapRecords[0].Classification;
            }
        }

    }
}



