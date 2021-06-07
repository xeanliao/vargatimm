using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using GPS.DataLayer;
using GPS.Website.TransferObjects;
using System.Collections;
using GPS.DataLayer.ValueObjects;
using System.Web.UI.WebControls;
using GPS.DomainLayer.Entities;
using GPS.Website.AppFacilities;
using log4net;

namespace GPS.Website.DistributionMapServices
{
    [ServiceContract(Namespace = "TIMM.Website.DistributionMapServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DJReaderService
    {
        [OperationContract]
        public IEnumerable<ToDistributionJob> GetDistributionJobs(int campaignId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Campaign c = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                        IEnumerable<DistributionJob> distributionJobs = ws.Repositories.DistributionJobRepository.GetDistributionJobsForCampaign(c);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToDistributionJob, DistributionJob>().AssembleFrom(distributionJobs);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = LogManager.GetLogger(GetType());
                        logger.Error("WCF Unhandle Error", ex);
                        return null;
                    }
                }
            }
        }

        [OperationContract]
        public ToDistributionJob GetDistributionJob(int distributionjobId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        DistributionJob distributionJob = ws.Repositories.DistributionJobRepository.GetDistributionJob(distributionjobId);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToDistributionJob, DistributionJob>().AssembleFrom(distributionJob);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = LogManager.GetLogger(GetType());
                        logger.Error("WCF Unhandle Error", ex);
                        return null;
                    }
                }
            }
        }
    }
}
