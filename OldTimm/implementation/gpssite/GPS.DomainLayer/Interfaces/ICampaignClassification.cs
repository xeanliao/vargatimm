using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Interfaces
{
    public interface ICampaignClassification
    {
        int Id { get; set; }

        int CampaignId { get; set; }

        string Classification { get; set; }

    }
}
