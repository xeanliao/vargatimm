using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class CampaignFiveZipImported : CampaignImportedDataItem
    {
        public virtual float PartPercentage
        {
            get;
            set;
        }
        public virtual bool IsPartModified
        {
            get;
            set;
        }
        #region parent
        public virtual FiveZipArea FiveZipArea
        {
            get;
            set;
        }
        #endregion
    }
}
