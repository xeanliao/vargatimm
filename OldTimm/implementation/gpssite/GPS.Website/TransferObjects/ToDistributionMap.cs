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
    public class ToDistributionMap
    {
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
        public int SubMapId
        {
            get;
            set;
        }
        [DataMember]
        public virtual int ColorB
        {
            get;
            set;
        }
        [DataMember]
        public virtual int ColorG
        {
            get;
            set;
        }
        [DataMember]
        public virtual int ColorR
        {
            get;
            set;
        }
        [DataMember]
        public virtual string ColorString
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
        public ToAreaRecord[] DistributionMapRecords
        {
            get;
            set;
        }

        [DataMember]
        public ToCoordinate[] DistributionMapCoordinates
        {
            get;
            set;
        }

        [DataMember]
        public ToTask[] Tasks
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
        public List<double[][]> Holes { get; set; }

        public static void Convert(ref ToDistributionMap target, ref GPS.DomainLayer.Entities.DistributionMap source)
        {
            if (source.DistributionMapCoordinates != null && source.DistributionMapCoordinates.Count > 0)
            {
                double minlat, maxlat;
                minlat = maxlat = source.DistributionMapCoordinates[0].Latitude;
                double minlon, maxlon;
                minlon = maxlon = source.DistributionMapCoordinates[0].Longitude;

                foreach (DistributionMapCoordinate cd in source.DistributionMapCoordinates)
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
        }

    }
}
