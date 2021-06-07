using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using GPS.DataLayer;
using GPS.DataLayer.DataInfrastructure;
using GPS.DomainLayer.Area;
using GPS.DomainLayer.Area.Addresses;
using GPS.DomainLayer.Area.AreaMerge;
using GPS.DomainLayer.Campaign;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.RepositoryInterfaces.BulkRepositories;
using GPS.DomainLayer.Security;
using GPS.Website.AppFacilities;
using GPS.Website.CampaignServices.DataContracts;
using GPS.Website.TransferObjects;
using DomainLayer = GPS.DomainLayer;
using System.Data.SqlClient;
using GPS.Utilities;
using log4net;
// using System.Threading.Tasks; 

namespace GPS.Website.CampaignServices
{

    /// <summary>
    /// <see cref="CampaignWriterService"/> is the interface for interaction with
    /// browser-side Javascript code, responsible for business logic related to
    /// changes to campaigns.
    /// </summary>
    [ServiceContract(Namespace = "TIMM.Website.CampaignServices")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CampaignWriterService
    {
        public bool LoginMemberCheck()
        {
            if (LoginMember.CurrentMember == null || new UserRepository().GetUser(LoginMember.CurrentMember.UserName) == null)
            {
                return true;
            }
            return false;
        }

        #region Campaign

        [OperationContract]
        [FaultContract(typeof(FaultException<string>))]
        public void DeleteCampaigns(Int32[] campaignIds)
        {
            if (LoginMemberCheck())
            {
                MyException e = new MyException("Illegle user!");
            }

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
                { 
                    foreach (Int32 id in campaignIds)
                    {
                        GPS.DomainLayer.Entities.Campaign c = ws.Repositories.CampaignRepository.GetEntity(id);
                        if (null != c)
                        {
                            
                            // Delete CampaignBlockGroupImported
                            //
                            //bws.Repositories.BulkCampaignBGRepository.DeleteAllCampaignBlockGroupImportedsOfCampaign(c.Id);

                            // Delete RadiusRecord
                            //

                            //delete automatic
                            //foreach (Address addr in c.Addresses)
                            //{
                            //    foreach (Radiuse r in addr.Radiuses)
                            //    {
                            //        bws.Repositories.BulkRadiusRecordRepository.DeleteRadiusRecords(r.Id);
                            //    }
                            //}

                            // Delete DistributionJob
                            //
                            IEnumerable<DistributionJob> djs = ws.Repositories.DistributionJobRepository.GetDistributionJobsForCampaign(c);
                            foreach (DistributionJob dj in djs)
                            {
                                ws.Repositories.DistributionJobRepository.DeleteDistributionJob(dj);
                            }

                            // Delete SubMapCoordinate
                            //
                            //foreach (SubMap sm in c.SubMaps)
                            //{
                            //    bws.Repositories.BulkSubMapCoordinateRepository.DeleteBySubMap(sm.Id);
                            //}


                            // Create backup for Campaign
                            //
                            CampaignBackup cb = new CampaignBackup();
                            cb.AreaDescription = c.AreaDescription;
                            cb.ClientCode = c.ClientCode;
                            cb.ClientName = c.ClientName;
                            cb.ContactName = c.ContactName;
                            cb.CustemerName = c.CustemerName;
                            cb.Date = c.Date;
                            cb.Description = c.Description;
                            cb.Id = c.Id;
                            cb.Latitude = c.Latitude;
                            cb.Longitude = c.Longitude;
                            cb.Logo = c.Logo;
                            cb.Name = c.Name;
                            cb.Sequence = c.Sequence;
                            cb.UserName = c.UserName;
                            cb.ZoomLevel = c.ZoomLevel;
                            cb.IPAddress = HttpContext.Current.Request.UserHostAddress;
                            cb.OperationTime = System.DateTime.Now;
                            if (HttpContext.Current.Session["USER"] != null)
                            {
                                User user = (User)HttpContext.Current.Session["USER"];
                                cb.OperationUser = user.UserName;
                            }
                            ws.Repositories.CampaignBackupRepository.Create(cb);


                            // Delete Campaign and other cascaded children objects
                            //
                            ws.Repositories.CampaignRepository.Delete(id);

                        }
                    }

                }

            }

        }

        /// <summary>
        /// Before updating campaign properties, this method helps check if the 
        /// new Sequence specified by the user is valid.
        /// </summary>
        /// <param name="campaignProperties">A <see cref="BasicCampaignProperties"/></param>
        /// <returns>True if the new Sequence is valid, or false if not.</returns>
        /// <remarks>
        /// If the campaign properties are same as the existing and the new 
        /// Sequence is also same as the existing sequence, then the new 
        /// Sequence is regarded as valid.
        /// 
        /// If the the campaign properties are not same as the existing, and ...
        /// </remarks>
        [OperationContract]
        public Boolean IsValidSequence(BasicCampaignProperties campaignProperties)
        {
            return true;
        }

        [OperationContract]
        public ToBasicCampaignProperties CreateNewCampaign(ToBasicCampaignProperties campaignProperties)
        {
            if (LoginMemberCheck())
            {
                MyException e = new MyException("Illegle user!");
                throw e;
            }

            Campaign c = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        User currUser = new User();
                        c = AssemblerConfig.GetAssembler<Campaign, ToBasicCampaignProperties>().AssembleFrom(campaignProperties);
                        //if (c.UserName == string.Empty)
                        //{
                        //    currUser = LoginMember.CurrentMember;
                        //    c.UserName = LoginMember.CurrentMember.UserName;
                        //}
                        //else
                        //{
                        //    currUser = new UserRepository().GetUser(c.UserName);
                        //}
                        c.CreatorName = LoginMember.CurrentMember.UserName;
                        if (c.UserName == string.Empty)
                        {
                            c.UserName = LoginMember.CurrentMember.UserName;
                        }
                        currUser = new UserRepository().GetUser(c.UserName);

                        StatusInfo si = new StatusInfo(0);
                        c.Users = new Dictionary<User, StatusInfo>();
                        c.Users.Add(currUser, si);
                        CampaignManager.CreateNewCampaign(c, ws.Repositories.CampaignRepository);
                        
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw new MyException("Failed to create the campaign.", ex);
                    }
                }
            }

            ToBasicCampaignProperties toCampaign = AssemblerConfig.GetAssembler<ToBasicCampaignProperties, Campaign>().AssembleFrom(c);
            return toCampaign;
        }

        /// <summary>
        /// Update the basic properties of specified campaign if the campaign exists.
        /// </summary>
        /// <param name="campaignProperties">
        /// The new basic properties of the campaign.
        /// </param>
        [OperationContract]
        public ToBasicCampaignProperties UpdateCampaignProperties(ToBasicCampaignProperties campaignProperties)
        {
            if (LoginMemberCheck())
            {
                MyException e = new MyException("Illegle user!");
                throw e;
            }

            DomainLayer.Entities.Campaign campaign = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                // Get the campaign object
                //
                campaign = ws.Repositories.CampaignRepository.GetEntity(campaignProperties.Id);

                if (null != campaign)
                {
                    try
                    {
                        // Set new campaign properties
                        //
                        campaign.ClientName = campaignProperties.ClientName;
                        campaign.ContactName = campaignProperties.ContactName;
                        campaign.ClientCode = campaignProperties.ClientCode;
                        campaign.AreaDescription = campaignProperties.AreaDescription;
                        campaign.Date = DateTime.Parse(campaignProperties.Date);
                        campaign.Sequence = campaignProperties.Sequence;
                        campaign.Logo = campaignProperties.Logo;

                        if (campaignProperties.UserName != null && campaignProperties.UserName != campaign.UserName)
                        {
                            campaign.UserName = campaignProperties.UserName;
                            User currUser = new UserRepository().GetUser(campaign.UserName);
                            StatusInfo si = new StatusInfo(0);
                            campaign.Users.Clear();
                            campaign.Users.Add(currUser, si);
                        }


                        // Update campaign to database
                        //
                        ws.Repositories.CampaignRepository.Update(campaign);
                    }
                    catch (Exception ex)
                    {
                        ILog logger = LogManager.GetLogger(GetType());
                        logger.Error("WCF Unhandle Error", ex);
                        throw new MyException("Failed to edit the campaign.", ex); 
                    }
                }
            }

            return AssemblerConfig.GetAssembler<ToBasicCampaignProperties, Campaign>().AssembleFrom(campaign);
        }

        [OperationContract]
        public void SaveCampaignPercentageColors(int campaignId, List<ToCampaignPercentageColor> toColors)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    campaign.CampaignPercentageColors.Clear();
                    foreach (ToCampaignPercentageColor toColor in toColors)
                    {
                        campaign.CampaignPercentageColors.Add(new CampaignPercentageColor()
                        {
                            Min = toColor.Min,
                            Max = toColor.Max,
                            ColorId = toColor.ColorId,
                            Campaign = campaign
                        });
                    }
                    ws.Repositories.CampaignRepository.Update(campaign);
                    ws.Commit();
                }
            }
            
        }

        [OperationContract]
        public void SaveCampaign(ToCampaign toCampaign)
        {
            DataLayer.CampaignRepository campaignRep = new DataLayer.CampaignRepository();
            DomainLayer.Entities.Campaign campaign = campaignRep.GetEntity(toCampaign.Id);
            if (campaign != null)
            {
                campaign.Latitude = toCampaign.Latitude;
                campaign.Longitude = toCampaign.Longitude;
                campaign.ZoomLevel = toCampaign.ZoomLevel;

                campaign.CampaignRecords.Clear();
                // use all submap records to save to campaign records
                //foreach (ToAreaRecord record in toCampaign.CampaignRecords)
                //{
                //    campaign.CampaignRecords.Add(new CampaignRecord()
                //    {
                //        Campaign = campaign,
                //        Classification = record.Classification,
                //        AreaId = record.AreaId,
                //        Value = record.Value
                //    });
                //}
                foreach(var submap in campaign.SubMaps)
                {
                    foreach (var item in submap.SubMapRecords)
                    {
                        if (item.Value)
                        {
                            campaign.CampaignRecords.Add(new CampaignRecord()
                            {
                                Campaign = campaign,
                                Classification = (int)item.Classification,
                                AreaId = item.AreaId,
                                Value = item.Value
                            });
                        }
                    }
                }
                
                campaign.CampaignClassifications.Clear();
                foreach (int c in toCampaign.VisibleClassifications)
                {
                    campaign.CampaignClassifications.Add(new CampaignClassification()
                    {
                        Campaign = campaign,
                        Classification = c
                    });
                }

            }
            campaignRep.Update(campaign);
        }


        [OperationContract]
        public string PublishCampaign(int[]campaignIds, int userId,int status) {
            if (LoginMemberCheck())
            {
                MyException e = new MyException("Illegle user!");
                throw e;
            }

            int i = 0;
            string retStr="";
            DataLayer.UserRepository userRep = new DataLayer.UserRepository();
            DomainLayer.Entities.User user = userRep.GetUser(userId);
            DataLayer.SubMapRepository submapRep = new DataLayer.SubMapRepository();
            while (i < campaignIds.Count())
            {
                DataLayer.CampaignRepository campaignRep = new DataLayer.CampaignRepository();
                
                DomainLayer.Entities.Campaign campaign = campaignRep.GetEntity(campaignIds[i]);

                if (campaign != null) {
                    int stage=0;

                    #region check holes in submap and dmap
                    //CampaignManager.CalculateHolesInSubMap(campaign);
                    //CampaignManager.CalculateHolesInDMap(campaign);
                    //bool haveHoles = false;
                    //foreach (var submap in campaign.SubMaps)
                    //{
                    //    if (submap.Holes != null && submap.Holes.Count > 0)
                    //    {
                    //        haveHoles = true;
                    //        break;
                    //    }
                    //    foreach (var dmap in submap.DistributionMaps)
                    //    {
                    //        if (dmap.Holes != null && dmap.Holes.Count > 0)
                    //        {
                    //            haveHoles = true;
                    //            break;
                    //        }
                    //    }
                    //    if (haveHoles)
                    //    {
                    //        break;
                    //    }
                    //}
                    //if (haveHoles)
                    //{
                    //    string name = "";
                    //    name = Campaign.ConstructCompositeName(campaign.Date, campaign.ClientCode, campaign.AreaDescription, campaign.CreatorName, campaign.Sequence);
                    //    retStr = string.Format("{0}{1} has holes in Sub Map, and can't be published! <div>", retStr, name);
                    //    i++;
                    //    continue;
                    //}
                    #endregion
                    

                    if (campaign.Users != null && campaign.Users.Count > 0)
                    {
                        stage=status - campaign.Users.Values.FirstOrDefault().Status;
                        if (Math.Abs(stage) != 1)
                        {
                            //ret = "Unable to process the request. Wait a few minutes or refresh current page";
                            i++;
                            continue;
                        }
                    }

                    if (stage == 1)
                    {

                        int vcRet = ValidatorChildren(campaign, (status == 2));
                        if (vcRet != 0)
                        {
                            string name = "";
                            name = Campaign.ConstructCompositeName(campaign.Date, campaign.ClientCode, campaign.AreaDescription, campaign.CreatorName, campaign.Sequence);
                            if (vcRet == 1)
                                retStr = retStr + name + " has no Sub Map, and can't be published!" + "<div>";
                            else
                                retStr = retStr + name + " has no Distribution Map, and can't be published!" + "<div>";
                            i++;
                            continue;
                        }
                    }

                    StatusInfo si = new StatusInfo(status);
                    campaign.Users.Clear();
                    //if not publish(dismiss) campaign, clear dms under the campaign
                    if (status==0)
                    {
                        if(campaign.SubMaps!=null){
                            IList<SubMap> subs = campaign.SubMaps;        
                                int x = 0;
                                while(x<subs.Count){
                                    if (subs[x].DistributionMaps!=null) {
                                        int y = 0;
                                        while(y<subs[x].DistributionMaps.Count){
                                            if (subs[x].DistributionMaps[y].DistributionMapCoordinates!=null)
                                            {
                                                subs[x].DistributionMaps[y].DistributionMapCoordinates.Clear();
                                            }
                                            if (subs[x].DistributionMaps[y].DistributionMapRecords!=null)
                                            {
                                                subs[x].DistributionMaps[y].DistributionMapRecords.Clear();
                                            }
                                            y++;
                                        }
                                        subs[x].DistributionMaps.Clear();
                                    }
                                    x++;
                                }
                        }
                    }

                    //if publish campaign to monitor, create one task for each dm under this campaign
                    int dmIdTem = 0;
                    string dmNameTem = "";
                    Task task = new Task();
                    if (status == 2)
                    {
                        if (campaign.SubMaps != null)
                        {
                            IList<SubMap> subs = campaign.SubMaps;
                            int x = 0;
                            while (x < subs.Count)
                            {
                                if (subs[x].DistributionMaps != null)
                                {
                                    int y = 0;
                                    while (y < subs[x].DistributionMaps.Count)
                                    {
                                        DataLayer.RepositoryImplementations.TaskRepository tRep = new DataLayer.RepositoryImplementations.TaskRepository();
                                        dmIdTem = subs[x].DistributionMaps[y].Id;
                                        dmNameTem = subs[x].DistributionMaps[y].Name;
                                        //task.AuditorId = null;
                                        task.Date = DateTime.Now;
                                        task.DmId = dmIdTem;
                                        task.Name = dmNameTem;
                                        task.Status = 0;
                                        task.Taskgtuinfomappings = new List<Taskgtuinfomapping>();
                                        task.Tasktimes = new List<TaskTime>();
                                        task.AuditorId = 1;
                                        tRep.AddTask(task);
                                        y++;
                                    }
                                }
                                x++;
                            }
                        }    
                    }

                    campaign.Users.Add(user, si);
                    campaignRep.Update(campaign);                                        
                }
                i++;
            }
            return retStr;
        }

        [OperationContract]
        public void CopyCampaigns(Int32[] campaignIds)
        {
            if (LoginMemberCheck())
            {
                MyException e = new MyException("Illegle user!");
                throw e;
            }

            foreach (Int32 id in campaignIds)
            {
                CopyCampaigns(id);

            }

        }


        public void CopyCampaigns(int campaignId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                //GPS.DomainLayer.Entities.Campaign toCampaign = ws.Repositories.CampaignRepository.GetEntity(id);
                using (ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        DataLayer.CampaignRepository campaignRep = new DataLayer.CampaignRepository();
                        DomainLayer.Entities.Campaign toCampaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                        if (toCampaign != null)
                        {
                            GPS.DomainLayer.Entities.Campaign campaign = new Campaign();
                            ToBasicCampaignProperties campaignProperties = new ToBasicCampaignProperties();
                            campaignProperties.ClientName = toCampaign.ClientName;
                            campaignProperties.ContactName = toCampaign.ContactName;
                            campaignProperties.ClientCode = toCampaign.ClientCode;
                            campaignProperties.AreaDescription = toCampaign.AreaDescription;
                            campaignProperties.Date = toCampaign.Date.ToString();
                            campaign = AssemblerConfig.GetAssembler<Campaign, ToBasicCampaignProperties>().AssembleFrom(campaignProperties);
                            campaign.UserName = toCampaign.UserName;
                            User user = new UserRepository().GetUser(toCampaign.UserName);
                            StatusInfo si = new StatusInfo(0);
                            campaign.Users = new Dictionary<User, StatusInfo>();
                            campaign.Users.Add(user, si);
                            campaign.CreatorName = toCampaign.CreatorName;

                            CampaignManager.CreateNewCampaign(campaign, ws.Repositories.CampaignRepository);

                            campaign.Latitude = toCampaign.Latitude;
                            campaign.Longitude = toCampaign.Longitude;
                            campaign.ZoomLevel = toCampaign.ZoomLevel;

                            //CampaignRecord
                            campaign.CampaignRecords = new List<CampaignRecord>();
                            foreach (CampaignRecord record in toCampaign.CampaignRecords)
                            {
                                campaign.CampaignRecords.Add(new CampaignRecord()
                                {
                                    Campaign = campaign,
                                    Classification = record.Classification,
                                    AreaId = record.AreaId,
                                    Value = record.Value
                                });
                            }

                            campaign.CampaignClassifications = new List<CampaignClassification>();
                            foreach (CampaignClassification c in toCampaign.CampaignClassifications)
                            {
                                campaign.CampaignClassifications.Add(new CampaignClassification()
                                {
                                    Campaign = campaign,
                                    Classification = c.Classification
                                });
                            }

                            //address
                            campaign.Addresses = new List<Address>();
                            foreach (Address addr in toCampaign.Addresses)
                            {
                                Address ar = new Address();
                                //ar.CampaignId = campaign.Id;
                                ar.Campaign = campaign;
                                ar.Address1 = addr.Address1;
                                ar.Color = addr.Color;
                                ar.Latitude = addr.Latitude;
                                ar.Longitude = addr.Longitude;
                                ar.Picture = addr.Picture;
                                ar.ZipCode = addr.ZipCode;
                                ar.OriginalLatitude = addr.OriginalLatitude;
                                ar.OriginalLongitude = addr.OriginalLongitude;
                                ar.Radiuses = new List<Radiuse>();

                                if (addr.Radiuses != null && addr.Radiuses.Count > 0)
                                {
                                    //List<Radiuse> radius = new List<Radiuse>();
                                    foreach (Radiuse r in addr.Radiuses)
                                    {
                                        Radiuse rd = new Radiuse();
                                        rd.Address = ar;
                                        //rd.AddressId = ar.Id;
                                        rd.IsDisplay = r.IsDisplay;
                                        rd.Length = r.Length;
                                        rd.LengthMeasuresId = r.LengthMeasuresId;

                                        rd.RadiusRecords = new List<RadiusRecord>();
                                        //if (r.RadiusRecords != null && r.RadiusRecords.Count > 0)
                                        //{
                                        //    foreach (RadiusRecord rrd in r.RadiusRecords)
                                        //    {
                                        //        rd.RadiusRecords.Add(new RadiusRecord()
                                        //        {
                                        //            
                                        //            AreaId = rrd.AreaId,
                                        //            Classification=rrd.Classification,
                                        //            RadiusId=rrd.RadiusId
                                        //        });
                                        //    }
                                        //}
                                        ar.Radiuses.Add(rd);
                                    }

                                }
                                campaign.Addresses.Add(ar);
                            }

                            //
                            if (toCampaign.CampaignBlockGroupImporteds.Count > 0)
                            {
                                campaign.CampaignBlockGroupImporteds = new List<CampaignBlockGroupImported>();
                                foreach (CampaignBlockGroupImported cg in toCampaign.CampaignBlockGroupImporteds)
                                {
                                    campaign.CampaignBlockGroupImporteds.Add(new CampaignBlockGroupImported()
                                    {
                                        BlockGroup = new BlockGroup() { Id = cg.BlockGroup.Id },
                                        Total = cg.Total,
                                        Penetration = cg.Penetration,
                                        Campaign = campaign
                                    });
                                }
                            }




                            if (toCampaign.CampaignCRouteImporteds.Count > 0)
                            {
                                campaign.CampaignCRouteImporteds = new List<CampaignCRouteImported>();
                                foreach (CampaignCRouteImported cbgi in toCampaign.CampaignCRouteImporteds)
                                {
                                    campaign.CampaignCRouteImporteds.Add(new CampaignCRouteImported()
                                    {
                                        PremiumCRoute = new PremiumCRoute() { Id = cbgi.PremiumCRoute.Id },
                                        Total = cbgi.Total,
                                        Penetration = cbgi.Penetration,
                                        PartPercentage = cbgi.PartPercentage,
                                        IsPartModified = cbgi.IsPartModified,
                                        Campaign = campaign
                                    });
                                }
                            }


                            if (toCampaign.CampaignFiveZipImporteds.Count > 0)
                            {
                                campaign.CampaignFiveZipImporteds = new List<CampaignFiveZipImported>();
                                foreach (CampaignFiveZipImported cbgi in toCampaign.CampaignFiveZipImporteds)
                                {
                                    campaign.CampaignFiveZipImporteds.Add(new CampaignFiveZipImported()
                                    {
                                        FiveZipArea = new FiveZipArea() { Id = cbgi.FiveZipArea.Id },
                                        Total = cbgi.Total,
                                        Penetration = cbgi.Penetration,
                                        PartPercentage = cbgi.PartPercentage,
                                        IsPartModified = cbgi.IsPartModified,
                                        Campaign = campaign
                                    });
                                }
                            }

                            if (toCampaign.CampaignTractImporteds.Count > 0)
                            {
                                campaign.CampaignTractImporteds = new List<CampaignTractImported>();
                                foreach (CampaignTractImported cbgi in toCampaign.CampaignTractImporteds)
                                {
                                    campaign.CampaignTractImporteds.Add(new CampaignTractImported()
                                    {
                                        Tract = new Tract() { Id = cbgi.Tract.Id },
                                        Total = cbgi.Total,
                                        Penetration = cbgi.Penetration,
                                        Campaign = campaign
                                    });
                                }
                            }

                            if (toCampaign.CampaignPercentageColors.Count > 0)
                            {
                                campaign.CampaignPercentageColors = new List<CampaignPercentageColor>();
                                foreach (CampaignPercentageColor cbgi in toCampaign.CampaignPercentageColors)
                                {
                                    campaign.CampaignPercentageColors.Add(new CampaignPercentageColor()
                                    {
                                        Campaign = campaign,
                                        ColorId = cbgi.ColorId,
                                        Min = cbgi.Min,
                                        Max = cbgi.Max
                                    });
                                }
                            }




                            //Submap
                            campaign.SubMaps = new List<SubMap>();

                            foreach (SubMap toSubmap in toCampaign.SubMaps)
                            {
                                SubMap sm = new SubMap();
                                sm.Campaign = campaign;
                                sm.ColorB = toSubmap.ColorB;
                                sm.ColorG = toSubmap.ColorG;
                                sm.ColorR = toSubmap.ColorR;
                                sm.ColorString = toSubmap.ColorString;
                                sm.Name = toSubmap.Name;
                                sm.OrderId = toSubmap.OrderId;
                                sm.Penetration = toSubmap.Penetration;
                                sm.Percentage = toSubmap.Percentage;
                                sm.Total = toSubmap.Total;
                                sm.TotalAdjustment = toSubmap.TotalAdjustment;
                                sm.CountAdjustment = toSubmap.CountAdjustment;
                                sm.DistributionMaps = new List<GPS.DomainLayer.Entities.DistributionMap>();
                                sm.SubMapRecords = new List<SubMapRecord>();
                                sm.SubMapCoordinates = new List<SubMapCoordinate>();

                                foreach (SubMapRecord toArea in toSubmap.SubMapRecords)
                                {
                                    SubMapRecord area = new SubMapRecord();
                                    area.SubMap = sm;
                                    area.AreaId = toArea.AreaId;
                                    area.Classification = toArea.Classification;
                                    area.Value = toArea.Value;
                                    area.Relation = new List<List<string>>();
                                    sm.SubMapRecords.Add(area);
                                }

                                foreach (SubMapCoordinate toCoordinate in toSubmap.SubMapCoordinates)
                                {
                                    SubMapCoordinate coordinate = new SubMapCoordinate();
                                    coordinate.SubMap = sm;
                                    coordinate.Latitude = toCoordinate.Latitude;
                                    coordinate.Longitude = toCoordinate.Longitude;
                                    sm.SubMapCoordinates.Add(coordinate);
                                }

                                campaign.SubMaps.Add(sm);
                            }

                            ws.Repositories.CampaignRepository.UpdateCopy(campaign);
                            tx.Commit();

                        }


                    }
                    catch(Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = log4net.LogManager.GetLogger(typeof(CampaignWriterService));
                        logger.Error("WCF Unhandl Error", ex);
                    }

                }
            }


        }

        

        #endregion

        #region Address


        [OperationContract]
        public void DeleteAddress(int campaignId, int addressId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    
                    Address address = campaign.Addresses.Where(t => t.Id == addressId).FirstOrDefault();
                    if (address != null)
                    {
                        campaign.Addresses.Remove(address);
                        ws.Repositories.CampaignRepository.Update(campaign);
                    }
                }
            }
        }

        [OperationContract]
        public void ChangeAddressRadiusDisplay(int radiusId, bool isDisplay)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Radiuse radiuse = ws.Repositories.RadiusRepository.GetEntity(radiusId);
                if (radiuse != null)
                {
                    radiuse.IsDisplay = isDisplay;
                    ws.Repositories.RadiusRepository.Update(radiuse);
                    ws.Commit();
                }
            }
        }

        [OperationContract]
        public List<ToAddressRadius> ChangeAddressRadius(int addressId, double lat, double lon, List<ToAddressRadius> radiuses)
        {
            AddressOperator oper = new AddressOperator();
            ICoordinate center = new Coordinate(lat, lon);
            foreach (ToAddressRadius radius in radiuses)
            {
                radius.Relations = oper.GetRadiusRelations(center, radius.Length, radius.LengthMeasuresId);
            }

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Address address = ws.Repositories.AddressRepository.GetEntity(addressId);

                if (address != null)
                {
                    foreach (ToAddressRadius toRadius in radiuses)
                    {
                        Radiuse radius = address.Radiuses.Where(t => t.Id == toRadius.Id).SingleOrDefault();
                        if (radius != null)
                        {
                            radius.Length = toRadius.Length;
                            radius.LengthMeasuresId = toRadius.LengthMeasuresId;
                            List<RadiusRecord> records = new List<RadiusRecord>();
                            foreach (int c in toRadius.Relations.Keys)
                            {
                                foreach (int id in toRadius.Relations[c].Keys)
                                {
                                    records.Add(new RadiusRecord()
                                    {
                                        AreaId = id,
                                        Classification = (GPS.DomainLayer.Enum.Classifications)c,
                                        Radiuse = radius
                                        //RadiusId = toRadius.Id
                                    });
                                }
                            }
                            if (records.Count > 0)
                            {
                                using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
                                {
                                    using (ITransaction tx = bws.BeginTransaction())
                                    {
                                        try
                                        {
                                            bws.Repositories.BulkRadiusRecordRepository.DeleteRadiusRecords(toRadius.Id);
                                            bws.Repositories.BulkRadiusRecordRepository.InsertEntityList(records);
                                            tx.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            ILog logger = LogManager.GetLogger(GetType());
                                            logger.Error("WCF Unhandle Error", ex);
                                            tx.Rollback();
                                        }
                                    }
                                }
                            }

                        }
                    }
                    address.Latitude = lat;
                    address.Longitude = lon;
                    ws.Repositories.AddressRepository.Update(address);
                    ws.Commit();
                }
            }

            return radiuses;
        }

        private class ParallelAddress : AddressRecord
        {
            public int campaignId;
            public string color;
            public List<MapAddress> mapAddresses;
            public List<AddressRecord> badAddresses;

            public ParallelAddress(AddressRecord record)
            {
                this.StreetLine = record.StreetLine;
                this.PostalCode = record.PostalCode;
            }
        }

        private void InsertCampaignAddress(ParallelAddress address)
        {
            AddressOperator oper = new AddressOperator();
            string street = address.StreetLine.Trim();
            string postalCode = address.PostalCode.Trim();
            try
            {
                MapAddress mapaddress = oper.GetAddress(street, postalCode, address.color);

                if (mapaddress != null)
                {
                    List<MapAddress> mapaddressestemp = new List<MapAddress>();
                    mapaddressestemp.Add(mapaddress);
                    // InsertCampaignAddresses(address.campaignId, mapaddressestemp);
                    address.mapAddresses.Add(mapaddress);
                }
                else
                {
                    // retry one more time. 
                    mapaddress = oper.GetAddress(street, postalCode, address.color);
                    if (mapaddress != null)
                    {
                        List<MapAddress> mapaddressestemp = new List<MapAddress>();
                        mapaddressestemp.Add(mapaddress);
                        // InsertCampaignAddresses(address.campaignId, mapaddressestemp);
                        address.mapAddresses.Add(mapaddress);
                    }
                    else
                    {
                        AddressRecord badAddress = new AddressRecord();
                        badAddress.StreetLine = address.StreetLine;
                        badAddress.PostalCode = address.PostalCode;
                        address.badAddresses.Add(badAddress);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogUtils.Error("WCF Unhandle Error", ex);
                AddressRecord badAddress = new AddressRecord();
                badAddress.StreetLine = address.StreetLine;
                badAddress.PostalCode = address.PostalCode;
                address.badAddresses.Add(badAddress); 
            }
        }


        [OperationContract]
        public IEnumerable<ToAddress> UploadAddress(int campaignId, string fileName, string color)
        {
            AddressOperator oper = new AddressOperator();
            List<AddressRecord> badAddresses = new List<AddressRecord>();
            List<ParallelAddress> ParallelAddresses = new List<ParallelAddress>(); 
            List<MapAddress> mapAddresses = new List<MapAddress>();

            AddressRecord[] records = oper.ReadAddressFile(color, HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["addressfilepath"] + fileName));

            Campaign campaign = null;
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);

                int line = 0;
                foreach (AddressRecord record in records)
                {
                    line++;
                    string street = record.StreetLine.Trim();
                    string postalCode = record.PostalCode.Trim();
                    if (string.IsNullOrEmpty(street.Trim()) && string.IsNullOrEmpty(postalCode.Trim())) { continue; }
                    if (string.IsNullOrEmpty(street.Trim()))
                    {
                        MyException e = new MyException("The file contains invalid address! Line:" + line);

                        throw e;
                    }
                    if (string.IsNullOrEmpty(postalCode.Trim()))
                    {
                        MyException e = new MyException("The file contains invalid postal code! Line:" + line);

                        throw e;
                    }
                    // check for duplicate address to existing campaign
                    Address checkdup = campaign.Addresses.SingleOrDefault(
                        a => (a.ZipCode == record.PostalCode.Trim() && (a.Address1 == record.StreetLine.Trim())));
                    ParallelAddress checkdup2 = ParallelAddresses.SingleOrDefault(
                        a => (a.PostalCode.Trim() == record.PostalCode.Trim() && a.StreetLine.Trim() == record.StreetLine.Trim()));

                    if (checkdup == null && checkdup2 == null)
                    {
                        ParallelAddress pAddress = new ParallelAddress(record);
                        pAddress.campaignId = campaignId;
                        pAddress.color = color;
                        pAddress.mapAddresses = mapAddresses;
                        pAddress.badAddresses = badAddresses;

                        ParallelAddresses.Add(pAddress);
                    }
                    else
                    {
                        if (checkdup != null)
                        {
                            Utilities.LogUtils.Info("duplicate existing address" + checkdup.Address1 + ", " + checkdup.ZipCode);
                        }
                        if (checkdup2 != null)
                        {
                            Utilities.LogUtils.Info("duplicate existing address" + checkdup2.StreetLine+ ", " + checkdup2.PostalCode);
                        }
                    }
                    // 
                }
            }

            System.Threading.Tasks.Parallel.ForEach(ParallelAddresses, item => InsertCampaignAddress(item));
            InsertCampaignAddresses(campaignId, mapAddresses);

            if (badAddresses.Count > 0)
            {
                string errormsg = "";
                foreach (AddressRecord record in badAddresses)
                {
                    errormsg += errormsg + "bad address: " + record.StreetLine + ", " + record.PostalCode + "\r\n";
                }

                Utilities.LogUtils.Info(errormsg); 
                throw new MyException(errormsg);
            }

            Assembler<ToAddress, MapAddress> asm = AssemblerConfig.GetAssembler<ToAddress, MapAddress>();
            return asm.AssembleFrom(mapAddresses);
        }


        [OperationContract]
        public ToMonitorAddresses NewMonitorAddress(int did, string street, string zip, string pic)
        {
            AddressOperator oper = new AddressOperator();
            MonitorAddresses address = oper.GetMonitorAddress(street, zip, pic);
            if (address != null)
            {
                InsertMonitorAddresses(did, address);
                Assembler<ToMonitorAddresses, MonitorAddresses> asm = AssemblerConfig.GetAssembler<ToMonitorAddresses, MonitorAddresses>();
                return asm.AssembleFrom(address);
            }
            else
            {
                return null;
            }
        }

        private void InsertMonitorAddresses(int did, MonitorAddresses mapAddresses)
        {
            //List<RadiusRecord> addRecords = new List<RadiusRecord>();
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                //Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                //if (campaign != null)
                //{
                    //foreach (MonitorAddresses mapAddress in mapAddresses)
                    //{
                        MonitorAddresses address = new MonitorAddresses()
                        {
                            Id = mapAddresses.Id,
                            Address1 = mapAddresses.Address1,
                            ZipCode = mapAddresses.ZipCode,
                            //Color = mapAddress.Color,
                            Latitude = mapAddresses.Latitude,
                            Longitude = mapAddresses.Longitude,
                            OriginalLatitude = mapAddresses.OriginalLatitude,
                            OriginalLongitude = mapAddresses.OriginalLongitude,
                            Picture = mapAddresses.Picture,
                            DmId = did
                            //CampaignId = campaign.Id
                        };

                        //List<Radiuse> radiuses = new List<Radiuse>();
                        //foreach (MapAddressRadius mapRadius in mapAddress.Radiuses)
                        //{

                        //    Radiuse addRad = new Radiuse()
                        //    {
                        //        Id = mapRadius.Id,
                        //        AddressId = mapAddress.Id,
                        //        Length = mapRadius.Length,
                        //        LengthMeasuresId = mapRadius.LengthMeasuresId,
                        //        IsDisplay = mapRadius.IsDisplay
                        //    };

                        //    foreach (int c in mapRadius.Relations.Keys)
                        //    {
                        //        foreach (int id in mapRadius.Relations[c].Keys)
                        //        {
                        //            addRecords.Add(new RadiusRecord()
                        //            {
                        //                AreaId = id,
                        //                Classification = (GPS.DomainLayer.Enum.Classifications)c,
                        //                
                        //                RadiusId = addRad.Id
                        //            });
                        //        }
                        //    }
                        //    radiuses.Add(addRad);
                        //}
                        //address.Radiuses = radiuses;
                        //campaign.Addresses.Add(address);
                        ws.Repositories.CampaignRepository.NewMonitorAddress(address);
                        ws.Commit();
                    //}
                    //ws.Repositories.CampaignRepository.Update(campaign);
                    //ws.Commit();
                //}
            }
            //if (addRecords.Count > 0)
            //{
            //    using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
            //    {
            //        using (ITransaction tx = bws.BeginTransaction())
            //        {
            //            try
            //            {
            //                bws.Repositories.BulkRadiusRecordRepository.InsertEntityList(addRecords);
            //                tx.Commit();
            //            }
            //            catch (Exception)
            //            {
            //                tx.Rollback();
            //            }
            //        }
            //    }
            //}
        }

        [OperationContract]
        public void DeleteMonitorAddress(int addressId)
        {
            try
            {
                GPS.DataLayer.MonitorAddressRepository addressRep = new MonitorAddressRepository();
                addressRep.DeleteEntity(addressId);
            }
            catch (Exception ex)
            {
                GPS.Utilities.LogUtils.Error("WCF Unhandle Error", ex);
            }
        }

        [OperationContract]
        public ToAddress NewAddress(int campaignId, string street, string zip, string color, string pic)
        {
            AddressOperator oper = new AddressOperator();
            MapAddress address = oper.GetAddress(street, zip, color, pic);
            if (address != null)
            {
                InsertCampaignAddresses(campaignId, new List<MapAddress>() { address });
                Assembler<ToAddress, MapAddress> asm = AssemblerConfig.GetAssembler<ToAddress, MapAddress>();
                var returnValue = asm.AssembleFrom(address);
                return returnValue;
            }
            else
            {
                return null;
            }
        }

        private void InsertCampaignAddresses(int campaignId, List<MapAddress> mapAddresses)
        {
            List<RadiusRecord> addRecords = new List<RadiusRecord>();
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Dictionary<MapAddress, Address> returnAddress = new Dictionary<MapAddress, Address>();
                Dictionary<MapAddressRadius, Radiuse> returnRadiuse = new Dictionary<MapAddressRadius, Radiuse>();
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    foreach (MapAddress mapAddress in mapAddresses)
                    {
                        Address address = new Address()
                        {
                            //Id = mapAddress.Id,
                            Address1 = mapAddress.Street,
                            ZipCode = mapAddress.ZipCode,
                            Color = mapAddress.Color,
                            Latitude = mapAddress.Latitude,
                            Longitude = mapAddress.Longitude,
                            OriginalLatitude = mapAddress.OriginalLatitude,
                            OriginalLongitude = mapAddress.OriginalLongitude,
                            Picture = mapAddress.Picture,
                            Campaign = campaign
                            //CampaignId = campaign.Id
                        };

                        returnAddress.Add(mapAddress, address);

                        List<Radiuse> radiuses = new List<Radiuse>();
                        address.Radiuses = new List<Radiuse>();
                        foreach (MapAddressRadius mapRadius in mapAddress.Radiuses)
                        {

                            Radiuse addRad = new Radiuse()
                            {
                                //Id = mapRadius.Id,
                                //AddressId = mapAddress.Id,
                                Address = address,
                                Length = mapRadius.Length,
                                LengthMeasuresId = mapRadius.LengthMeasuresId,
                                IsDisplay = mapRadius.IsDisplay
                            };
                            returnRadiuse.Add(mapRadius, addRad);
                            foreach (int c in mapRadius.Relations.Keys)
                            {
                                foreach (int id in mapRadius.Relations[c].Keys)
                                {
                                    addRecords.Add(new RadiusRecord()
                                    {
                                        AreaId = id,
                                        Classification = (GPS.DomainLayer.Enum.Classifications)c,

                                        Radiuse = addRad
                                    });
                                }
                            }
                            radiuses.Add(addRad);
                            address.Radiuses.Add(addRad);
                        }
                        //address.Radiuses = radiuses;
                        
                        campaign.Addresses.Add(address);
                        
                    }
                    ws.Repositories.CampaignRepository.Update(campaign);
                    
                    ws.Commit();

                    //set address id back
                    foreach (var item in returnAddress)
                    {
                        item.Key.Id = item.Value.Id;
                        foreach (var radiuse in item.Key.Radiuses)
                        {
                            radiuse.Id = returnRadiuse[radiuse].Id;
                        }
                    }
                }
            }
            if (addRecords.Count > 0)
            {
                using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
                {

                    bws.Repositories.BulkRadiusRecordRepository.InsertEntityList(addRecords);

                }

            }

            //return the address id to paramters
            

        }

        #endregion

        #region SubMap

        [OperationContract]
        public void DeleteSubMap(int campaignId, int submapId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    var submap = campaign.SubMaps.Where(t => t.Id == submapId).SingleOrDefault();
                    if (submap != null)
                    {
                        campaign.SubMaps.Remove(submap);
                        ws.Repositories.CampaignRepository.Update(campaign);
                        ws.Commit();
                    }
                }
            }
        }

        [OperationContract]
        public void UpdateSubMapDetails(ToSubMap toSubmap)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(toSubmap.CampaignId);
                if (campaign != null)
                {
                    var submap = campaign.SubMaps.Where(t => t.Id == toSubmap.Id).SingleOrDefault();
                    if (submap != null)
                    {
                        submap.Name = toSubmap.Name;
                        submap.ColorR = toSubmap.ColorR;
                        submap.ColorG = toSubmap.ColorG;
                        submap.ColorB = toSubmap.ColorB;
                        submap.ColorString = toSubmap.ColorString;
                        submap.Total = toSubmap.Total;
                        submap.Penetration = toSubmap.Penetration;
                        submap.Percentage = toSubmap.Percentage;
                        submap.TotalAdjustment = toSubmap.TotalAdjustment;
                        submap.CountAdjustment = toSubmap.CountAdjustment;
                        ws.Repositories.CampaignRepository.Update(campaign);
                        ws.Commit();
                    }
                }
            }
        }

        [OperationContract]
        public ToSubMap NewSubMap(ToSubMap toSubmap)
        {
            ToSubMap ret = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(toSubmap.CampaignId);
                if (campaign != null)
                {
                    var submap = new SubMap();

                    submap.Name = toSubmap.Name;
                    submap.ColorR = toSubmap.ColorR;
                    submap.ColorG = toSubmap.ColorG;
                    submap.ColorB = toSubmap.ColorB;
                    submap.ColorString = toSubmap.ColorString;
                    submap.Total = toSubmap.Total;
                    submap.Penetration = toSubmap.Penetration;
                    submap.Percentage = toSubmap.Percentage;
                    submap.TotalAdjustment = toSubmap.TotalAdjustment;
                    submap.CountAdjustment = toSubmap.CountAdjustment;
                    submap.OrderId = toSubmap.OrderId;
                    submap.Campaign = campaign;
                    campaign.SubMaps.Add(submap);

                    ws.Repositories.CampaignRepository.Update(campaign);
                    ws.Commit();
                    toSubmap.Id = submap.Id;
                    ret = toSubmap;

                }
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="submapId"></param>
        /// <param name="toRecords"></param>
        /// <param name="allowAutoFix">tis param is only used for add all shapes. we will merge as all connected area as possible</param>
        /// <returns></returns>
        [OperationContract]
        public ToMergeResult MergeAreas(int campaignId, int submapId, List<ToAreaRecord> toRecords, bool isAddAllShapesToSubMap)
        {
            IEnumerable<AreaRecord> records = AssemblerConfig.GetAssembler<AreaRecord, ToAreaRecord>().AssembleFrom(toRecords);
            MergeOperator oper = new MergeOperator();
            MergeResult result = null;

            var useGeoAPI = ConfigurationManager.AppSettings["UseGeo.API"];
            double[][] basePolygon = null;
            if (isAddAllShapesToSubMap)//only add all shapes to submap need load current submap. this paramter is ture means this UI operate. otherwise false
            {
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                    if (campaign != null)
                    {
                        var submap = campaign.SubMaps.Where(t => t.Id == submapId).SingleOrDefault();
                        if (submap != null && submap.SubMapCoordinates != null && submap.SubMapCoordinates.Count > 0)
                        {
                            basePolygon = new double[submap.SubMapCoordinates.Count][];
                            var submapCoordinates = submap.SubMapCoordinates.OrderBy(i => i.Id).ToList();
                            for (int i = 0; i < submapCoordinates.Count; i++)
                            {
                                basePolygon[i] = new double[2] { submapCoordinates[i].Latitude, submapCoordinates[i].Longitude };
                            }

                        }
                    }
                }
            }
            result = oper.MergeAreasAPI(campaignId, basePolygon, ref records, isAddAllShapesToSubMap);

            #region old merge method
            //result = oper.MergeAreas(campaignId, ref records);
            ////check donut: whether a ring is inside another ring
            //if (result.Locs.Count == 2)
            //{
            //    if (result.Locs[0].Length < result.Locs[1].Length)
            //    {
            //        bool bDonut = GPS.DomainLayer.Area.ShapeMethods.PointInPolygon(result.Locs[1], result.Locs[0][0]);
            //        if (bDonut)
            //            result.Locs.RemoveAt(0);
            //    }
            //    else
            //    {
            //        bool bDonut = GPS.DomainLayer.Area.ShapeMethods.PointInPolygon(result.Locs[0], result.Locs[1][0]);
            //        if (bDonut)
            //            result.Locs.RemoveAt(1);
            //    }
            //}
            #endregion

            

            if (result.Locs.Count >= 2)
            {
                string warning = "CampaignId = " + campaignId + " subMapId = " + submapId;
                int index = 0;

                foreach (double[][] loc in result.Locs)
                {
                    warning += " (index: " + index + " count: " + loc.Length + "(";
                    int coord = 0;
                    foreach (double[] point in loc)
                    {
                        warning += coord + " point: " + point[0] + ", " + point[1] + ", ";
                        coord++;
                    }
                    index++;
                }
                warning += ")";

                Utilities.LogUtils.Warn(warning);
                /*
                // find the largest ring
                // remove all the inner rings
                double[][] largestRing = null;
                List<double[][]> newlocs = new List<double[][]>();
                foreach (double[][] ring in result.Locs)
                {
                    if (largestRing == null)
                    {
                        largestRing = ring;
                    }
                    else
                    {
                        if (ring.Length > largestRing.Length)
                        {
                            largestRing = ring;
                        }
                    }
                }
                // insert the default ring;
                newlocs.Add(largestRing);

                foreach (double[][] ring in result.Locs)
                {
                    // check if it's the same ring
                    if (ring[0] != largestRing[0])
                    {
                        bool bDonut = GPS.DomainLayer.Area.ShapeMethods.PointInPolygon(largestRing, ring[0]);
                        if (!bDonut)
                        {
                            newlocs.Add(ring);
                        }
                    }
                }

                if (newlocs.Count == 1)
                {
                    result.Locs = newlocs;
                }

                */
            }
            
            //remove false records
            records = records.Where(i => i.Value == true).ToList(); 

            return UpdateMergeResult(campaignId, submapId, result, records);
        }

        [OperationContract]
        public void EmptySubmap(int campaignId, int submapId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    var submap = campaign.SubMaps.Where(t => t.Id == submapId).SingleOrDefault();
                    if (submap != null)
                    {
                        submap.SubMapCoordinates.Clear();
                        submap.SubMapRecords.Clear();
                        submap.Total = 0;
                        submap.Penetration = 0;
                        if ((submap.Total + submap.TotalAdjustment) > 0)
                        {
                            submap.Percentage = (double)(submap.Penetration + submap.CountAdjustment) / (double)(submap.Total + submap.TotalAdjustment);
                        }
                        else
                        {
                            submap.Percentage = 0;
                        }
                        ws.Repositories.CampaignRepository.Update(campaign);
                        ws.Commit();
                    }
                }
            }
        }

        private ToMergeResult UpdateMergeResult(int campaignId, int submapId, MergeResult result, IEnumerable<AreaRecord> records)
        {
            ToMergeResult ret = null;

            if (result.Locs.Count == 1)
            {
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                    if (campaign != null)
                    {
                        var submap = campaign.SubMaps.Where(t => t.Id == submapId).SingleOrDefault();
                        if (submap != null)
                        {
                            submap.SubMapRecords.Clear();
                            submap.Total = 0;
                            foreach (AreaRecord record in records)
                            {
                                submap.SubMapRecords.Add(new SubMapRecord()
                                {
                                    SubMap = submap,
                                    Classification = record.Classification,
                                    AreaId = record.AreaId,
                                    Value = record.Value
                                });

                            }
                            submap.Total = Convert.ToInt32(result.Total);
                            submap.Penetration = Convert.ToInt32(result.Count);
                            if ((submap.Total + submap.TotalAdjustment) > 0)
                            {
                                submap.Percentage = (double)(submap.Penetration + submap.CountAdjustment) / (double)(submap.Total + submap.TotalAdjustment);
                            }
                            else
                            {
                                submap.Percentage = 0;
                            }
                            ws.Repositories.CampaignRepository.Update(campaign);
                            ws.Commit();

                            using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
                            {
                                using (ITransaction tx = bws.BeginTransaction())
                                {
                                    try
                                    {
                                        bws.Repositories.BulkSubMapCoordinateRepository.DeleteBySubMap(submap.Id);
                                        foreach (double[] loc in result.Locs[0])
                                        {
                                            bws.Repositories.BulkSubMapCoordinateRepository.Add(new SubMapCoordinate()
                                            {
                                                SubMap = submap,
                                                Latitude = loc[0],
                                                Longitude = loc[1]
                                            });
                                        }
                                        tx.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        ILog logger = LogManager.GetLogger(GetType());
                                        logger.Error("WCF Unhandle Error", ex);
                                        tx.Rollback();
                                    }
                                }
                            }

                            ret = AssemblerConfig.GetAssembler<ToMergeResult, MergeResult>().AssembleFrom(result);
                        }
                    }
                }
            }
            return ret;
        }

        #endregion


        public int ValidatorChildren(Campaign campaign , bool isDM)
        {
            int ret = 0;
            if (!isDM)
            {
                if (campaign.SubMaps == null || campaign.SubMaps.Count == 0) ret=1;
            }
            else
            {
                ret=2;
                foreach (SubMap sm in campaign.SubMaps)
                {
                    if (sm.DistributionMaps != null && sm.DistributionMaps.Count > 0) ret=0;
                }
            }
            return ret;
        
        }

    }


    public class MyException : Exception
    {
        public MyException(string message)
            : base(message)
        { }
        public MyException(string message, Exception ex)
            : base(message, ex)
        { }

    }
}
