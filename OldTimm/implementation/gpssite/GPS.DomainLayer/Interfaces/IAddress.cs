using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace GPS.DomainLayer.Interfaces
{
    public interface IAddress
    {
        int Id { get; set; }

        string Address1 { get; set; }

        string ZipCode { get; set; }

        double Longitude { get; set; }

        double Latitude { get; set; }

        string Color { get; set; }

        int CampaignId { get;  }

        double OriginalLatitude { get; set; }

        double OriginalLongitude { get; set; }

        List<IRadius> GPSRadiuses { get; set; }
		
    }
}
