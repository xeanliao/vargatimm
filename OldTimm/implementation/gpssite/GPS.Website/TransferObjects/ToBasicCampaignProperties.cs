using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.RepositoryInterfaces;
using GPS.DataLayer;

namespace GPS.Website.TransferObjects
{
    /// <summary>
    /// Encapsulation of basic <see cref="Campaign"/> properties mainly for create
    /// and edit <see cref="Campaign"/>s.
    /// </summary>
    [DataContract(Namespace = "TIMM.Website.TransferObjects")]
    public class ToBasicCampaignProperties
    {
        #region properties inherited from the Campaign entity
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string CreatorName { get; set; }
        [DataMember]
        public string Date { get; set; }
        [DataMember]
        public string CustemerName { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public int ZoomLevel { get; set; }
        [DataMember]
        public int Sequence { get; set; }
        [DataMember]
        public string ContactName { get; set; }
        [DataMember]
        public string ClientCode { get; set; }
        [DataMember]
        public string Logo { get; set; }
        [DataMember]
        public string AreaDescription { get; set; }
        [DataMember]
        public string ClientName { get; set; }
        #endregion

        #region extended properties for easy access on client side
        /// <summary>
        /// Get or set a value indicating the campaign name seen by end users, which
        /// combines the value of several basic campaign properties.
        /// </summary>
        [DataMember]
        public string CompositeName { get; set; }
        #endregion

        public static void Convert(ref ToBasicCampaignProperties target, ref Campaign source)
        {
            User u = GetUserByUserName(source.CreatorName);
            string userCode = null != u ? u.UserCode : string.Empty;
            target.CompositeName = Campaign.ConstructCompositeName(source.Date, source.ClientCode, source.AreaDescription, userCode, source.Sequence);
            target.Date = source.Date.ToString("yyyy-MM-dd");
        }

        public static void ConvertBack(ref Campaign target, ref ToBasicCampaignProperties source)
        {
            target.Date = DateTime.Parse(source.Date);
        }

        protected static User GetUserByUserName(string userName)
        {
            User u = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                u = ws.Repositories.UserRepository.GetUser(userName);
            }

            return u;
        }
    }
}
