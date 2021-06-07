using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DomainLayer.Entities;
using GPS.Website.AppFacilities;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TransferObjects")]
    public class ToCampaign : ToBasicCampaignProperties
    {
        [DataMember]
        public List<ToAreaRecord> CampaignRecords { get; set; }

        [DataMember]
        public IEnumerable<ToCampaignPercentageColor> CampaignPercentageColors { get; set; }

        [DataMember]
        public ToAddress[] Addresses { get; set; }

        [DataMember]
        public ToSubMap[] SubMaps { get; set; }

        [DataMember]
        public List<Int32> VisibleClassifications { get; set; }

        [DataMember]
        public IDictionary<ToUser, ToStatusInfo> Users
        {
            get;
            set;
        }

        [DataMember]
        public List<NotIncludeArea> NotIncludeInSubMapArea { get; set; }
        /// <summary>
        /// Only used by the Otis transformer to transform the Relations property.
        /// </summary>
        /// <param name="target">A <see cref="ToAddressRadius"/>, the DTO.</param>
        /// <param name="source">A <see cref="MapAddressRadius"/>, the source.</param>
        public static void Convert(ref ToCampaign target, ref Campaign source)
        {
            User u = GetUserByUserName(source.CreatorName);
            string userCode = null != u ? u.UserCode : string.Empty;
            target.CompositeName = Campaign.ConstructCompositeName(source.Date, source.ClientCode, source.AreaDescription, userCode, source.Sequence);
            target.Date = source.Date.ToString();

            List<ToAreaRecord> records = new List<ToAreaRecord>();
            foreach (CampaignRecord cRecord in source.CampaignRecords)
            {
                ToAreaRecord record = new ToAreaRecord()
                {
                    AreaId = cRecord.AreaId,
                    Classification = cRecord.Classification,
                    Value = cRecord.Value
                };
                ToAreaRecord.Convert(ref record, cRecord);

                records.Add(record);
            }
            target.CampaignRecords = records;

            List<Int32> vClassifications = new List<int>();
            foreach (CampaignClassification c in source.CampaignClassifications)
            {
                vClassifications.Add(c.Classification);
            }
            target.VisibleClassifications = vClassifications;

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
            target.CampaignPercentageColors = toColors;

            //if (source.Users == null)
            //{
            //    target.Users = new Dictionary<ToUser, ToStatusInfo>();
            //}
            //else if (source.Users.Count > 0)
            //{
            //    target.Users = new Dictionary<ToUser, ToStatusInfo>();
            //    foreach (KeyValuePair<User, StatusInfo> kvp in source.Users)
            //    {
            //        ToUser toUser = AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(kvp.Key);
            //        ToStatusInfo toStatusInfo = AssemblerConfig.GetAssembler<ToStatusInfo, StatusInfo>().AssembleFrom(kvp.Value);
            //        target.Users.Add(toUser, toStatusInfo);
            //    }
            //}
        }

    }
}
