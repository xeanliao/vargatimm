using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using GPS.DataLayer;
using System.Collections.Generic;
using GPS.Website.TransferObjects;
using GPS.DomainLayer.Entities;
using GPS.Website.AppFacilities;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Area;
using log4net;

namespace GPS.Website.DistributionMapServices
{
    [ServiceContract(Namespace = "TIMM.Website.DistributionMapServices")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DMReaderService
    {
        [OperationContract]
        public IEnumerable<ToSubMap> GetSubMaps(int campaignId)
        {
            CampaignRepository camRep = new CampaignRepository();
            Campaign c = camRep.GetEntity(campaignId);
            return AssemblerConfig.GetAssembler<ToSubMap, SubMap>().AssembleFrom(c.SubMaps);

        }

        [OperationContract]
        public ToSubMap GetSubMap(int id)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                SubMap subMap = ws.Repositories.SubMapRepository.GetEntity(id);
                return AssemblerConfig.GetAssembler<ToSubMap, SubMap>().AssembleFrom(subMap);
            }
        }

        [OperationContract]
        public IEnumerable<ToDistributionMap> GetDistributionMaps(int campaignId)
        {
            CampaignRepository camRep = new CampaignRepository();
            Campaign c = camRep.GetEntity(campaignId);

            List<GPS.DomainLayer.Entities.DistributionMap> dmaps = new List<GPS.DomainLayer.Entities.DistributionMap>();
                       
            foreach (SubMap sm in c.SubMaps)
            {               
                foreach(GPS.DomainLayer.Entities.DistributionMap dmap in sm.DistributionMaps)
                {
                    dmaps.Add(dmap);
                }
            }

            return AssemblerConfig.GetAssembler<ToDistributionMap, GPS.DomainLayer.Entities.DistributionMap>().AssembleFrom(dmaps);
        }

        [OperationContract]
        public ToDistributionMap GetDistributionMapsById(int id)
        {
            DistributionMapRepository dr = new DistributionMapRepository();
            GPS.DomainLayer.Entities.DistributionMap dmap = dr.GetEntity(id);
          
            return AssemblerConfig.GetAssembler<ToDistributionMap, GPS.DomainLayer.Entities.DistributionMap>().AssembleFrom(dmap);
        }



        [OperationContract]
        public ToPrintCampaign GetPrintCampaign(int Id)
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
            
            return printCampaign;
        }

        [OperationContract]
        public ToPrintDistributionMap GetPrintDM(int Id)
        {
            ToPrintDistributionMap printDM = null;
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                GPS.DomainLayer.Entities.DistributionMap dm = ws.Repositories.DistributionMapRepository.GetEntity(Id);
                if (dm != null)
                {
                    printDM = AssemblerConfig.GetAssembler<ToPrintDistributionMap, GPS.DomainLayer.Entities.DistributionMap>().AssembleFrom(dm);
                }
            }

            return printDM;
        }


        [OperationContract]
        public List<ToArea> GetCustomArea()
        {
            List<ToArea> areas = new List<ToArea>();
            CustomAreaRepository repository = new CustomAreaRepository();
            List<CustomArea> customAreas = repository.GetCustomAreas();
            foreach (CustomArea customArea in customAreas)
            {
                MapArea area = new MapArea(customArea);
                ToArea toarea = AssemblerConfig.GetAssembler<ToArea, MapArea>().AssembleFrom(area);

                areas.Add(toarea);
            }
            return areas;
        }


        [OperationContract]
        public List<ToArea> GetCustomAreaByBox(int[] boxIds)
        {
            List<ToArea> areas = new List<ToArea>();
            CustomAreaRepository repository = new CustomAreaRepository();
            foreach (int boxId in boxIds)
            {
                List<CustomArea> customAreas = repository.GetCustomAreas(boxId);
                foreach (CustomArea customArea in customAreas)
                {
                    MapArea area = new MapArea(customArea);
                    ToArea toarea = AssemblerConfig.GetAssembler<ToArea, MapArea>().AssembleFrom(area);

                    areas.Add(toarea);
                }
            }

            NdAddressRepository rs = new NdAddressRepository();
            foreach (int boxId in boxIds)
            {
                IList<NdAddress> ndAddresses = rs.GetNdAddresses(boxId);
                foreach (NdAddress nda in ndAddresses)
                {
                    MapArea area = new MapArea(nda);
                    ToArea toarea = AssemblerConfig.GetAssembler<ToArea, MapArea>().AssembleFrom(area);

                    areas.Add(toarea);
                }
            }
            return areas;
        }

        [OperationContract]
        public ToTask GetPrintTask(int Id)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                try
                {
                    Task task = ws.Repositories.TaskRepository.GetTask(Id);
                    return AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(task);
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
}
