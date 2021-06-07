using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.ModelLayer.Common
{
    [Serializable]
    public class Address
    {
        public int Id { get; set; }

        public String Address1 { get; set; }

        public String ZipCode { get; set; }

        public Double Longitude { get; set; }

        public Double Latitude { get; set; }

        public Double Radius { get; set; }

        //String Color { get; set; }

        //int CampaignId { get; set; }

        //Double OriginalLatitude { get; set; }

        //Double OriginalLongitude { get; set; }

        //List<IRadius> GPSRadiuses { get; set; }

        //IRadius GPSRadius { get; set; }


    }
}
