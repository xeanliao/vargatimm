using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class CampaignTractImported : CampaignImportedDataItem
    {
        public virtual Tract Tract
        {
            get;
            set;
        }
    }
}
