using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DomainLayer.Entities;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TransferObjects")]
    public class ToPrintCampaign : ToBasicCampaignProperties
    {
        #region Extended properties
        [DataMember]
        public string UserFullName { get; set; }
        [DataMember]
        public ToPrintSubMap[] SubMaps { get; set; }
        [DataMember]
        public ToPrintAddress[] Addresses { get; set; }

        [DataMember]
        public ToCampaignPercentageColor[] CampaignPercentageColors { get; set; }

        #endregion

        public static void Convert(ref ToPrintCampaign target, ref Campaign source)
        {
            List<ToCampaignPercentageColor> toColors = new List<ToCampaignPercentageColor>();
            foreach (CampaignPercentageColor color in source.CampaignPercentageColors)
            {
                toColors.Add(new ToCampaignPercentageColor()
                {
                    Id = color.Id,
                    ColorId = color.ColorId,
                    CampaignId = color.CampaignId,
                    Min = color.Min,
                    Max = color.Max
                });
            }
            target.CampaignPercentageColors = toColors.ToArray();

            User u = GetUserByUserName(source.CreatorName);
            string userCode = null != u ? u.UserCode : string.Empty;
            target.CompositeName = Campaign.ConstructCompositeName(source.Date, source.ClientCode, source.AreaDescription, userCode, source.Sequence);
            target.UserFullName = null != u ? u.FullName : string.Empty;
        }
    }
}
