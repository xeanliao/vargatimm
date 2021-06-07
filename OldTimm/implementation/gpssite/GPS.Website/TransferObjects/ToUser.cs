using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.Website.TransferObjects;
using GPS.Website.AppFacilities;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.UserServices")]
    public class ToUser
    {
        [DataMember]
        public int Id
        {
            get;
            set;
        }
        [DataMember]
        public String UserName
        {
            get;
            set;
        }
        [DataMember]
        public String Password
        {
            get;
            set;
        }
        [DataMember]
        public String Email
        {
            get;
            set;
        }
        [DataMember]
        public Boolean Enabled
        {
            get;
            set;
        }
        [DataMember]
        public String FullName
        {
            get;
            set;
        }
        [DataMember]
        public String UserCode
        {
            get;
            set;
        }
        [DataMember]
        public int Role
        {
            get;
            set;
        }

        [DataMember]
        public String RoleName
        {
            get;
            set;
        }

        [DataMember]
        public ToGroup[] Groups
        {
            get;
            set;
        }

        //[DataMember]
        //public ToTaskgtuinfomapping[] Taskgtuinfomappings
        //{
        //    get;
        //    set;
        //}
        //[DataMember]
        //public ToTask[] Tasks
        //{
        //    get;
        //    set;
        //}

        //[DataMember]
        //public List<ToGroup> Groups
        //{
        //    get;
        //    set;
        //}

        //[DataMember]
        //public IDictionary<ToCampaign, ToStatusInfo> Campaigns
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Only used by the Otis transformer to transform the role property.
        /// </summary>
        /// <param name="target">A <see cref="ToUser"/>, the DTO.</param>
        /// <param name="source">A <see cref="User"/>, the source.</param>
        public static void Convert(ref ToUser target, ref User source)
        {
            target.Role = (int)source.Role;
            target.RoleName = source.Role.ToString();
            target.Email = source.Email;
            //if (source.Groups == null)
            //{
            //    target.Groups = new List<ToGroup>();
            //}
            //else if (source.Groups.Count > 0)
            //{
            //    target.Groups = new List<ToGroup>();
            //    target.Groups.AddRange(AssemblerConfig.GetAssembler<ToGroup, Group>().AssembleFrom(source.Groups));
            //}

            //if (source.Campaigns == null)
            //{
            //    target.Campaigns = new Dictionary<ToCampaign, ToStatusInfo>();
            //}
            //else if (source.Campaigns.Count > 0)
            //{
            //    target.Campaigns = new Dictionary<ToCampaign, ToStatusInfo>();
            //    foreach (KeyValuePair<Campaign, StatusInfo> kvp in source.Campaigns)
            //    {
            //        ToCampaign toCampaign = AssemblerConfig.GetAssembler<ToCampaign, Campaign>().AssembleFrom(kvp.Key);
            //        ToStatusInfo toStatusInfo = AssemblerConfig.GetAssembler<ToStatusInfo, StatusInfo>().AssembleFrom(kvp.Value);
            //        target.Campaigns.Add(toCampaign, toStatusInfo);
            //    }
            //}

        }
        /// <summary>
        /// Only used by the Otis transformer to transform the role property.
        /// </summary>
        /// <param name="target">A <see cref="ToUser"/>, the DTO.</param>
        /// <param name="source">A <see cref="User"/>, the source.</param>
        public static void ConvertBack(ref User target, ref ToUser source)
        {
            target.Role = (UserRoles)source.Role;
            target.Email = source.Email;
            //if (source.Groups == null)
            //{
            //    target.Groups = new List<Group>();
            //}
            //else if (source.Groups.Count > 0)
            //{
            //    target.Groups = new List<Group>();
            //    (target.Groups as List<Group>).AddRange(AssemblerConfig.GetAssembler<Group, ToGroup>().AssembleFrom(source.Groups));
            //}

            //if (source.Campaigns == null)
            //{
            //    target.Campaigns = new Dictionary<Campaign, StatusInfo>();
            //}
            //else if (source.Campaigns.Count > 0)
            //{
            //    target.Campaigns = new Dictionary<Campaign, StatusInfo>();
            //    foreach (KeyValuePair<ToCampaign, ToStatusInfo> kvp in source.Campaigns)
            //    {
            //        Campaign campaign = AssemblerConfig.GetAssembler<Campaign, ToCampaign>().AssembleFrom(kvp.Key);
            //        StatusInfo statusInfo = AssemblerConfig.GetAssembler<StatusInfo, ToStatusInfo>().AssembleFrom(kvp.Value);
            //        target.Campaigns.Add(campaign, statusInfo);
            //    }
            //}
        }
        
    }


    [DataContract(Namespace = "TIMM.Website.UserServices")]
    public class ToStatusInfo
    {
        [DataMember]
        public int Status
        {
            get;
            set;
        }

    }
}
