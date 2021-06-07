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
using GPS.DomainLayer.Distribution;
using log4net;

namespace GPS.Website.DistributionMapServices
{
    [ServiceContract(Namespace = "TIMM.Website.DistributionMapServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DJWriterService
    {
        /// <summary>
        /// Save Drivers to the selected distribution job
        /// </summary>
        /// <param name="toDistributionJob"></param>
        [OperationContract]
        public void SaveDrivers(ToDistributionJob toDistributionJob)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        DistributionJob dj = ws.Repositories.DistributionJobRepository.GetDistributionJob(toDistributionJob.Id);
                        IList<DriverAssignment> drivers = new List<DriverAssignment>();
                        foreach (ToDriver driver in toDistributionJob.DriverAssignments)
                        {
                            DriverAssignment d = DistributionJobFactory.CreateDriverFromUser(dj, new UserRepository().GetUser(driver.LoginUserId));
                            drivers.Add(d);
                        }
                        dj.ReplaceDriversWith(drivers);
                        ws.Repositories.DistributionJobRepository.UpdateDistributionJob(dj);
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = log4net.LogManager.GetLogger(typeof(DJWriterService));
                        logger.Error("WCF Unhandl Error", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Save Auditor to the selected distribution job
        /// </summary>
        /// <param name="toDistributionJob"></param>
        [OperationContract]
        public void SaveAuditor(ToDistributionJob toDistributionJob)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        DistributionJob dj = ws.Repositories.DistributionJobRepository.GetDistributionJob(toDistributionJob.Id);
                        if (null != toDistributionJob.AuditorAssignment)
                        {
                            AuditorAssignment auditor = DistributionJobFactory.CreateAuditorFromUser(dj, new UserRepository().GetUser(toDistributionJob.AuditorAssignment.LoginUserId));
                            dj.AssignAuditor(auditor);
                        }
                        else
                        {
                            dj.RemoveAuditor();
                        }
                        ws.Repositories.DistributionJobRepository.UpdateDistributionJob(dj);
                        tx.Commit();
                    }
                    catch (Exception ex) { 
                        tx.Rollback();
                    ILog logger = log4net.LogManager.GetLogger(typeof(DJWriterService));
                    logger.Error("WCF Unhandl Error", ex);
                    }
                }
            }
        }
        
        /// <summary>
        /// Save Walkers to the selected distribution job
        /// </summary>
        /// <param name="toDistributionJob"></param>
        [OperationContract]
        public void SaveWalkers(ToDistributionJob toDistributionJob)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        DistributionJob dj = ws.Repositories.DistributionJobRepository.GetDistributionJob(toDistributionJob.Id);
                        IList<WalkerAssignment> walkers = new List<WalkerAssignment>();
                        foreach (ToWalker walker in toDistributionJob.WalkerAssignments)
                        {
                            WalkerAssignment w = DistributionJobFactory.CreateWalkerFromUser(dj, new UserRepository().GetUser(walker.LoginUserId));
                            walkers.Add(w);
                        }
                        dj.ReplaceWalkersWith(walkers);
                        ws.Repositories.DistributionJobRepository.UpdateDistributionJob(dj);
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        
                        ILog logger = log4net.LogManager.GetLogger(typeof(DJWriterService));
                        logger.Error("WCF Unhandl Error", ex);
                    }
                }
            }
        }
        
        /// <summary>
        /// Save Gtus for Walkers, Drivers and Auditor
        /// </summary>
        /// <param name="toDistributionJob"></param>
        [OperationContract]
        public void SaveGtus(ToDistributionJob toDistributionJob)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        DistributionJob dj = ws.Repositories.DistributionJobRepository.GetDistributionJob(toDistributionJob.Id);
                        if (null != toDistributionJob.AuditorAssignment)
                        {
                            if (toDistributionJob.AuditorAssignment.LoginUserId == dj.AuditorAssignment.LoginUser.Id)
                            {
                                dj.AssignGtuToAuditor(ws.Repositories.GtuRepository.GetGtu(toDistributionJob.AuditorAssignment.UniqueID));
                            }
                        }

                        foreach (ToWalker towalker in toDistributionJob.WalkerAssignments)
                        {
                            foreach (WalkerAssignment walker in dj.WalkerAssignments)
                            {
                                if (towalker.LoginUserId == walker.LoginUser.Id)
                                {
                                    dj.AssignGtuToWalker(walker, ws.Repositories.GtuRepository.GetGtu(towalker.UniqueID));
                                    break;
                                }
                            }
                        }
                        foreach (ToDriver todriver in toDistributionJob.DriverAssignments)
                        {
                            foreach (DriverAssignment driver in dj.DriverAssignments)
                            {
                                if (todriver.LoginUserId == driver.LoginUser.Id)
                                {
                                    dj.AssignGtuToDriver(driver, ws.Repositories.GtuRepository.GetGtu(todriver.UniqueID));
                                    break;
                                }

                            }
                        }
                        ws.Repositories.DistributionJobRepository.UpdateDistributionJob(dj);
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = log4net.LogManager.GetLogger(typeof(DJWriterService));
                        logger.Error("WCF Unhandl Error", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Add a new Distribution Job
        /// </summary>
        /// <param name="toDistributionJob"></param>
        [OperationContract]
        public int SaveDistributionJob(ToDistributionJob toDistributionJob)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        DistributionJob dj = DistributionJobFactory.CreateDistributionJob(new CampaignRepository().GetEntity(toDistributionJob.CampaignID), toDistributionJob.Name);
                        DistributionJob djback = ws.Repositories.DistributionJobRepository.AddDistributionJob(dj);
                        tx.Commit();
                        return djback.Id;
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = LogManager.GetLogger(GetType());
                        logger.Error("WCF Unhandle Error", ex);
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toDistributionJob"></param>
        [OperationContract]
        public void UpdateDistributionJob(ToDistributionJob toDistributionJob)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        DistributionJob dj = ws.Repositories.DistributionJobRepository.GetDistributionJob(toDistributionJob.Id);
                        dj.Name = toDistributionJob.Name;
                        ws.Repositories.DistributionJobRepository.UpdateDistributionJob(dj);
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();

                        ILog logger = LogManager.GetLogger(GetType());
                        logger.Error("WCF Unhandle Error", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Delete Distribution Job
        /// </summary>
        /// <param name="toDistributionJob"></param>
        [OperationContract]
        public void DeleteDistributionJob(ToDistributionJob toDistributionJob)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        DistributionJob dj = ws.Repositories.DistributionJobRepository.GetDistributionJob(toDistributionJob.Id);
                        ws.Repositories.DistributionJobRepository.DeleteDistributionJob(dj);
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = log4net.LogManager.GetLogger(typeof(DJWriterService));
                        logger.Error("WCF Unhandl Error", ex);
                    }
                }
            }
        }
    }
}
