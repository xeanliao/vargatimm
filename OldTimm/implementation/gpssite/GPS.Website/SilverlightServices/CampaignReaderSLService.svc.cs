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
using GPS.DataLayer.RepositoryImplementations;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.Area.AreaMerge;
using GPS.DataLayer.DataInfrastructure;
using GPS.Utilities.Boundary;
using CommonUtils;
using GPS.DomainLayer.Area.AreaOperators;
using GPS.DomainLayer.Area.MapPointService;
using GPS.DomainLayer.Area.Import;
using log4net;

namespace GPS.Website.SilverlightServices
{
    [ServiceContract(Namespace = "TIMM.Website.SilverlightServices")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    // NOTE: If you change the class name "CampaignReaderSLService" here, you must also update the reference to "CampaignReaderSLService" in Web.config.
    public class CampaignReaderSLService
    {
        private static ILog m_Loger = LogManager.GetLogger(typeof(CampaignReaderSLService));

        [OperationContract]
        public byte[] GetCampaignById(int id)
        {
            ToCampaign toCampaign = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(id);
                if (campaign == null) return null;
                toCampaign = AssemblerConfig.GetAssembler<ToCampaign, Campaign>().AssembleFrom(campaign);
            }

            return CompressedSerializer.Compress<ToCampaign>(toCampaign);
        }

        [OperationContract]
        public ToCampaign GetCampaignObjectById(int id)
        {
            ToCampaign toCampaign = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(id);
                if (campaign == null) return null;
                toCampaign = AssemblerConfig.GetAssembler<ToCampaign, Campaign>().AssembleFrom(campaign);
            }

            return toCampaign;
        }

        //[OperationContract]
        //public IEnumerable<ToSubMap> GetSubMaps(int campaignId)
        //{
        //    CampaignRepository camRep = new CampaignRepository();
        //    Campaign c = camRep.GetEntity(campaignId);
        //    return AssemblerConfig.GetAssembler<ToSubMap, SubMap>().AssembleFrom(c.SubMaps);

        //}

        [OperationContract]
        public Dictionary<string, List<ToCoordinate>> GetCroutesByBoxes(List<int> boxIds)
        {
            Classifications classification = Classifications.PremiumCRoute;
            Dictionary<string, List<ToCoordinate>> crouteShapDct = new Dictionary<string, List<ToCoordinate>>();
            try
            {
                foreach (int boxId in boxIds)
                {
                    IEnumerable<IArea> areas = MapAreaManager.GetAreas(classification, boxId);
                    if (areas.Count() > 0)
                        foreach (IArea area in areas)
                        {
                            if (!crouteShapDct.ContainsKey(area.Id.ToString()))
                            {
                                List<ToCoordinate> areaLst = new List<ToCoordinate>();
                                foreach (ICoordinate cd in area.Locations)
                                {
                                    ToCoordinate toCd = new ToCoordinate();
                                    toCd.Latitude = cd.Latitude;
                                    toCd.Longitude = cd.Longitude;
                                    areaLst.Add(toCd);
                                }
                                crouteShapDct.Add(area.Id.ToString(), areaLst);
                            }

                        }
                }
                return crouteShapDct;
            }
            catch (Exception ex)
            {
                m_Loger.Error("WCF Unhandler Error", ex);
                return null;
            }
        }

        [OperationContract]
        public byte[] GetAreaByBoxes(List<int> boxIds, int campaignId, Classifications classification,ToArea toArea)
        {
            List<ToArea> areas = new List<ToArea>();
            BlockGroupRepository bgRep = new BlockGroupRepository();
            PremiumCRouteRepository crRep = new PremiumCRouteRepository();
            CampaignRepository camRep = new CampaignRepository();

            Campaign campaign = camRep.GetEntity(campaignId);

            if (classification == Classifications.BG)
            {
                List<BlockGroup> BGAreas = bgRep.GetBoxItems(boxIds).ToList();

                foreach (BlockGroup BGArea in BGAreas)
                {
                    MapArea marea = BGConvertToArea(BGArea);
                    ToArea toarea = AssemblerConfig.GetAssembler<ToArea, MapArea>().AssembleFrom(marea);
                    areas.Add(toarea);
                }

            }
            else
            {
                IList<PremiumCRoute> premiumCRoutes = crRep.GetBoxItems(boxIds).ToList();
                foreach (PremiumCRoute cr in premiumCRoutes)
                {
                    MapArea area = CrouteConvertToArea(cr);

                    var data = campaign.CampaignCRouteImporteds.Where(t => t.PremiumCRoute.Id == cr.Id).SingleOrDefault();
                    if (data != null)
                    {
                        var per = data.Total > 0 ? (double)data.Penetration / (double)data.Total : 0;
                        area.Attributes.Add("SourceTotal", data.Total.ToString());
                        area.Attributes.Add("SourceCount", data.Penetration.ToString());
                        area.Attributes.Add("Penetration", per.ToString());
                        if (data.IsPartModified)
                        {
                            area.Attributes.Add("Total", Math.Round((data.Total * data.PartPercentage)).ToString());
                            area.Attributes.Add("Count", Math.Round((data.Penetration * data.PartPercentage)).ToString());
                            area.Attributes.Add("PartPercentage", data.PartPercentage.ToString());
                        }
                        else
                        {
                            area.Attributes.Add("Total", data.Total.ToString());
                            area.Attributes.Add("Count", data.Penetration.ToString());
                            area.Attributes.Add("PartPercentage", "1");
                        }
                        area.Attributes.Add("IsPartModified", data.IsPartModified.ToString());
                    }

                    ToArea toarea = AssemblerConfig.GetAssembler<ToArea, MapArea>().AssembleFrom(area);

                    areas.Add(toarea);
                }
            }

            return CompressedSerializer.Compress<List<ToArea>>(areas);
        }


        [OperationContract]
        public byte[] GetCampaignCRouteAreas(int campaignId, String geoCode)
        {
            PremiumCRouteOperator oper = new PremiumCRouteOperator();
            IEnumerable<MapArea> areas = oper.Gets(campaignId, geoCode);
            Assembler<ToArea, MapArea> asm = AssemblerConfig.GetAssembler<ToArea, MapArea>();
            IEnumerable<ToArea> numerable =  asm.AssembleFrom(areas);
            return CompressedSerializer.Compress<List<ToArea>>(numerable.ToList<ToArea>());

        }


        [OperationContract]
        public ToSubMap GetSubMapsById(int id)
        {
            try
            {
                SubMapRepository dr = new SubMapRepository();
                GPS.DomainLayer.Entities.SubMap dmap = dr.GetEntity(id);

                return AssemblerConfig.GetAssembler<ToSubMap, GPS.DomainLayer.Entities.SubMap>().AssembleFrom(dmap);
            }
            catch(Exception ex)
            {
                m_Loger.Error("WCF Unhandler Error", ex);
                return null;
            }
        }


        [OperationContract]
        public void AdjustData(int campaignId, int classification, List<ToAdjustData>datas)
        {
            Importer importer = new Importer();
            foreach (ToAdjustData data in datas)
            {
                importer.AdjustCount(campaignId, classification, data.Id, data.Total, data.Count, data.PartPercentage, data.IsPartModified,false);
            }
        }


        [OperationContract]
        public void SaveCampaign(ToCampaign toCampaign)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        DataLayer.CampaignRepository campaignRep = new DataLayer.CampaignRepository();
                        DomainLayer.Entities.Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(toCampaign.Id);
                        if (campaign != null)
                        {
                            campaign.Latitude = toCampaign.Latitude;
                            campaign.Longitude = toCampaign.Longitude;
                            campaign.ZoomLevel = toCampaign.ZoomLevel;

                            

                            //campaign.CampaignCRouteImporteds.Clear();


                            campaign.CampaignRecords.Clear();
                            foreach (ToAreaRecord record in toCampaign.CampaignRecords)
                            {
                                campaign.CampaignRecords.Add(new CampaignRecord()
                                {
                        
                                    Campaign = campaign,
                                    Classification = record.Classification,
                                    AreaId = record.AreaId,
                                    Value = record.Value
                                });
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

                            campaign.SubMaps.Clear();                             

                            foreach (ToSubMap toSubmap in toCampaign.SubMaps)
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

                                foreach (ToAreaRecord toArea in toSubmap.SubMapRecords)
                                {
                                    SubMapRecord area = new SubMapRecord();
                                    area.SubMap = sm;
                                    area.AreaId = toArea.AreaId;
                                    area.Classification = (Classifications)toArea.Classification;
                                    area.Value = toArea.Value;
                                    area.Relation = new List<List<string>>();
                                    sm.SubMapRecords.Add(area);
                                }

                                foreach (ToCoordinate toCoordinate in toSubmap.SubMapCoordinates)
                                {
                                    SubMapCoordinate coordinate = new SubMapCoordinate();
                                    coordinate.SubMap = sm;
                                    coordinate.Latitude = toCoordinate.Latitude;
                                    coordinate.Longitude = toCoordinate.Longitude;
                                    sm.SubMapCoordinates.Add(coordinate);
                                }

                                campaign.SubMaps.Add(sm);
                            }

                        }

                        ws.Repositories.CampaignRepository.UpdateCopy(campaign);
                        tx.Commit();
                    } 
                    catch(Exception ex)
                    {
                        tx.Rollback();
                        m_Loger.Error("WCF Unhandler Error", ex);
                    }

                }
            }
        }

        private MapArea CrouteConvertToArea(PremiumCRoute item)
        {
            MapArea area = new MapArea()
            {
                Classification = Classifications.PremiumCRoute,
                Id = item.Id,
                Name = item.Code,
                IsEnabled = item.IsEnabled,
                Description = item.Description,
                State = item.STATE_FIPS,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Relations = new Dictionary<int, Dictionary<int, bool>>()
            };

            var coordinates = item.PremiumCRouteCoordinates.OrderBy(t => t.Id);
            foreach (var coordinate in coordinates)
            {
                area.Locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude, coordinate.ShapeId));
            }

            area.Attributes.Add("OTotal", item.OTotal.ToString());
            area.Attributes.Add("PartCount", item.PartCount.ToString());
            area.Attributes.Add("Zip", item.ZIP);
            return area;
        }

        private MapArea BGConvertToArea(BlockGroup item)
        {
            MapArea area = new MapArea()
            {
                Classification = Classifications.BG,
                Id = item.Id,
                Name = item.Name,
                IsEnabled = item.IsEnabled,
                Description = item.Description,
                State = item.StateCode,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Relations = new Dictionary<int, Dictionary<int, bool>>()
            };

            var coordinates = item.BlockGroupCoordinates.OrderBy(t => t.Id);
            foreach (var coordinate in coordinates)
            {
                area.Locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude, coordinate.ShapeId));
            }

            area.Attributes.Add("OTotal", item.OTotal.ToString());
            area.Attributes.Add("State", item.StateCode);
            area.Attributes.Add("County", item.CountyCode);
            area.Attributes.Add("Tract", item.TractCode);
            area.Attributes.Add("BlockGroup", item.Code);
            return area;
        }

    }


}