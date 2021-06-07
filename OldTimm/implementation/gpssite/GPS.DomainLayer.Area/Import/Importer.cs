using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Utilities.IO;
using GPS.DataLayer.ValueObjects;
using GPS.DomainLayer.Enum;
using GPS.DataLayer;
using GPS.DomainLayer.QuerySpecifications;
using GPS.DomainLayer.Area.AreaOperators;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.DataInfrastructure;
using log4net;
using System.Data.SqlClient;

namespace GPS.DomainLayer.Area.Import
{
    public class Importer
    {
        public IEnumerable<AreaRecord> ImportFile(int campaignId, string filePath)
        {
            IEnumerable<AreaRecord> aRecords = new List<AreaRecord>();
            IEnumerable<ToAreaData> areaDatas = new List<ToAreaData>();
            FileOperator fileOper = new FileOperator();
            Classifications classification = Classifications.Z3;
            List<CsvAreaRecord> records = new List<CsvAreaRecord>();

            Campaign campaign;
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);

                if (campaign == null) return aRecords;
            }

            if (fileOper.GetFileType(filePath) == FileOperator.FileTypes.Excel)
            {
                GPS.Utilities.OfficeHelpers.AreaRecord[] oRecords = fileOper.GetAreaRecords(filePath);
                if (oRecords != null && oRecords.Length > 1)
                {
                    classification = GetClassfication(oRecords[0].Code);
                    if (classification != Classifications.Z3)
                    {

                        for (int i = 1; i < oRecords.Length; i++)
                        {
                            records.Add(new CsvAreaRecord()
                            {
                                Code = oRecords[i].Code,
                                Total = oRecords[i].Total,
                                Penetration = oRecords[i].Penetration
                            });
                        }

                    }

                }
            }
            else
            {
                CsvAreaRecord[] fRecords = fileOper.ReadFile<CsvAreaRecord>(filePath);
                if (fRecords != null && fRecords.Length > 1) // Special for importing croute
                {
                    if (GetClassfication(fRecords[0].Code) == Classifications.PremiumCRoute)
                    {
                        // relodad the file to get the data
                        var lines = System.IO.File.ReadAllLines(filePath);
                        var newRecords = new List<CsvAreaRecord>();
                        foreach (var line in lines)
                        {
                            var pms = line.Split(',');
                            if (pms.Length != 2) break; // the end of file

                            newRecords.Add(new CsvAreaRecord()
                            {
                                Code = pms[0],
                                Penetration = pms[1]
                            });                           
                        }
                        
                        if (newRecords[0].Penetration.ToUpper() != "PENETRATION" || newRecords[0].Code.ToUpper() != "PREMIUMCROUTE")
                        {
                            MyException e = new MyException("Column name is invalid!");
                            throw e;
                        }

                        classification = GetClassfication(fRecords[0].Code);

                        #region restore the origin data to client data - ticket 484
                        var condition = string.Empty;

                        newRecords.RemoveAt(0);  // remove the columns name row

                        foreach (var nr in newRecords)
                        {
                            double parsedPeneration;
                            if (!double.TryParse(nr.Penetration, out parsedPeneration) || parsedPeneration < 0)
                            {
                                var errorMessage = string.Format("peneration is out range; it's {0}. Peneration is {1}", nr.Code, parsedPeneration);
                                throw new MyException(errorMessage);
                            }
                        }

                        for (int i = 0; i < newRecords.Count; i++)
                        {
                            condition += string.Format(@"'{0}',", newRecords[i].Code);
                        }

                        condition = condition.Remove(condition.Length - 1); // remove last ','

                        var connString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["TIMMContext"];

                        // get the origin croute
                        using (var conn = new SqlConnection(connString.ConnectionString))
                        {
                            // 1. filter premiumcroutepart table by imported part_geocode list 
                            string query = string.Format(
                                @"SELECT [final_geocode], [part_geocode],
                                [final_home],[final_apt],[final_bus],[final_tot],
                                [part_home], [part_apt], [part_bus],[part_tot],
                                [premiumcroute_id]                                
                                FROM premiumcrouteparts 
                                WHERE [part_geocode] in ({0})
                                ORDER BY [part_geocode]", condition);

                            var cmd = new SqlCommand(query, conn);
                            cmd.Connection.Open();
                            var reader = cmd.ExecuteReader();

                            var rawCrouteParts = new List<List<object>>();
                            while (reader.Read())
                            {
                                var r = new List<object>();
                                for (int i = 0; i < 11; i++)
                                {
                                    r.Add(reader[i]);
                                }

                                rawCrouteParts.Add(r);
                            }

                            // 2. combine imported peneratation
                            var combinedCrouteParts = from cp in rawCrouteParts
                                                     join r in newRecords on cp[1] equals r.Code into rs
                                                     from r in rs.DefaultIfEmpty()
                                                     select new
                                                     {
                                                         final_geocode = cp[0] as string,
                                                         part_geocode = cp[1] as string,
                                                         final_home = System.Convert.ToDouble(cp[2]),
                                                         final_apt = System.Convert.ToDouble(cp[3]),
                                                         final_bus = System.Convert.ToDouble(cp[4]),
                                                         final_tot = System.Convert.ToDouble(cp[5]),
                                                         part_home = System.Convert.ToDouble(cp[6]),
                                                         part_apt = System.Convert.ToDouble(cp[7]),
                                                         part_bus = System.Convert.ToDouble(cp[8]),
                                                         part_tot = System.Convert.ToDouble(cp[9]),
                                                         premiumcroute_id = cp[10],
                                                         penertation = double.Parse(r.Penetration) / 100,
                                                         //part_apt_home_total = System.Convert.ToDouble(cp[6]) + System.Convert.ToDouble(cp[7]),
                                                         //count = System.Math.Round(System.Convert.ToDouble(cp[5]) * (double.Parse(r.Penetration) / 100), 0)
                                                     };

                            // calculate total as config
                            Func<double, double, double> calByConfigFunc = (apt, home) =>
                                {
                                    double typedTotal;
                                    switch (campaign.AreaDescription)
                                    {
                                        case "APT ONLY":
                                            typedTotal = apt;
                                            break;
                                        case "HOME ONLY":
                                            typedTotal = home;
                                            break;
                                        case "APT + HOME":
                                        default:
                                            typedTotal = apt + home;
                                            //result.Count += 0;
                                            break;
                                    }

                                    return typedTotal;
                                };

                            // 3. group by final_geocode/premiumcroute_id, sum the count and get default final_residental_total
                            var finalGeocodeGroups = from cp in combinedCrouteParts
                                                     group cp by cp.premiumcroute_id into cps
                                                     orderby cps.Key
                                                     select new
                                                     {
                                                         premiumcroute_id = cps.Key.ToString(),
                                                         total = calByConfigFunc(cps.FirstOrDefault().final_apt, cps.FirstOrDefault().final_home), //cps.FirstOrDefault().final_home + cps.FirstOrDefault().final_apt,
                                                         count = cps.Sum(cpsi=>System.Math.Round(cpsi.penertation * calByConfigFunc(cpsi.part_apt, cpsi.part_home), 0))
                                                     };

                            // 4. update areaDatas
                            foreach (var g in finalGeocodeGroups)
                            {                                
                                (areaDatas as List<ToAreaData>).Add(
                               new ToAreaData()
                               {
                                   Id = System.Convert.ToInt32(g.premiumcroute_id),
                                   Total = (int)g.total,
                                   Count = (int)g.count
                               });
                            }

                            // 5. create fake records for return records 'aRecords'
                            foreach (var g in combinedCrouteParts.Select(p => p.final_geocode).Distinct())
                            {
                                records.Add(new CsvAreaRecord()
                                {
                                    Code = g,
                                    Total = "0", //  fake
                                    Penetration = "0", // fake
                                });
                            }
                        }

                        // update firstly
                        SaveCampaignImportData(campaignId, classification, areaDatas);

                        // prepare return value
                        areaDatas = GetImportedDataSpecification(classification, records);
                        aRecords = ConvertAreaRecord(classification, areaDatas);

                        return aRecords;

                        #endregion
                    }
                    else // origin logic of importing zip or layers
                    {
                        if (fRecords[0].Penetration.ToUpper() != "PENETRATION" || fRecords[0].Total.ToUpper() != "TOTAL")
                        {
                            MyException e = new MyException("Column name is invalid!");
                            throw e;
                        }
                        classification = GetClassfication(fRecords[0].Code);
                        if (classification != Classifications.Z3)
                        {
                            for (int i = 1; i < fRecords.Length; i++)
                            {
                                records.Add(fRecords[i]);
                            }

                        }
                    }
                }
            }

            if (classification != Classifications.Z3)
            {
                areaDatas = GetImportedDataSpecification(classification, records);
                aRecords = ConvertAreaRecord(classification, areaDatas);
                //ImportDataCache.Push(classification, areaDatas);
                SaveCampaignImportData(campaignId, classification, areaDatas);
            }

            return aRecords;
        }

        private void SaveCampaignImportData(int campaignId, Classifications classification, IEnumerable<ToAreaData> datas)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    switch (classification)
                    {
                        case Classifications.Z5:
                            foreach (ToAreaData data in datas)
                            {
                                var fData = campaign.CampaignFiveZipImporteds.Where(t => t.FiveZipArea.Id == data.Id).SingleOrDefault();
                                if (fData != null)
                                {
                                    fData.Total = data.Total;
                                    fData.Penetration = data.Count;                                    
                                }
                                else
                                {
                                    campaign.CampaignFiveZipImporteds.Add(new CampaignFiveZipImported()
                                    {
                                        FiveZipArea = new FiveZipArea() { Id = data.Id },
                                        Total = data.Total,
                                        Penetration = data.Count,
                                        Campaign = campaign
                                    });
                                }
                            }
                            ws.Repositories.CampaignRepository.Update(campaign);
                            ws.Commit();
                            JsonAreaCache.ClearCampaignAreas();
                            break;
                        case Classifications.PremiumCRoute:
                            using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
                            {
                                using (ITransaction tx = bws.BeginTransaction())
                                {
                                    try
                                    {
                                        foreach (ToAreaData data in datas)
                                        {
                                            var fData = campaign.CampaignCRouteImporteds.Where(t => t.PremiumCRoute.Id == data.Id).SingleOrDefault();
                                            if (fData != null)
                                            {
                                                fData.Total = data.Total;
                                                fData.Penetration = data.Count;

                                                fData.IsPartModified = true;
                                                fData.PartPercentage = 1;

                                                bws.Repositories.BulkCampaignCRouteRepository.Update(fData);
                                            }
                                            else
                                            {
                                                bws.Repositories.BulkCampaignCRouteRepository.Add(new CampaignCRouteImported()
                                                {
                                                    PremiumCRoute = new PremiumCRoute() { Id = data.Id },
                                                    Total = data.Total,
                                                    Penetration = data.Count,
                                                    Campaign = campaign,

                                                    IsPartModified = true,
                                                    PartPercentage = 1
                                                });
                                            }
                                        }

                                        tx.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        tx.Rollback();
                                        ILog logger = log4net.LogManager.GetLogger(typeof(Importer));
                                        logger.Error("Compentent Unhandle Error", ex);
                                    }
                                }
                            }
                            JsonAreaCache.ClearCampaignAreas();
                            break;
                        case Classifications.TRK:
                            foreach (ToAreaData data in datas)
                            {
                                var fData = campaign.CampaignTractImporteds.Where(t => t.Tract.Id == data.Id).SingleOrDefault();
                                if (fData != null)
                                {
                                    fData.Total = data.Total;
                                    fData.Penetration = data.Count;
                                }
                                else
                                {
                                    campaign.CampaignTractImporteds.Add(new CampaignTractImported()
                                    {
                                        Tract = new Tract() { Id = data.Id },
                                        Total = data.Total,
                                        Penetration = data.Count,
                                        Campaign = campaign
                                    });
                                }
                            }
                            ws.Repositories.CampaignRepository.Update(campaign);
                            ws.Commit();
                            JsonAreaCache.ClearCampaignAreas();
                            break;
                        case Classifications.BG:
                            if (datas.Count() > 0)
                            {
                                using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
                                {
                                    using (ITransaction tx = bws.BeginTransaction())
                                    {
                                        try
                                        {
                                            foreach (ToAreaData data in datas)
                                            {
                                                var fData = campaign.CampaignBlockGroupImporteds.Where(t => t.BlockGroup.Id == data.Id).SingleOrDefault();
                                                if (fData != null)
                                                {
                                                    fData.Total = data.Total;
                                                    fData.Penetration = data.Count;
                                                    bws.Repositories.BulkCampaignBGRepository.Update(fData);
                                                }
                                                else
                                                {
                                                    bws.Repositories.BulkCampaignBGRepository.Add(new CampaignBlockGroupImported()
                                                    {
                                                        BlockGroup = new BlockGroup() { Id = data.Id },
                                                        Total = data.Total,
                                                        Penetration = data.Count,
                                                        Campaign = campaign
                                                    });
                                                }
                                            }
                                            tx.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            tx.Rollback();
                                            ILog logger = log4net.LogManager.GetLogger(typeof(Importer));
                                            logger.Error("Compentent Unhandle Error", ex);
                                        }
                                    }
                                }
                            }
                            JsonAreaCache.ClearCampaignAreas();
                            break;
                    }

                    foreach (SubMap submap in campaign.SubMaps)
                    {
                        //int total = 0;
                        //int count = 0;
                        //foreach (SubMapRecord toArea in submap.SubMapRecords)
                        //{
                           
                        //}   
                       
                        
                    }
                }
            }
        }

        public IEnumerable<AreaRecord> ConvertAreaRecord(Classifications classification, IEnumerable<ToAreaData> areaDatas)
        {
            List<AreaRecord> records = new List<AreaRecord>();
            AreaOperatorBase areaOper = AreaOperatorFacory.CreateOperator(classification);

            foreach (ToAreaData areaData in areaDatas)
            {
                records.Add(new AreaRecord()
                {
                    Classification = classification,
                    AreaId = areaData.Id,
                    Value = true,
                    Relations = GetRelations(classification, areaData.PremiumCRouteSelectMappings, areaData.BlockGroupSelectMappings)

                });
                JsonAreaTempCache.AddKeys(classification, areaData.BoxIds);
            }
            return records;
        }

        #region GetRelations
        private Dictionary<int, Dictionary<int, bool>> GetRelations(Classifications classification, IEnumerable<ToPremiumCRouteSelectMapping> mappings, IEnumerable<ToBlockGroupSelectMapping> bgMappings)
        {
            Dictionary<int, Dictionary<int, bool>> relations;
            switch (classification)
            {
                case Classifications.Z5:
                    relations = GetFiveZipRelations(mappings);
                    break;
                case Classifications.TRK:
                    relations = GetTractRelations(bgMappings);
                    break;
                case Classifications.BG:
                    relations = GetBlockGroupRelations(bgMappings);
                    break;
                case Classifications.PremiumCRoute:
                    relations = GetCRouteRelations(mappings);
                    break;
                default:
                    relations = new Dictionary<int, Dictionary<int, bool>>();
                    break;
            }
            return relations;
        }

        private Dictionary<int, Dictionary<int, bool>> GetFiveZipRelations(IEnumerable<ToPremiumCRouteSelectMapping> mappings)
        {
            Dictionary<int, Dictionary<int, bool>> relations = new Dictionary<int, Dictionary<int, bool>>();
            foreach (var mapping in mappings)
            {
                // add Z3 relations
                if (!relations.ContainsKey((int)Classifications.Z3))
                {
                    relations.Add((int)Classifications.Z3, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rz3s = relations[(int)Classifications.Z3];
                if (!rz3s.ContainsKey(mapping.ThreeZipId))
                {
                    rz3s.Add(mapping.ThreeZipId, true);
                }
            }
            return relations;
        }

        private Dictionary<int, Dictionary<int, bool>> GetTractRelations(IEnumerable<ToBlockGroupSelectMapping> mappings)
        {
            Dictionary<int, Dictionary<int, bool>> relations = new Dictionary<int, Dictionary<int, bool>>();
            foreach (var mapping in mappings)
            {
                // add Z5 relations
                if (!relations.ContainsKey((int)Classifications.Z5))
                {
                    relations.Add((int)Classifications.Z5, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rz5s = relations[(int)Classifications.Z5];
                if (!rz5s.ContainsKey(mapping.FiveZipId))
                {
                    rz5s.Add(mapping.FiveZipId, true);
                }

                // add Z3 relations
                if (!relations.ContainsKey((int)Classifications.Z3))
                {
                    relations.Add((int)Classifications.Z3, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rz3s = relations[(int)Classifications.Z3];
                if (!rz3s.ContainsKey(mapping.ThreeZipId))
                {
                    rz3s.Add(mapping.ThreeZipId, true);
                }
            }
            return relations;
        }

        private Dictionary<int, Dictionary<int, bool>> GetBlockGroupRelations(IEnumerable<ToBlockGroupSelectMapping> mappings)
        {
            Dictionary<int, Dictionary<int, bool>> relations = new Dictionary<int, Dictionary<int, bool>>();
            foreach (var mapping in mappings)
            {
                // add trk relations
                if (!relations.ContainsKey((int)Classifications.TRK))
                {
                    relations.Add((int)Classifications.TRK, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rtrks = relations[(int)Classifications.TRK];
                if (!rtrks.ContainsKey(mapping.TractId))
                {
                    rtrks.Add(mapping.TractId, true);
                }

                // add Z5 relations
                if (!relations.ContainsKey((int)Classifications.Z5))
                {
                    relations.Add((int)Classifications.Z5, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rz5s = relations[(int)Classifications.Z5];
                if (!rz5s.ContainsKey(mapping.FiveZipId))
                {
                    rz5s.Add(mapping.FiveZipId, true);
                }

                // add Z3 relations
                if (!relations.ContainsKey((int)Classifications.Z3))
                {
                    relations.Add((int)Classifications.Z3, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rz3s = relations[(int)Classifications.Z3];
                if (!rz3s.ContainsKey(mapping.ThreeZipId))
                {
                    rz3s.Add(mapping.ThreeZipId, true);
                }
            }
            return relations;
        }

        private Dictionary<int, Dictionary<int, bool>> GetCRouteRelations(IEnumerable<ToPremiumCRouteSelectMapping> mappings)
        {
            Dictionary<int, Dictionary<int, bool>> relations = new Dictionary<int, Dictionary<int, bool>>();
            //foreach (var mapping in mappings)
            //{
            //    // add Z5 relations
            //    if (!relations.ContainsKey((int)Classifications.Z5))
            //    {
            //        relations.Add((int)Classifications.Z5, new Dictionary<int, bool>());
            //    }
            //    Dictionary<int, bool> rz5s = relations[(int)Classifications.Z5];
            //    if (!rz5s.ContainsKey(mapping.FiveZipId))
            //    {
            //        rz5s.Add(mapping.FiveZipId, true);
            //    }

            //    // add Z3 relations
            //    if (!relations.ContainsKey((int)Classifications.Z3))
            //    {
            //        relations.Add((int)Classifications.Z3, new Dictionary<int, bool>());
            //    }
            //    Dictionary<int, bool> rz3s = relations[(int)Classifications.Z3];
            //    if (!rz3s.ContainsKey(mapping.ThreeZipId))
            //    {
            //        rz3s.Add(mapping.ThreeZipId, true);
            //    }
            //}
            return relations;
        }
        #endregion
        private Classifications GetClassfication(string fieldName)
        {
            if (fieldName == "ZIP")
            {
                return Classifications.Z5;
            }
            else if (fieldName == "CNSTT TRACKS")
            {
                return Classifications.TRK;
            }
            else if (fieldName == "CTBKG BLOCK GROUPS")
            {
                return Classifications.BG;
            }
            else if (fieldName == "PREMIUMCROUTE")
            {
                return Classifications.PremiumCRoute;
            }
            else
            {
                return Classifications.Z3;
            }
        }

        private IEnumerable<ToAreaData> GetImportedDataSpecification(Classifications classification, IEnumerable<CsvAreaRecord> records)
        {
            IEnumerable<ToAreaData> areaDatas;
            ImportedDataSpecification spec = new ImportedDataSpecification();
            switch (classification)
            {
                case Classifications.Z5:
                    areaDatas = spec.GetImportedData(records, new FiveZipRepository());
                    break;
                case Classifications.TRK:
                    areaDatas = spec.GetImportedData(records, new TractRepository());
                    break;
                case Classifications.BG:
                    areaDatas = spec.GetImportedData(records, new BlockGroupRepository());
                    break;
                case Classifications.PremiumCRoute:
                    areaDatas = spec.GetImportedData(records, new PremiumCRouteRepository());
                    break;
                default:
                    areaDatas = new List<ToAreaData>();
                    break;
            }

            return areaDatas;
        }

        public void AdjustCountReset(int campaignId, int classification, string zipCode)
        { 
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    Classifications c = (Classifications)classification;
                    switch (c)
                    {
                        case Classifications.Z5:
                            foreach (CampaignFiveZipImported area in campaign.CampaignFiveZipImporteds)
                            {
                                if (area.FiveZipArea.Code == zipCode)
                                {
                                    area.PartPercentage = 0;
                                    area.IsPartModified = true;
                                }
                            }

                            ws.Repositories.CampaignRepository.Update(campaign);
                            ws.Commit();
                            JsonAreaCache.ClearCampaignAreas();
                            break;
                    }
                }
            }
        }

        public void AdjustCount(int campaignId, int classification, int areaId, int total, int count, float per, bool isModified,bool isImport)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    Classifications c = (Classifications)classification;
                    switch (c)
                    {
                        case Classifications.Z5:
                            var fData = campaign.CampaignFiveZipImporteds.Where(t => t.FiveZipArea.Id == areaId).SingleOrDefault();
                            if (fData != null)
                            {
                                if (isModified)
                                {
                                    fData.PartPercentage = per;
                                    fData.IsPartModified = isModified;
                                }
                                else
                                {
                                    fData.Total = total;
                                    fData.Penetration = count;
                                }
                            }
                            else
                            {
                                if (isImport)
                                {
                                   
                                    campaign.CampaignFiveZipImporteds.Add(new CampaignFiveZipImported()
                                    {
                                        FiveZipArea = new FiveZipArea() { Id = areaId },
                                        Total = total,                                      
                                        Penetration = count,
                                        PartPercentage = per,
                                        IsPartModified = isModified,
                                        Campaign = campaign
                                    });
                                }
                                else
                                {
                                    FiveZipArea area = ws.Repositories.FiveZipRepository.GetItem(areaId);
                                    campaign.CampaignFiveZipImporteds.Add(new CampaignFiveZipImported()
                                    {
                                        //FiveZipArea = new FiveZipArea() { Id = areaId },                                    
                                        //Total = total,
                                        FiveZipArea = area,
                                        Total = area.APT_COUNT + area.HOME_COUNT,
                                        Penetration = count,
                                        PartPercentage = per,
                                        IsPartModified = isModified,
                                        Campaign = campaign
                                    });

                                }
                            }
                            ws.Repositories.CampaignRepository.Update(campaign);
                            ws.Commit();
                            JsonAreaCache.ClearCampaignAreas();
                            break;
                        case Classifications.PremiumCRoute:
                            var cData = campaign.CampaignCRouteImporteds.Where(t => t.PremiumCRoute.Id == areaId).SingleOrDefault();
                            if (cData != null)
                            {
                                if (isModified)
                                {
                                    cData.PartPercentage = per;
                                    cData.IsPartModified = isModified;
                                }
                                else
                                {
                                    cData.Total = total;
                                    cData.Penetration = count;
                                }
                            }
                            else
                            {
                                if (isImport)
                                {
                                    
                                    campaign.CampaignCRouteImporteds.Add(new CampaignCRouteImported()
                                    {
                                        PremiumCRoute = new PremiumCRoute() { Id = areaId },
                                        Total = total,                                       
                                        Penetration = count,
                                        PartPercentage = per,
                                        IsPartModified = isModified,
                                        Campaign = campaign
                                    });
                                }
                                else
                                {
                                    PremiumCRoute area = ws.Repositories.PremiumCRouteRepository.GetItem(areaId);

                                    int typedTotal;
                                    switch (campaign.AreaDescription)
                                    {
                                        case "APT ONLY":
                                            typedTotal = area.APT_COUNT;
                                            break;
                                        case "HOME ONLY":
                                            typedTotal = area.HOME_COUNT;
                                            break;
                                        case "APT + HOME":
                                        default:
                                            typedTotal = area.HOME_COUNT + area.APT_COUNT;
                                            //result.Count += 0;
                                            break;
                                    }

                                    campaign.CampaignCRouteImporteds.Add(new CampaignCRouteImported()
                                    {
                                        //PremiumCRoute = new PremiumCRoute() { Id = areaId },
                                        //Total = total,
                                        PremiumCRoute = area,
                                        Total = typedTotal, //area.APT_COUNT + area.HOME_COUNT,
                                        Penetration = count,
                                        PartPercentage = per,
                                        IsPartModified = isModified,
                                        Campaign = campaign
                                    });
                                }
                            }
                            ws.Repositories.CampaignRepository.Update(campaign);
                            ws.Commit();
                            JsonAreaCache.ClearCampaignAreas();
                            break;
                        case Classifications.TRK:
                            var tData = campaign.CampaignTractImporteds.Where(t => t.Tract.Id == areaId).SingleOrDefault();
                            if (tData != null)
                            {
                                tData.Total = total;
                                tData.Penetration = count;
                            }
                            else
                            {
                                campaign.CampaignTractImporteds.Add(new CampaignTractImported()
                                {
                                    Tract = new Tract() { Id = areaId },
                                    Total = total,
                                    Penetration = count,
                                    Campaign = campaign
                                });
                            }
                            ws.Repositories.CampaignRepository.Update(campaign);
                            ws.Commit();
                            JsonAreaCache.ClearCampaignAreas();
                            break;
                        case Classifications.BG:
                            var bData = campaign.CampaignBlockGroupImporteds.Where(t => t.BlockGroup.Id == areaId).SingleOrDefault();
                            if (bData != null)
                            {
                                bData.Total = total;
                                bData.Penetration = count;
                            }
                            else
                            {
                                campaign.CampaignBlockGroupImporteds.Add(new CampaignBlockGroupImported()
                                {
                                    BlockGroup = new BlockGroup() { Id = areaId },
                                    Total = total,
                                    Penetration = count,
                                    Campaign = campaign
                                });
                            }
                            ws.Repositories.CampaignRepository.Update(campaign);
                            ws.Commit();
                            JsonAreaCache.ClearCampaignAreas();
                            break;
                    }
                }
            }

        }

    }
}
