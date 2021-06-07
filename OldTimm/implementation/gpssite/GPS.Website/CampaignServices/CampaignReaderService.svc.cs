using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.Website.AppFacilities;
using GPS.Website.TransferObjects;
using log4net;

namespace GPS.Website.CampaignServices
{
    [ServiceContract(Namespace = "TIMM.Website.CampaignServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CampaignReaderService
    {
        /// <summary>
        /// Fetch the list of campaigns to which the current user has Distribution permissions to get access.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public IEnumerable<ToBasicCampaignProperties> GetCurrentUserCampaignsForDistribution()
        {
            // Temporarily return all campaigns without consideration on user's permissions.
            // THIS HAS TO BE CHANGED LATER WHEN CONSIDERING PERMISSIONS            
            IEnumerable<ToBasicCampaignProperties> items = new List<ToBasicCampaignProperties>();

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                IEnumerable<Campaign> campaigns = ws.Repositories.CampaignRepository.GetAllEntities();
                items = AssemblerConfig.GetAssembler<ToBasicCampaignProperties, Campaign>().AssembleFrom(campaigns);
            }

            return items;
        }

        /// <summary>
        /// Fetch the list of campaigns to which the current user has Sales permissions to get access.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public IEnumerable<ToBasicCampaignProperties> GetCurrentUserCampaigns()
        {
            // Temporarily return all campaigns without consideration on user's permissions.
            // THIS HAS TO BE CHANGED LATER WHEN CONSIDERING PERMISSIONS
            IEnumerable<ToBasicCampaignProperties> items = new List<ToBasicCampaignProperties>();

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                IEnumerable<Campaign> campaigns = ws.Repositories.CampaignRepository.GetAllEntities();
                items = AssemblerConfig.GetAssembler<ToBasicCampaignProperties, Campaign>().AssembleFrom(campaigns);
            }

            return items;
        }


        [OperationContract]
        public IEnumerable<ToBasicCampaignProperties> GetAllBySubmapStatus()
        {
            // Temporarily return all campaigns without consideration on user's permissions.
            // THIS HAS TO BE CHANGED LATER WHEN CONSIDERING PERMISSIONS
            IEnumerable<ToBasicCampaignProperties> items = new List<ToBasicCampaignProperties>();

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                IEnumerable<Campaign> campaigns = ws.Repositories.CampaignRepository.GetAllBySubmapStatus(GPS.DomainLayer.Security.LoginMember.CurrentMember);
                items = AssemblerConfig.GetAssembler<ToBasicCampaignProperties, Campaign>().AssembleFrom(campaigns);
            }

            return items;
        }

        [OperationContract]
        public IEnumerable<ToBasicCampaignProperties> GetAllByDMStatus()
        {
            // Temporarily return all campaigns without consideration on user's permissions.
            // THIS HAS TO BE CHANGED LATER WHEN CONSIDERING PERMISSIONS
            IEnumerable<ToBasicCampaignProperties> items = new List<ToBasicCampaignProperties>();

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                IEnumerable<Campaign> campaigns = ws.Repositories.CampaignRepository.GetAllByDMStatus(GPS.DomainLayer.Security.LoginMember.CurrentMember);
                items = AssemblerConfig.GetAssembler<ToBasicCampaignProperties, Campaign>().AssembleFrom(campaigns);
            }

            return items;
        }

        [OperationContract]
        public IEnumerable<ToBasicCampaignProperties> GetAllCampByDMStatusWithoutUser()
        {
            // Temporarily return all campaigns without consideration on user's permissions.
            // THIS HAS TO BE CHANGED LATER WHEN CONSIDERING PERMISSIONS
            IEnumerable<ToBasicCampaignProperties> items = new List<ToBasicCampaignProperties>();

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                IEnumerable<Campaign> campaigns = ws.Repositories.CampaignRepository.GetAllCampByDMStatusWithoutUser();
                items = AssemblerConfig.GetAssembler<ToBasicCampaignProperties, Campaign>().AssembleFrom(campaigns);
            }

            return items;
        }

        [OperationContract]
        public IEnumerable<ToBasicCampaignProperties> GetAllBySubmapStatusWithoutUser()
        {
            // Temporarily return all campaigns without consideration on user's permissions.
            // THIS HAS TO BE CHANGED LATER WHEN CONSIDERING PERMISSIONS
            IEnumerable<ToBasicCampaignProperties> items = new List<ToBasicCampaignProperties>();

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                IEnumerable<Campaign> campaigns = ws.Repositories.CampaignRepository.GetAllBySubmapStatusWithoutUser();
                items = AssemblerConfig.GetAssembler<ToBasicCampaignProperties, Campaign>().AssembleFrom(campaigns);
            }

            return items;
        }

        [OperationContract]
        public ToBasicCampaignProperties GetCampaignForEdit(int id) 
        {
            ToBasicCampaignProperties campaignProperties = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(id);
                campaignProperties = AssemblerConfig.GetAssembler<ToBasicCampaignProperties, Campaign>().AssembleFrom(campaign);
            }

            return campaignProperties;
        }

        [OperationContract]
        public int[] GetCurrentDate()
        {
            int[] date = new int[3];
            date[0] = System.DateTime.Now.Year;
            date[1] = System.DateTime.Now.Month;
            date[2] = System.DateTime.Now.Day;
            return date;          
        }

        [OperationContract]
        public ToCampaign GetCampaignById(int id)
        {
            ToCampaign toCampaign = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(id);
                toCampaign = AssemblerConfig.GetAssembler<ToCampaign, Campaign>().AssembleFrom(campaign);
            }

            return toCampaign;
        }

        [OperationContract]
        public ToCampaign GetCampaignByIdForDM(int id)
        {
            ToCampaign toCampaign = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(id);
                campaign.Users = null;
                if (campaign.SubMaps != null)
                {
                    foreach (SubMap sm in campaign.SubMaps)
                    {
                        if (sm.DistributionMaps != null)
                        {
                            foreach (GPS.DomainLayer.Entities.DistributionMap dm in sm.DistributionMaps)
                            {                                 
                                dm.Tasks = null;   
                            }
 
                        }
                    }
                }
                toCampaign = AssemblerConfig.GetAssembler<ToCampaign, Campaign>().AssembleFrom(campaign);
            }

            return toCampaign;
        }


        [OperationContract]
        public ToCampaign GetCampaignByIdForDMList(int id)
        {
            ToCampaign toCampaign = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(id);
                campaign.CampaignCRouteImporteds = null;
                campaign.CampaignFiveZipImporteds = null;                
                campaign.CampaignTractImporteds = null;
                campaign.CampaignBlockGroupImporteds = null;                
                campaign.Users = null;
                if (campaign.SubMaps != null)
                {
                    foreach (SubMap sm in campaign.SubMaps)
                    {
                        sm.SubMapRecords = null;
                        sm.SubMapCoordinates = null;
                        if (sm.DistributionMaps != null)
                        {
                            foreach (GPS.DomainLayer.Entities.DistributionMap dm in sm.DistributionMaps)
                            {
                                dm.DistributionMapCoordinates = null;
                                dm.DistributionMapRecords = null;
                                dm.Tasks = null;
                            }

                        }
                    }
                }
                toCampaign = AssemblerConfig.GetAssembler<ToCampaign, Campaign>().AssembleFrom(campaign);
            }

            return toCampaign;
        }

        [OperationContract]
        public string GetCamNameByTaskId(int tid) {
            string camName = "";
            Campaign campaign = null;
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                campaign = ws.Repositories.CampaignRepository.GetCamNameByTaskId(tid);
                camName = campaign.Name;
            }
            return camName;
        }

        [OperationContract]
        public string[] GetCamNameByTasks(int[] taskids)
        {
            string[] campaignNames = null;
           
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                campaignNames = ws.Repositories.CampaignRepository.GetCamNameByTasks(taskids);
            }
            return campaignNames;
        }

        [OperationContract]
        public string[] GetCamNameByReports(int[] taskids)
        {
            string[] campaignNames = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                campaignNames = ws.Repositories.CampaignRepository.GetCamNameByReports(taskids);
            }
            return campaignNames;
        }

        [OperationContract]
        public ToPrintCampaign GetPrintCampaign(int Id)
        {
            try
            {
                ToPrintCampaign printCampaign = null;
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(Id);

                    if (campaign != null)
                    {
                        printCampaign = AssemblerConfig.GetAssembler<ToPrintCampaign, Campaign>().AssembleFrom(campaign);
                    }
                    
                }
                //List<ToPrintSubMap> list = printCampaign.SubMaps.ToList<ToPrintSubMap>();
                //ToPrintSubMap sm = list.Where<ToPrintSubMap>(s => s.Id == -402788626).First<ToPrintSubMap>();
                //list.Remove(sm);
                //ToPrintSubMap sm1 = list.Where<ToPrintSubMap>(s => s.Id == -1942310923).First<ToPrintSubMap>();
                //list.Remove(sm1);
                //foreach (ToPrintSubMap tsm in list)
                //{
                //    tsm.FiveZipAreas = new ToPrintArea[0];
                //}

                //printCampaign.SubMaps = list.ToArray<ToPrintSubMap>();
                
                //string json = Newtonsoft.Json.JsonConvert.SerializeObject(printCampaign);

                return printCampaign;
            }
            catch (Exception ex)
            {
                ILog logger = LogManager.GetLogger(GetType());
                logger.Error("WCF Unhandle Error", ex);
                return null;
            }
        }
    }
}
