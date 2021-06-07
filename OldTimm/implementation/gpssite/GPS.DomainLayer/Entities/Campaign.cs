using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class Campaign : ICampaign
    {
        #region basic properties

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string UserName { get; set; }

        public virtual string CreatorName { get; set; }

        public virtual DateTime Date { get; set; }

        public virtual string CustemerName { get; set; }

        public virtual double Latitude { get; set; }

        public virtual double Longitude { get; set; }

        public virtual int ZoomLevel { get; set; }

        public virtual int Sequence { get; set; }

        public virtual string ContactName { get; set; }

        public virtual string ClientCode { get; set; }

        public virtual string Logo { get; set; }

        public virtual string AreaDescription { get; set; }

        public virtual string ClientName { get; set; }

        #endregion

        #region childrens

        public virtual IList<CampaignBlockGroupImported> CampaignBlockGroupImporteds { get; set; }

         public virtual IList<CampaignClassification> CampaignClassifications { get; set; }

        public virtual IList<CampaignFiveZipImported> CampaignFiveZipImporteds { get; set; }

        public virtual IList<CampaignRecord> CampaignRecords { get; set; }

        public virtual IList<CampaignTractImported> CampaignTractImporteds { get; set; }

        public virtual IList<CampaignCRouteImported> CampaignCRouteImporteds { get; set; }

        public virtual IList<SubMap> SubMaps { get; set; }

        public virtual IList<Address> Addresses { get; set; }

        public virtual IList<CampaignPercentageColor> CampaignPercentageColors { get; set; }

        public virtual IDictionary<User, StatusInfo> Users { get; set; }

        #endregion

        #region extended properties
        
        public virtual List<ISubMap> CampaignSubMaps { get;set;}

        public virtual List<ICircleAreaRelation> CircleRecords { get; set; }

        public virtual List<ICampaignRecords> GPSCampaignRecords { get; set; }

        public virtual List<IAddress> GPSAddresses { get; set; }

        public virtual List<ICampaignClassification> GPSCampaignClassification { get; set; }

        public virtual List<ICampaignPercentageColor> GPSPercentageColors { get; set; }

        #endregion

        #region ICampaign Members

        public virtual string UserCode { get; set; }

        public virtual string UserFullName { get; set; }

        #endregion

        #region Query methods

        public virtual SubMap GetSubMap(int submapId)
        {
            if (null == this.SubMaps)
            {
                return null;
            }
            return this.SubMaps.FirstOrDefault(s => s.Id == submapId);
        }

        public virtual Boolean ContainsSubMap(int submapId)
        {
            return null != GetSubMap(submapId);
        }

        public virtual DistributionMap GetDistributionMap(int submapId, int distributionMapId)
        {
            DistributionMap dm = null;

            if (ContainsSubMap(submapId))
                dm = GetDistributionMapFromSubMap(GetSubMap(submapId), distributionMapId);

            return dm;
        }

        #endregion

        #region Commands

        public virtual void RemoveDistributionMapsOfSubMap(int submapId)
        {
            GetSubMap(submapId).DistributionMaps.Clear();
        }

        public virtual void AddDistributionMapToSubMap(SubMap submap, DistributionMap distributionMap)
        {
            if (null == submap.DistributionMaps)
            {
                submap.DistributionMaps = new List<DistributionMap>();
            }
            submap.DistributionMaps.Add(distributionMap);
        }

        public virtual void RemoveDistributionMapFromSubMap(int submapId, DistributionMap distributionMap)
        {
            RemoveDistributionMapFromSubMap(GetSubMap(submapId), distributionMap);
        }
        
        #endregion

        #region Helper methods that encapsulates business rules
        public static string ConstructCompositeName(DateTime campaignCreationDate, string clientCode, string areaDescription, string userCode, int sequence)
        {
            string[] dateComponents = new string[]
            {
                campaignCreationDate.Month.ToString().PadLeft(2, '0'),
                campaignCreationDate.Day.ToString().PadLeft(2, '0'),
                campaignCreationDate.Year.ToString().Substring(2)
            };

            string[] compositeNamecomponents = new string[] 
            { 
                string.Join("", dateComponents),
                clientCode,
                userCode,
                areaDescription,
                sequence.ToString()
            };

            return string.Join("-", compositeNamecomponents);
        }

        #endregion

        #region Implementations

        private DistributionMap GetDistributionMapFromSubMap(SubMap sm, int distributionMapId)
        {
            return sm.DistributionMaps.FirstOrDefault(d => d.Id == distributionMapId);
        }

        private void RemoveDistributionMapFromSubMap(SubMap sm, DistributionMap distributionMap)
        {
            sm.DistributionMaps.Remove(distributionMap);
        }

        #endregion
    }
}
