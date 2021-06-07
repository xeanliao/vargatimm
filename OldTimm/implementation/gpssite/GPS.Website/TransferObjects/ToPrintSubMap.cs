using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.QuerySpecifications;
using GPS.DataLayer;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.DistributionMapServices")]
    public class ToPrintSubMap
    {
        [DataMember]
        public int CampaignId
        {
            get;
            set;
        }
        [DataMember]
        public int ColorB
        {
            get;
            set;
        }
        [DataMember]
        public int ColorG
        {
            get;
            set;
        }

        [DataMember]
        public int ColorR
        {
            get;
            set;
        }
        [DataMember]
        public String ColorString
        {
            get;
            set;
        }
        [DataMember]
        public int Id
        {
            get;
            set;
        }
        [DataMember]
        public String Name
        {
            get;
            set;
        }

        [DataMember]
        public int OrderId
        {
            get;
            set;
        }
        [DataMember]
        public int Penetration
        {
            get;
            set;
        }

        [DataMember]
        public double Percentage
        {
            get;
            set;
        }

        [DataMember]
        public int Total
        {
            get;
            set;
        }

        [DataMember]
        public ToPrintArea[] FiveZipAreas
        {
            get;
            set;
        }

        [DataMember]
        public ToPrintArea[] CRoutes
        {
            get;
            set;
        }

        [DataMember]
        public ToPrintArea[] Tracts
        {
            get;
            set;
        }

        [DataMember]
        public ToPrintArea[] BlockGroups
        {
            get;
            set;
        }

        [DataMember]
        public ToCoordinate[] SubMapCoordinates
        {
            get;
            set;
        }

        [DataMember]
        public ToPrintDistributionMap[] DistributionMaps
        {
            get;
            set;
        }

        /// <summary>
        /// Only used by the Otis transformer to transform the Relations property.
        /// </summary>
        /// <param name="target">A <see cref="ToAddressRadius"/>, the DTO.</param>
        /// <param name="source">A <see cref="MapAddressRadius"/>, the source.</param>
        public static void Convert(ref ToPrintSubMap target, ref SubMap source)
        {
            target.Id = source.OrderId;
            target.Total = source.Total + source.TotalAdjustment;
            target.Penetration = source.Penetration + source.CountAdjustment;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(source.CampaignId);
                if (campaign != null && source.SubMapRecords.Count > 0)
                {
                    if (source.SubMapRecords[0].Classification == Classifications.Z5)
                    {
                        List<Int32> fiveZipIds = GetRecordIds(source.SubMapRecords, Classifications.Z5, true);
                        //List<Int32> nonFiveZipIds = new List<int>();
                        List<Int32> cRouteIds = GetRecordIds(source.SubMapRecords, Classifications.PremiumCRoute, true);
                        List<Int32> nonCRouteIds = GetRecordIds(source.SubMapRecords, Classifications.PremiumCRoute, false);

                        FiveZipsSatisfyingSubMapSpecification fSpec = new FiveZipsSatisfyingSubMapSpecification(fiveZipIds, cRouteIds, nonCRouteIds);
                        IEnumerable<FiveZipArea> fiveZips = fSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.FiveZipRepository);
                        List<ToPrintArea> fAreas = new List<ToPrintArea>();
                        foreach (FiveZipArea fiveZip in fiveZips)
                        {
                            var areaData = campaign.CampaignFiveZipImporteds.Where(t => t.FiveZipArea.Id == fiveZip.Id).SingleOrDefault();

                            ToPrintArea area = new ToPrintArea()
                            {
                                Id = fiveZip.Id,
                                Code = fiveZip.Code,
                                IsEnabled = fiveZip.IsEnabled,
                            };

                            if (areaData != null)
                            {
                                if (areaData.PartPercentage == 0) areaData.PartPercentage = 1;
                                area.Total = (int)Math.Round(areaData.Total * areaData.PartPercentage);
                                area.Count = (int)Math.Round(areaData.Penetration * areaData.PartPercentage);
                                area.PartPercentage = (int)Math.Round(areaData.PartPercentage * 100);
                            }
                            else
                            {
                                area.Total = fiveZip.HOME_COUNT + fiveZip.APT_COUNT;
                            }

                            int size = fiveZip.FiveZipAreaCoordinates.Count;
                            ToCoordinate[] ps = new ToCoordinate[size];
                            for (int i = 0; i < size; i++)
                            {
                                ps[i] = new ToCoordinate()
                                {
                                    Latitude = fiveZip.FiveZipAreaCoordinates[i].Latitude,
                                    Longitude = fiveZip.FiveZipAreaCoordinates[i].Longitude
                                };
                            }
                            area.Coordinates = ps;
                            fAreas.Add(area);
                            //nonFiveZipIds.Add(fiveZip.Id);

                        }
                        target.FiveZipAreas = fAreas.ToArray();

                        List<ToPrintArea> cRAreas = new List<ToPrintArea>();
                        target.CRoutes = cRAreas.ToArray();
                        //target.CRoutes = new ToPrintArea[0];
                        target.Tracts = new ToPrintArea[0];
                        target.BlockGroups = new ToPrintArea[0];
                    }
                    else if (source.SubMapRecords[0].Classification == Classifications.PremiumCRoute)
                    {
                        List<Int32> fiveZipIds = GetRecordIds(source.SubMapRecords, Classifications.Z5, true);
                        List<Int32> nonFiveZipIds = new List<int>();
                        List<Int32> cRouteIds = GetRecordIds(source.SubMapRecords, Classifications.PremiumCRoute, true);
                        List<Int32> nonCRouteIds = GetRecordIds(source.SubMapRecords, Classifications.PremiumCRoute, false);

                        //FiveZipsSatisfyingSubMapSpecification fSpec = new FiveZipsSatisfyingSubMapSpecification(fiveZipIds, cRouteIds, nonCRouteIds);
                        //IEnumerable<FiveZipArea> fiveZips = fSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.FiveZipRepository);
                        List<ToPrintArea> fAreas = new List<ToPrintArea>();
                        //foreach (FiveZipArea fiveZip in fiveZips)
                        //{
                        //    var areaData = campaign.CampaignFiveZipImporteds.Where(t => t.FiveZipArea.Id == fiveZip.Id).SingleOrDefault();
                        //    if (areaData != null)
                        //    {
                        //        ToPrintArea area = new ToPrintArea()
                        //        {
                        //            Id = fiveZip.Id,
                        //            Code = fiveZip.Code,
                        //            IsEnabled = fiveZip.IsEnabled,
                        //            Total = areaData.Total,
                        //            Count = areaData.Penetration
                        //        };

                        //        int size = fiveZip.FiveZipAreaCoordinates.Count;
                        //        ToCoordinate[] ps = new ToCoordinate[size];
                        //        for (int i = 0; i < size; i++)
                        //        {
                        //            ps[i] = new ToCoordinate()
                        //            {
                        //                Latitude = fiveZip.FiveZipAreaCoordinates[i].Latitude,
                        //                Longitude = fiveZip.FiveZipAreaCoordinates[i].Longitude
                        //            };
                        //        }
                        //        area.Coordinates = ps;
                        //        fAreas.Add(area);
                        //        nonFiveZipIds.Add(fiveZip.Id);
                        //    }
                        //}
                        target.FiveZipAreas = fAreas.ToArray();

                        List<ToPrintArea> cRAreas = new List<ToPrintArea>();
                        PremiumCRoutesSatisfyingSubMapSpecification cSpec = new PremiumCRoutesSatisfyingSubMapSpecification(
                            fiveZipIds, nonFiveZipIds, cRouteIds, nonCRouteIds);
                        IEnumerable<PremiumCRoute> cRoutes = cSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.PremiumCRouteRepository);
                        foreach (PremiumCRoute cRoute in cRoutes)
                        {

                            ToPrintArea area = new ToPrintArea()
                            {
                                Id = cRoute.Id,
                                Code = cRoute.Code,
                                IsEnabled = cRoute.IsEnabled
                            };
                            var areaData = campaign.CampaignCRouteImporteds.Where(t => t.PremiumCRoute.Id == cRoute.Id).SingleOrDefault();
                            if (areaData != null)
                            {
                                if (areaData.PartPercentage == 0) areaData.PartPercentage = 1;
                                area.Total = (int)Math.Round(areaData.Total * areaData.PartPercentage);
                                area.Count = (int)Math.Round(areaData.Penetration * areaData.PartPercentage);
                                area.PartPercentage = (int)Math.Round(areaData.PartPercentage * 100);
                            }
                            else
                            {
                                area.Total = cRoute.HOME_COUNT + cRoute.APT_COUNT;
                            }

                            int size = cRoute.PremiumCRouteCoordinates.Count;
                            ToCoordinate[] ps = new ToCoordinate[size];
                            for (int i = 0; i < size; i++)
                            {
                                ps[i] = new ToCoordinate()
                                {
                                    Latitude = cRoute.PremiumCRouteCoordinates[i].Latitude,
                                    Longitude = cRoute.PremiumCRouteCoordinates[i].Longitude
                                };
                            }
                            area.Coordinates = ps;
                            cRAreas.Add(area);
                        }
                        target.CRoutes = cRAreas.ToArray();
                        //target.CRoutes = new ToPrintArea[0];
                        target.Tracts = new ToPrintArea[0];
                        target.BlockGroups = new ToPrintArea[0];
                    }
                    else if (source.SubMapRecords[0].Classification == Classifications.TRK)
                    {
                        List<Int32> tractIds = GetRecordIds(source.SubMapRecords, Classifications.TRK, true);
                        //List<Int32> nonTractIds = new List<int>();
                        List<Int32> bgIds = GetRecordIds(source.SubMapRecords, Classifications.BG, true);
                        List<Int32> nonBgIds = GetRecordIds(source.SubMapRecords, Classifications.BG, false);
                        TractsSatisfyingSubMapSpecification tSpec = new TractsSatisfyingSubMapSpecification(tractIds, bgIds, nonBgIds);
                        IEnumerable<Tract> tracts = tSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.TractRepository);
                        List<ToPrintArea> tAreas = new List<ToPrintArea>();
                        foreach (Tract tract in tracts)
                        {
                            var areaData = campaign.CampaignTractImporteds.Where(t => t.Tract.Id == tract.Id).SingleOrDefault();


                            ToPrintArea area = new ToPrintArea()
                            {
                                Id = tract.Id,
                                Code = tract.ArbitraryUniqueCode,
                                IsEnabled = tract.IsEnabled,
                              
                            };

                            if (areaData != null)
                            {
                                area.Total = areaData.Total;
                                area.Count = areaData.Penetration;
                            }

                            int size = tract.TractCoordinates.Count;
                            ToCoordinate[] ps = new ToCoordinate[size];
                            for (int i = 0; i < size; i++)
                            {
                                ps[i] = new ToCoordinate()
                                {
                                    Latitude = tract.TractCoordinates[i].Latitude,
                                    Longitude = tract.TractCoordinates[i].Longitude
                                };
                            }
                            area.Coordinates = ps;
                            tAreas.Add(area);
                            //nonTractIds.Add(tract.Id);

                        }
                        target.Tracts = tAreas.ToArray();                      
                        
                        List<ToPrintArea> bAreas = new List<ToPrintArea>();                        
                        target.BlockGroups = bAreas.ToArray();

                        target.FiveZipAreas = new ToPrintArea[0];
                        target.CRoutes = new ToPrintArea[0];
                    }
                    else
                    {
                        List<Int32> tractIds = GetRecordIds(source.SubMapRecords, Classifications.TRK, true);
                        List<Int32> nonTractIds = new List<int>();
                        List<Int32> bgIds = GetRecordIds(source.SubMapRecords, Classifications.BG, true);
                        List<Int32> nonBgIds = GetRecordIds(source.SubMapRecords, Classifications.BG, false);
                        //TractsSatisfyingSubMapSpecification tSpec = new TractsSatisfyingSubMapSpecification(tractIds, bgIds, nonBgIds);
                        //IEnumerable<Tract> tracts = tSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.TractRepository);
                        List<ToPrintArea> tAreas = new List<ToPrintArea>();
                        //foreach (Tract tract in tracts)
                        //{
                        //    var areaData = campaign.CampaignTractImporteds.Where(t => t.Tract.Id == tract.Id).SingleOrDefault();

                        //    if (areaData != null)
                        //    {
                        //        ToPrintArea area = new ToPrintArea()
                        //        {
                        //            Id = tract.Id,
                        //            Code = tract.ArbitraryUniqueCode,
                        //            IsEnabled = tract.IsEnabled,
                        //            Total = areaData.Total,
                        //            Count = areaData.Penetration
                        //        };

                        //        int size = tract.TractCoordinates.Count;
                        //        ToCoordinate[] ps = new ToCoordinate[size];
                        //        for (int i = 0; i < size; i++)
                        //        {
                        //            ps[i] = new ToCoordinate()
                        //            {
                        //                Latitude = tract.TractCoordinates[i].Latitude,
                        //                Longitude = tract.TractCoordinates[i].Longitude
                        //            };
                        //        }
                        //        area.Coordinates = ps;
                        //        tAreas.Add(area);
                        //        nonTractIds.Add(tract.Id);
                        //    }
                        //}
                        target.Tracts = tAreas.ToArray();

                        BlockGroupsSatisfyingSubMapSpecification cSpec = new BlockGroupsSatisfyingSubMapSpecification(
                   tractIds, nonTractIds, bgIds, nonBgIds);
                        IEnumerable<BlockGroup> bgs = cSpec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.BlockGroupRepository);
                        List<ToPrintArea> bAreas = new List<ToPrintArea>();
                        foreach (BlockGroup bg in bgs)
                        {
                            ToPrintArea area = new ToPrintArea()
                            {
                                Id = bg.Id,
                                Code = bg.ArbitraryUniqueCode,
                                IsEnabled = bg.IsEnabled
                            };
                            var areaData = campaign.CampaignBlockGroupImporteds.Where(t => t.BlockGroup.Id == bg.Id).SingleOrDefault();
                            if (areaData != null)
                            {
                                area.Total = areaData.Total;
                                area.Count = areaData.Penetration;
                            }

                            int size = bg.BlockGroupCoordinates.Count;
                            ToCoordinate[] ps = new ToCoordinate[size];
                            for (int i = 0; i < size; i++)
                            {
                                ps[i] = new ToCoordinate()
                                {
                                    Latitude = bg.BlockGroupCoordinates[i].Latitude,
                                    Longitude = bg.BlockGroupCoordinates[i].Longitude
                                };
                            }
                            area.Coordinates = ps;
                            bAreas.Add(area);
                        }
                        target.BlockGroups = bAreas.ToArray();

                        target.FiveZipAreas = new ToPrintArea[0];
                        target.CRoutes = new ToPrintArea[0];
                    }



                }
                else
                {
                    target.Tracts = new ToPrintArea[0];
                    target.BlockGroups = new ToPrintArea[0];
                    target.FiveZipAreas = new ToPrintArea[0];
                    target.CRoutes = new ToPrintArea[0];
                }
            }
        }

        private static List<Int32> GetRecordIds(IEnumerable<SubMapRecord> records, Classifications classification, bool value)
        {
            List<Int32> ids = new List<int>();
            foreach (SubMapRecord record in records)
            {
                if (record.Classification == classification && record.Value == value)
                {
                    ids.Add(record.AreaId);
                }
            }
            return ids;
        }


    }
}
