using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.ModelLayer.Common
{
    public interface IAddress
    {
        int Id { get; set; }

        String Address1 { get; set; }

        String ZipCode { get; set; }

        Double Longitude { get; set; }

        Double Latitude { get; set; }

        //String Color { get; set; }

        //int CampaignId { get; set; }

        //Double OriginalLatitude { get; set; }

        //Double OriginalLongitude { get; set; }

        //List<IRadius> GPSRadiuses { get; set; }

        IRadius GPSRadius { get; set; }

    }
}
