using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GPS.DomainLayer.Entities
{
    [Serializable]
    public class CampaignBlockGroupImported : CampaignImportedDataItem
    {
        #region parent
        public virtual BlockGroup BlockGroup { get; set; }
        #endregion
    }
}
