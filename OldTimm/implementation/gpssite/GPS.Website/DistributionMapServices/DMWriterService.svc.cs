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
using GPS.DomainLayer.Area;
using GPS.DomainLayer.Area.AreaMerge;
using GPS.DataLayer.DataInfrastructure;
using log4net;
using NetTopologySuite.Geometries;


namespace GPS.Website.DistributionMapServices
{
    [ServiceContract(Namespace = "TIMM.Website.DistributionMapServices")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DMWriterService
    {
        [OperationContract]
        public void SaveDMs(IEnumerable<ToSubMap> tosubmaps)
        {
            IEnumerable<GPS.DomainLayer.Entities.SubMap> subMaps = new List<GPS.DomainLayer.Entities.SubMap>();
            subMaps = AssemblerConfig.GetAssembler<SubMap, ToSubMap>().AssembleFrom(tosubmaps);

            var campRepository = new CampaignRepository();
            foreach (GPS.DomainLayer.Entities.SubMap sm in subMaps)
            {
                Campaign c = campRepository.GetEntity(sm.CampaignId);
                SubMap s = c.GetSubMap(sm.Id);
                c.RemoveDistributionMapsOfSubMap(s.Id);
                //GPS.DomainLayer.Entities.DistributionMap dm = new GPS.DomainLayer.Entities.DistributionMap();
                foreach (GPS.DomainLayer.Entities.DistributionMap dm in sm.DistributionMaps)
                {
                    c.AddDistributionMapToSubMap(s, dm);
                }

                campRepository.Update(c);
            }
        }


        [OperationContract]
        public ToDistributionMap SaveDistributionMap(int campaignid, ToDistributionMap todistributionmap)
        {
            ToDistributionMap ret = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Campaign c = ws.Repositories.CampaignRepository.GetEntity(campaignid);
                        GPS.DomainLayer.Entities.DistributionMap dm = AssemblerConfig.GetAssembler<GPS.DomainLayer.Entities.DistributionMap, ToDistributionMap>().AssembleFrom(todistributionmap);
                        dm.ColorB = todistributionmap.ColorB;
                        dm.ColorG = todistributionmap.ColorG;
                        dm.ColorR = todistributionmap.ColorR;
                        dm.ColorString = todistributionmap.ColorString;
                        dm.CountAdjustment = todistributionmap.CountAdjustment;
                        dm.TotalAdjustment = todistributionmap.TotalAdjustment;
                        dm.Penetration = todistributionmap.Penetration;
                        dm.Percentage = todistributionmap.Percentage;
                        ws.Repositories.DistributionMapRepository.AddDistributionMap(dm);
                        c.AddDistributionMapToSubMap(c.GetSubMap(todistributionmap.SubMapId), dm);
                        ws.Repositories.CampaignRepository.Update(c);
                        tx.Commit();
                        todistributionmap.Id = dm.Id;
                        ret = todistributionmap;
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = LogManager.GetLogger(GetType());
                        logger.Error("WCF Unhandle Error", ex);
                    }
                }
            }

            return ret;
        }

        [OperationContract]
        public List<NotIncludeArea> DeleteDistributionMap(int campaignid, ToDistributionMap todistributionmap)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Campaign c = ws.Repositories.CampaignRepository.GetEntity(campaignid);
                        if (c.ContainsSubMap(todistributionmap.SubMapId))
                        {
                            c.RemoveDistributionMapFromSubMap(todistributionmap.SubMapId, c.GetDistributionMap(todistributionmap.SubMapId, todistributionmap.Id));
                            ws.Repositories.CampaignRepository.Update(c);
                            tx.Commit();
                        }
                    }
                    catch (Exception ex) { tx.Rollback();
                    ILog logger = log4net.LogManager.GetLogger(typeof(DMWriterService));
                    logger.Error("WCF Unhandl Error", ex);
                    }
                }
            }
            return CalcDmapNoIncludeArea(campaignid);
        }

        [OperationContract]
        public void UpdateDistributionMap(int campaignid, ToDistributionMap todistributionmap)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Campaign c = ws.Repositories.CampaignRepository.GetEntity(campaignid);
                        if (c.ContainsSubMap(todistributionmap.SubMapId))
                        {
                            GPS.DomainLayer.Entities.DistributionMap dm = c.GetDistributionMap(todistributionmap.SubMapId, todistributionmap.Id);
                            dm.Name = todistributionmap.Name;
                            dm.ColorB = todistributionmap.ColorB;
                            dm.ColorG = todistributionmap.ColorG;
                            dm.ColorR = todistributionmap.ColorR;
                            dm.ColorString = todistributionmap.ColorString;
                            dm.CountAdjustment = todistributionmap.CountAdjustment;
                            dm.TotalAdjustment = todistributionmap.TotalAdjustment;
                            dm.Penetration = todistributionmap.Penetration;
                            dm.Percentage = todistributionmap.Percentage;
                            ws.Repositories.CampaignRepository.Update(c);
                            tx.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback(); ILog logger = log4net.LogManager.GetLogger(typeof(DMWriterService));
                        logger.Error("WCF Unhandl Error", ex);
                    }
                }
            }
        }

        [OperationContract]
        public ToMergeResult MergeAreas(int campaignId, int submapId, int dmapId, List<ToAreaRecord> toRecords)
        {
            IEnumerable<AreaRecord> records = AssemblerConfig.GetAssembler<AreaRecord, ToAreaRecord>().AssembleFrom(toRecords);
            MergeOperator oper = new MergeOperator();
            MergeResult result = oper.MergeAreasAPI(campaignId, null, ref records);

            if (result.Locs.Count >= 2)
            {
                string warning = "CampaignId = " + campaignId + " subMapId = " + submapId;
                int index = 0;
                foreach (double[][] loc in result.Locs)
                {
                    warning += " (index: " + index + " count: " + loc.Length + " 1st point: " + loc[0][0] + ", " + loc[0][1] + ")";
                    index++;
                }

                Utilities.LogUtils.Warn(warning);
            }
            var ret = UpdateMergeResult(campaignId, submapId, dmapId, result, records);
            ret.NotIncludeInSubMapArea = CalcDmapNoIncludeArea(campaignId);
            return ret;
        }

        [OperationContract]
        public ToMergeResult MergeSelectedAreas(int campaignId, int submapId, int dmapId, List<ToAreaRecord> toRecords, List<ToAreaRecord> newAddedRecords)
        {
            //remove duplicate record in newAddedRecords
            var toRecordsList = toRecords.Where(i => i.Value == true);
            var toRecordsHash = toRecordsList.ToDictionary(i => i.AreaId);

            newAddedRecords = newAddedRecords.Where(i => !toRecordsHash.ContainsKey(i.AreaId)).ToList();
            if (newAddedRecords.Count == 0)
            {
                return MergeAreas(campaignId, submapId, dmapId, toRecords);
            }
            IEnumerable<AreaRecord> records = AssemblerConfig.GetAssembler<AreaRecord, ToAreaRecord>().AssembleFrom(toRecordsList);
            IEnumerable<AreaRecord> newRecords = AssemblerConfig.GetAssembler<AreaRecord, ToAreaRecord>().AssembleFrom(newAddedRecords);
            MergeOperator oper = new MergeOperator();
            var filterRecorder = oper.FilterNotIncludedInSubMapRecord(campaignId, submapId, newRecords);
            var mergedRecorder = new List<AreaRecord>();
            mergedRecorder.AddRange(records);
            mergedRecorder.AddRange(filterRecorder);
            var refMergedRecorder = mergedRecorder as IEnumerable<AreaRecord>;
            MergeResult result = oper.MergeAreasAPI(campaignId, null, ref refMergedRecorder);

            if (result.Locs.Count >= 2)
            {
                string warning = "CampaignId = " + campaignId + " subMapId = " + submapId;
                int index = 0;
                foreach (double[][] loc in result.Locs)
                {
                    warning += " (index: " + index + " count: " + loc.Length + " 1st point: " + loc[0][0] + ", " + loc[0][1] + ")";
                    index++;
                }

                Utilities.LogUtils.Warn(warning);
            }
            var mergeResult = UpdateMergeResult(campaignId, submapId, dmapId, result, mergedRecorder);
            mergeResult.ValidAreas = filterRecorder.Select(i=>new ToAreaRecord { 
                AreaId = i.AreaId,
                Classification = (int)i.Classification,
                Value = i.Value,
                Relations = i.Relations,
            }).ToList();
            mergeResult.NotIncludeInSubMapArea = CalcDmapNoIncludeArea(campaignId);
            return mergeResult;
        }

        [OperationContract]
        public List<NotIncludeArea> EmptyDM(int campaignId, int submapId, int dmapId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    var submap = campaign.SubMaps.Where(t => t.Id == submapId).SingleOrDefault();
                    var dmap = submap.DistributionMaps.Where(t => t.Id == dmapId).SingleOrDefault();
                    if (dmap != null)
                    {

                        dmap.DistributionMapCoordinates.Clear();
                        dmap.DistributionMapRecords.Clear();

                        dmap.Total = 0;
                        dmap.Penetration = 0;
                        if ((dmap.Total + dmap.TotalAdjustment) > 0)
                        {
                            dmap.Percentage = (double)(dmap.Penetration + dmap.CountAdjustment) / (double)(dmap.Total + dmap.TotalAdjustment);
                        }
                        else
                        {
                            dmap.Percentage = 0;
                        }

                        ws.Repositories.DistributionMapRepository.UpdateDistributionMap(dmap);
                        ws.Commit();
                    }
                }
            }

            return CalcDmapNoIncludeArea(campaignId);
        }

        private ToMergeResult UpdateMergeResult(int campaignId, int submapId, int dmapId, MergeResult result, IEnumerable<AreaRecord> records)
        {
            ToMergeResult ret = null;
            if (result.Locs.Count == 1)
            {
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    using (ITransaction it = ws.BeginTransaction())
                    { 
                        try
                        {
                            Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                            if (campaign != null)
                            {
                                var submap = campaign.SubMaps.Where(t => t.Id == submapId).SingleOrDefault();
                                var dmap = submap.DistributionMaps.Where(t => t.Id == dmapId).SingleOrDefault();

                                #region dmap is not null
                                if (dmap != null)
                                {
                                    //List<Int32> submapBgList = new List<Int32>();
                                    List<Int32> submapAreaList = new List<Int32>();
                                    //foreach (SubMapRecord sr in submap.SubMapRecords)
                                    //{
                                    //    if(sr.Value)
                                    //    submapBgList.AddRange(SeperateToBG(ws, sr.AreaId, sr.Classification.ToString()));
                                    //}

                                    foreach (SubMapRecord sr in submap.SubMapRecords)
                                    {
                                        if(sr.Value)
                                        submapAreaList.Add(sr.AreaId);
                                    }



                                    List<DistributionMapRecords> recordsList = new List<DistributionMapRecords>();  
                                    bool isBelongSubMap = true;
                                    foreach (AreaRecord record in records)
                                    {
                                        if (record.Value)
                                        {
                                            //if (record.Classification == GPS.DomainLayer.Enum.Classifications.PremiumCRoute)
                                            //{
                                            if (record.Classification != submap.SubMapRecords[0].Classification)
                                            {
                                                isBelongSubMap = false;
                                            }

                                            if (submapAreaList.FindIndex(item => (item == record.AreaId)) == -1)
                                            {
                                                isBelongSubMap = false;
                                            }
                                            //}
                                            //else
                                            //{

                                            //    List<Int32> bgList = SeperateToBG(ws, record.AreaId, record.Classification.ToString());
                                            //    foreach (int bgId in bgList)
                                            //    {
                                            //        if (submapBgList.FindIndex(item => (item == bgId)) == -1)
                                            //        {
                                            //            isBelongSubMap = false;
                                            //        }
                                            //    }

                                            //}
                                        }


                                        if (!isBelongSubMap)
                                            return new ToMergeResult();



                                        recordsList.Add(new DistributionMapRecords()
                                        {
                                            DistributionMapId = dmapId,
                                            Classification = record.Classification,
                                            AreaId = record.AreaId,
                                            Value = record.Value
                                        });

                                    }

                                    dmap.DistributionMapRecords.Clear();
                                    ws.Repositories.DistributionMapRepository.UpdateDistributionMap(dmap);

                                    dmap.Total = Convert.ToInt32(result.Total);
                                    dmap.Penetration = Convert.ToInt32(result.Count);
                                    if ((dmap.Total + dmap.TotalAdjustment) > 0)
                                    {
                                        dmap.Percentage = (double)(dmap.Penetration + dmap.CountAdjustment) / (double)(dmap.Total + dmap.TotalAdjustment);
                                    }
                                    else
                                    {
                                        dmap.Percentage = 0;
                                    }

                                    foreach (DistributionMapRecords r in recordsList)
                                    {
                                        dmap.DistributionMapRecords.Add(r);
                                    }

                                    ws.Repositories.DistributionMapRepository.UpdateDistributionMap(dmap);                                   

                                }
                                #endregion
                                
                                it.Commit();
                            }
                        }
                        catch(Exception ex)
                        {
                            it.Rollback();
                            ILog logger = LogManager.GetLogger(GetType());
                            logger.Error("WCF Unhandle Error", ex);
                        }
                    }
                }


                using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
                {
                    using (ITransaction tx = bws.BeginTransaction())
                    {
                        try
                        {
                            bws.Repositories.BulkDistributionMapCoordinateRepository.DeleteByDistributionMap(dmapId);
                            foreach (double[] loc in result.Locs[0])
                            {
                                bws.Repositories.BulkDistributionMapCoordinateRepository.Add(
                                new DistributionMapCoordinate()
                                {
                                    DistributionMapId = dmapId,
                                    Latitude = loc[0],
                                    Longitude = loc[1]
                                }
                                );
                            }
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
            ret = AssemblerConfig.GetAssembler<ToMergeResult, MergeResult>().AssembleFrom(result);
            return ret;
        }

        public static List<NotIncludeArea> CalcDmapNoIncludeArea(int campaignId)
        {
            List<NotIncludeArea> result = new List<NotIncludeArea>();
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                var campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                foreach (var submap in campaign.SubMaps)
                {
                    var submapRecord = submap.SubMapRecords.ToDictionary(i => i.AreaId);
                    if(submapRecord.Count == 0){
                        result.Add(new NotIncludeArea { 
                            Id = submap.Id,
                            Name = submap.Name,
                            Count = 0
                        });
                        continue;
                    }
                    foreach (var dmap in submap.DistributionMaps)
                    {
                        var dmapRecord = dmap.DistributionMapRecords.Where(i => i.Value == true).ToArray();
                        foreach (var exist in dmapRecord)
                        {
                            if (submapRecord.ContainsKey(exist.AreaId))
                            {
                                submapRecord.Remove(exist.AreaId);
                            }
                        }
                    }
                    var count = submapRecord.Values.Select(i => i.AreaId).Count();
                    var submapNotIncludedArea = new NotIncludeArea
                    {
                        Id = submap.Id,
                        Name = submap.Name,
                        Count = count,
                    };

                    var allArea = submapRecord.Values.Select(i => i.AreaId).ToArray();
                    if (allArea.Length > 0)
                    {
                        submapNotIncludedArea.Areas = GetBundaryForAreas(submap.SubMapRecords[0].Classification, allArea);
                    }
                    result.Add(submapNotIncludedArea);
                }
            }
            return result;
        }
        private static List<LatLngLocation[]> GetBundaryForAreas(GPS.DomainLayer.Enum.Classifications classification, int[] areas)
        {
            var result = new List<LatLngLocation[]>();

            
            if (classification == DomainLayer.Enum.Classifications.PremiumCRoute)
            {
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    List<Polygon> allPolygon = new List<Polygon>();
                    foreach(var id in areas){
                        var area = ws.Repositories.PremiumCRouteRepository.GetItem(id);
                        var areaLatLng = area.PremiumCRouteCoordinates.OrderBy(i => i.Id).Select(i=>new GeoAPI.Geometries.Coordinate(i.Longitude, i.Latitude)).ToList();
                        areaLatLng.Add(new GeoAPI.Geometries.Coordinate(areaLatLng[0].X, areaLatLng[0].Y));
                        var areaRing = new LinearRing(areaLatLng.ToArray());
                        var polygon = new Polygon(areaRing);
                        polygon = polygon.Buffer(0) as Polygon;
                        allPolygon.Add(polygon);                        
                    }
                    MultiPolygon allArea = new MultiPolygon(allPolygon.ToArray());
                    var union = allArea.Union();
                    var count = union.NumGeometries;
                    for (int i = 0; i < count; i++)
                    {
                        var polygon = union.GetGeometryN(i);
                        result.Add(polygon.Boundary.Coordinates.Select(l => new LatLngLocation
                        {
                            Lat = l.Y,
                            Lng = l.X
                        }).ToArray());
                    }
                }
            }
            return result;
        }

        private List<Int32> SeperateToBG(IWorkSpace ws, int areaId , string classfication)
        {
            List<Int32> bgList = new List<Int32>();
            IEnumerable<BlockGroup> bgObjects = null;
            switch (classfication)
            {
                case "Z5":
                    //bgList.AddRange(ws.Repositories.FiveZipRepository.GetBGByFiveZip(areaId));
                    bgObjects = ws.Repositories.FiveZipRepository.GetBGByFiveZip(areaId);
                    foreach (var bg in bgObjects)
                    {                        
                        bgList.Add(bg.Id);
                    }
                    break;
                case "TRK":
                    bgObjects = ws.Repositories.FiveZipRepository.GetBGByTract(areaId);
                    foreach (var bg in bgObjects)
                    {
                        bgList.Add(bg.Id);
                    }
                    break;
                //case "PremiumCRoute":
                //    bgObjects = ws.Repositories.FiveZipRepository.GetBGByPremiumCRoute(areaId);
                //    foreach (var bg in bgObjects)
                //    {
                //        bgList.Add(bg.Id);
                //    }
                //    break;
                case "BG":
                    bgList.Add(areaId);
                    break;
            }
            return bgList;

        }


    }
}


