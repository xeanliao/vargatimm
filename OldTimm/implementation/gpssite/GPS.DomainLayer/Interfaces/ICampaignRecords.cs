using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Interfaces
{
    public interface ICampaignRecords
    {
        int Id
        {
            get;
            set;
        }

        int CampaignId
        {
            get;
        }

        int Classification
        {
            get;
            set;
        }

        int AreaId
        {
            get;
            set;
        }

        bool Value
        {
            get;
            set;
        }

        string ClientAreaId { get; set; }

        List<List<string>> Relation { get; set; }
    }
}
