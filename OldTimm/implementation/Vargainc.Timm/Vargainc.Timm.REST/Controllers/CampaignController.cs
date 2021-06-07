using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Collections.Concurrent;
using Vargainc.Timm.EF;
using Vargainc.Timm.Models;
using System.Data.Entity;
using System.Text;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;
using NetTopologySuite.IO.KML;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("campaign")]
    public class CampaignController : ApiController
    {
        private TimmContext db = new TimmContext();

        #region Calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
        private const string APT = "APT ONLY";
        private const string HOME = "HOME ONLY";
        private const string APT_HOME = "APT + HOME";
        #endregion

        private void FixLogoPath(List<Campaign> result)
        {
            string basePath = ConfigurationManager.AppSettings["campaignimagepath"];
            basePath = basePath.Substring(1).TrimEnd('/');
            foreach (var item in result)
            {
                if (!string.IsNullOrWhiteSpace(item.Logo))
                {
                    item.LogoPath = string.Format("{0}/{1}",
                        basePath,
                        item.Logo);
                }
            }

        }

        private void FixLogoPath(Campaign result)
        {
            FixLogoPath(new List<Campaign> { result });
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCampaignList()
        {
            var result = await db.Campaigns.Include(i => i.Status)
                .OrderByDescending(i=>i.Date)
                .ThenBy(i=>i.CustemerName)
                .ThenBy(i=>i.UserName)
                .ThenByDescending(i=>i.Sequence)
                .ToListAsync();
            List<Models.Campaign> allowCopyCampaign = new List<Models.Campaign>();
            foreach(var item in result)
            {
                if((item.Status.Max(i=>i.Status) ?? 0) < 1 && item.SubMaps.Count() > 0)
                {
                    allowCopyCampaign.Add(item);
                }
            }
            return Json(allowCopyCampaign.Select(i => new
            {
                i.Id,
                Name = string.Format("{0}-{1}-{2}-{3}-{4}",
                    i.Date.Value.ToString("MMddyy"),
                    i.ClientCode,
                    i.CreatorName,
                    i.AreaDescription,
                    i.Sequence),
                i.UserName
            }));
        }

        [Route("{campaignId:int}/export")]
        [HttpGet]
        public async Task<IHttpActionResult> Export(int campaignId)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId);
            if (campaign == null)
            {
                return NotFound();
            }
            var submaps = campaign.SubMaps.OrderBy(i => i.OrderId)
                .Select(s => new
                {
                    s.Name,
                    s.ColorR,
                    s.ColorG,
                    s.ColorB,
                    s.ColorString,
                    s.CountAdjustment,
                    s.TotalAdjustment,
                    SubMapCRoutes = s.SubMapRecords.Where(i=>i.Classification == 15 && i.Value == true).Select(i=>i.AreaId).ToList(),
                    SubMapAreas = new List<ViewModel.ImportCRoute>()
                }).ToArray();

            var address = campaign.Addresses.Select(i => new {
                i.AddressName,
                i.Color,
                i.Latitude,
                i.Longitude,
                i.OriginalLatitude,
                i.OriginalLongitude,
                i.Picture,
                i.ZipCode,
                Radiuses = i.Radiuses.Select(r=>new { r.Length, r.LengthMeasuresId, r.IsDisplay }).ToList()
            });
            var userId = await db.Status.Where(i => i.CampaignId == campaign.Id).OrderBy(i => i.Status).Select(i=>i.UserId).FirstOrDefaultAsync();            
            var user = db.Users.FirstOrDefault(i=>i.Id == userId);
            var classifications = campaign.CampaignClassifications.Select(i => i.Classification).ToList();

            var allAreas = submaps.SelectMany(i => i.SubMapCRoutes).ToList();
            var allAreaMap = await QueryCRoutesById(allAreas);

            foreach (var item in submaps)
            {
                foreach(var id in item.SubMapCRoutes)
                {
                    if(id.HasValue && allAreaMap.ContainsKey(id.Value))
                    {
                        item.SubMapAreas.Add(allAreaMap[id.Value]);
                    }
                }                
            }
            var colors = campaign.CampaignPercentageColors.Select(i => new
            {
                i.ColorId,
                i.Max,
                i.Min
            }).ToList();

            var cRouteImporteds = await QueryPenetrationByCampaignId(campaignId);

            return Json(new {
                campaign.AreaDescription,
                campaign.ClientCode,
                campaign.ClientName,
                campaign.ContactName,
                campaign.CreatorName,
                campaign.CustemerName,
                campaign.Date,
                campaign.Description,
                campaign.Latitude,
                campaign.Longitude,
                campaign.Logo,
                campaign.Name,
                campaign.ZoomLevel,
                campaign.UserName,
                UserStatus = user.UserName,
                Address = address,
                Classifications = classifications,
                SubMap = submaps != null ? submaps.Select(s => new {
                    s.ColorR,
                    s.ColorG,
                    s.ColorB,
                    s.ColorString,
                    s.Name,
                    s.CountAdjustment,
                    s.TotalAdjustment,
                    CRoutes = s.SubMapAreas
                }) : null,
                PercentageColors = colors,
                //Penetration = cRouteImporteds
            });
            
        }

        [Route("import")]
        [HttpPost]
        public async Task<IHttpActionResult> Import([FromBody]ViewModel.ImportCampaign campaign)
        {
            StringBuilder info = new StringBuilder();
            var crouteGeoCode = campaign.SubMap.SelectMany(i => i.CRoutes).Select(i=>i.GEOCODE).ToList();
            var allCRoutes = await QueryCRoutesByGeoCode(crouteGeoCode);

            #region Check
            List<Models.Location> boundary = null;
            List<int> addedCRoute = new List<int>();
            StringBuilder message = null;
            List<int> systemCRoutes = new List<int>();
            foreach (var submap in campaign.SubMap)
            {
                if(submap.CRoutes == null || submap.CRoutes.Count == 0)
                {
                    continue;
                }
                #region Check SubMAP
                foreach (var item in submap.CRoutes)
                {
                    if (!allCRoutes.ContainsKey(item.GEOCODE))
                    {
                        //return Json(new
                        //{
                        //    success = false,
                        //    info = string.Format("Find GEOCODE {0} failed in SUBMAP, the geocode is not exist", item.GEOCODE)
                        //});
                        continue;
                    }
                    systemCRoutes.AddRange(allCRoutes[item.GEOCODE].Select(i=>i.Id.Value));
                    //if(allCRoutes[item.GEOCODE].Count > item.PartCount)
                    //{
                    //    systemCRoutes.Add(allCRoutes[item.GEOCODE][item.PartCount ?? 0].Id.Value);
                    //    info.AppendFormat("added {0} {1}\n", item.GEOCODE, item.PartCount);
                    //}
                    //else
                    //{
                    //    return Json(new
                    //    {
                    //        success = false,
                    //        info = string.Format("Find GEOCODE {0} {1}/{2} failed in database\r\n{3}\r\n{4}", 
                    //        item.GEOCODE, 
                    //        item.PartCount ?? 0, 
                    //        item.Total ?? 0,
                    //        Newtonsoft.Json.JsonConvert.SerializeObject(allCRoutes[item.GEOCODE]),
                    //        Newtonsoft.Json.JsonConvert.SerializeObject(crouteGeoCode))
                    //    });
                    //}

                }
                if (!ValidateCRouteArea(systemCRoutes, out addedCRoute, out boundary, out message))
                {
                    return Json(new
                    {
                        success = false,
                        info = string.Format("validate croute in submap {0} failed.\r\n{1}", submap.Name, message)
                    });
                }
                submap.PremiumCRoutes = addedCRoute;
                submap.Boundary = boundary;

                #endregion

                #region Check DMap
                //if (submap.DMap != null && submap.DMap.Count > 0)
                //{
                //    foreach (var dmap in submap.DMap)
                //    {
                //        systemCRoutes.Clear();
                //        if (dmap.CRoutes == null || dmap.CRoutes.Count == 0)
                //        {
                //            continue;
                //        }
                //        foreach (var item in dmap.CRoutes)
                //        {
                //            if (!allCRoutes.ContainsKey(item.GEOCODE))
                //            {
                //                return Json(new
                //                {
                //                    success = false,
                //                    info = string.Format("Find GEOCODE {0} {1}/{2} failed in DMAP, the geocode is not exist", item.GEOCODE, item.PartCount, item.PartTotal)
                //                });
                //            }
                //            var currentCode = allCRoutes[item.GEOCODE];
                //            if (item.PartTotal != currentCode.Count)
                //            {
                //                return Json(new
                //                {
                //                    success = false,
                //                    info = string.Format("Find GEOCODE {0} {1}/{2} failed in DMAP, the croute partcount changed", item.GEOCODE, item.PartCount, item.PartTotal)
                //                });
                //            }
                //            var geoCode = currentCode.Where(i => i.PartCount == item.PartCount).FirstOrDefault();
                //            if (geoCode == null)
                //            {
                //                return Json(new
                //                {
                //                    success = false,
                //                    info = string.Format("Find GEOCODE {0} {1}/{2} failed in DMAP, the current part croute is not exist", item.GEOCODE, item.PartCount, item.PartTotal)
                //                });
                //            }
                //            systemCRoutes.Add(geoCode.Id.Value);
                //        }
                //        if (!ValidateCRouteArea(systemCRoutes, out boundary, out message))
                //        {
                //            return Json(new
                //            {
                //                success = false,
                //                info = string.Format("validate croute in submap {0} failed.\r\n{1}", submap.Name, message)
                //            });
                //        }
                //        dmap.PremiumCRoutes = systemCRoutes;
                //        dmap.Boundary = boundary;
                //    }
                //}
                #endregion
            }


            #endregion
            try
            {
                await SaveImportCampaign(campaign, allCRoutes);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, info = ex.ToString() });
            }

            //dbCampaign.Addresses
            return Json(new { success = true, info = info.ToString() });
        }

        private async Task<bool> SaveImportCampaign(ViewModel.ImportCampaign campaign, Dictionary<string, List<ViewModel.ImportCRoute>> allCRoutes)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ValidateOnSaveEnabled = false;
            #region Save Campaign
            var dbCampaign = db.Campaigns.Create();
            dbCampaign.AreaDescription = campaign.AreaDescription ?? "";
            dbCampaign.ClientCode = campaign.ClientCode ?? "";
            dbCampaign.ClientName = campaign.ClientName ?? "";
            dbCampaign.ContactName = campaign.ContactName ?? "";
            dbCampaign.CreatorName = campaign.CreatorName ?? "";
            dbCampaign.CustemerName = campaign.CustemerName ?? "";
            dbCampaign.Date = campaign.Date;
            dbCampaign.Description = campaign.Description ?? "";
            dbCampaign.Latitude = campaign.Latitude;
            dbCampaign.Logo = campaign.Logo;
            dbCampaign.Longitude = campaign.Longitude;
            dbCampaign.Name = campaign.Name ?? "";
            dbCampaign.UserName = campaign.UserName;
            dbCampaign.ZoomLevel = campaign.ZoomLevel;
            dbCampaign.Sequence = 1;
            db.Campaigns.Add(dbCampaign);
            await db.SaveChangesAsync();

            #endregion

            #region save campaign user mapping
            var user = await db.Users.FirstOrDefaultAsync(i => i.UserName == campaign.UserStatus);
            db.Status.Add(new StatusInfo
            {
                Campaign = dbCampaign,
                User = user,
                Status = 0
            });
            await db.SaveChangesAsync();
            #endregion

            #region Save SubMap
            int subMapIndex = 1;
            foreach (var subMap in campaign.SubMap)
            {
                var dbSubMap = db.SubMaps.Create();
                dbSubMap.Campaign = dbCampaign;
                dbSubMap.ColorR = subMap.ColorR;
                dbSubMap.ColorG = subMap.ColorG;
                dbSubMap.ColorB = subMap.ColorB;
                dbSubMap.ColorString = subMap.ColorString;
                dbSubMap.CountAdjustment = subMap.CountAdjustment ?? 0;
                dbSubMap.TotalAdjustment = subMap.TotalAdjustment ?? 0;
                dbSubMap.Name = subMap.Name;
                dbSubMap.OrderId = subMapIndex++;
                //these value will changed later after croute have insert into database.
                dbSubMap.Penetration = 0;
                dbSubMap.Percentage = 0;
                dbSubMap.Total = 0;
                

                db.SubMaps.Add(dbSubMap);
                await db.SaveChangesAsync();
                if (subMap.PremiumCRoutes != null && subMap.PremiumCRoutes.Count > 0)
                {
                    var submapRecord = subMap.PremiumCRoutes.Select(i => new SubMapRecord
                    {
                        AreaId = i,
                        Classification = 15,
                        SubMap = dbSubMap,
                        Value = true
                    });
                    db.SubMapRecords.AddRange(submapRecord);
                    await db.SaveChangesAsync();
                }

                //var partCampaignRecord = subMap.PremiumCRoutes.Select(i => new CampaignRecord
                //{
                //    AreaId = i,
                //    Classification = 15,
                //    Campaign = dbCampaign,
                //    Value = true
                //});

                //db.CampaignRecords.AddRange(partCampaignRecord);
                //await db.SaveChangesAsync();

                if (subMap.Boundary != null && subMap.Boundary.Count > 0)
                {
                    var subMapCoordinate = subMap.Boundary.Select(i => new SubMapCoordinate
                    {
                        Latitude = i.Latitude,
                        Longitude = i.Longitude,
                        SubMap = dbSubMap
                    }).ToList();
                    db.SubMapCoordinates.AddRange(subMapCoordinate);
                    await db.SaveChangesAsync();
                }
            }
            #endregion

            #region Save Classification
            await db.SaveChangesAsync();
            if (campaign.Classifications != null && campaign.Classifications.Count > 0)
            {
                var classification = campaign.Classifications.Select(i => new CampaignClassification
                {
                    Classification = i,
                    Campaign = dbCampaign
                });
                db.CampaignClassifications.AddRange(classification);
            }
            #endregion

            #region Save Address
            if (campaign.Address != null && campaign.Address.Count > 0)
            {
                foreach (var item in campaign.Address)
                {
                    var address = new Address
                    {
                        AddressName = item.AddressName,
                        Campaign = dbCampaign,
                        Color = item.Color,
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                        OriginalLatitude = item.OriginalLatitude,
                        OriginalLongitude = item.OriginalLongitude,
                        Picture = item.Picture,
                        ZipCode = item.ZipCode,
                    };

                    db.Addresses.Add(address);
                    await db.SaveChangesAsync();

                    if (item.Radiuses != null && item.Radiuses.Count > 0)
                    {
                        var addressRadiuses = item.Radiuses.Select(i => new Radiuses
                        {
                            IsDisplay = i.IsDisplay,
                            Length = i.Length,
                            LengthMeasuresId = i.LengthMeasuresId,
                            Address = address
                        });
                        db.Radiuses.AddRange(addressRadiuses);
                        await db.SaveChangesAsync();
                    }
                }
            }
            #endregion

            #region Save PercentageColors
            if (campaign.PercentageColors != null && campaign.PercentageColors.Count > 0)
            {
                var colors = campaign.PercentageColors.Select(i => new CampaignPercentageColor
                {
                    CampaignId = dbCampaign.Id,
                    ColorId = i.ColorId,
                    Max = i.Max,
                    Min = i.Min
                });
                db.CampaignPercentageColors.AddRange(colors);
                await db.SaveChangesAsync();
            }
            #endregion

            #region Save Penetration
            if (campaign.Penetration != null && campaign.Penetration.Count > 0)
            {
                var campaignPenetration = new Dictionary<string, List<ViewModel.ImportPenetration>>();
                foreach (var item in campaign.Penetration)
                {
                    if (!campaignPenetration.ContainsKey(item.GEOCODE))
                    {
                        campaignPenetration.Add(item.GEOCODE, new List<ViewModel.ImportPenetration>());
                    }
                    campaignPenetration[item.GEOCODE].Add(item);
                }

                var dbPenetration = new List<CampaignCRouteImported>();
                foreach (var item in campaignPenetration)
                {
                    if (!allCRoutes.ContainsKey(item.Key))
                    {
                        continue;
                    }
                    var cRoute = allCRoutes[item.Key];
                    if (cRoute.Count >= item.Value.Count)
                    {
                        for (var i = 0; i < item.Value.Count; i++)
                        {
                            dbPenetration.Add(new CampaignCRouteImported
                            {
                                CampaignId = dbCampaign.Id,
                                IsPartModified = item.Value[i].IsPartModified,
                                PartPercentage = item.Value[i].PartPercentage,
                                Penetration = item.Value[i].Penetration,
                                PremiumCRouteId = cRoute[i].Id,
                                Total = CalcTotal(campaign.AreaDescription, cRoute[i])
                            });
                        }
                    }
                    else
                    {
                        for (var i = 0; i < cRoute.Count; i++)
                        {
                            dbPenetration.Add(new CampaignCRouteImported
                            {
                                CampaignId = dbCampaign.Id,
                                IsPartModified = item.Value[i].IsPartModified,
                                PartPercentage = item.Value[i].PartPercentage,
                                Penetration = item.Value[i].Penetration,
                                PremiumCRouteId = cRoute[i].Id,
                                Total = CalcTotal(campaign.AreaDescription, cRoute[i])
                            });
                        }
                        //add other source penetration to last added one
                        var index = dbPenetration.Count - 1;
                        for (var i = cRoute.Count - 1; i < item.Value.Count; i++)
                        {
                            dbPenetration[index].Penetration += item.Value[i].Penetration;
                        }
                    }
                }
                db.CampaignCRouteImporteds.AddRange(dbPenetration);
                await db.SaveChangesAsync();
            }
            #endregion

            #region Update Total
            var submaps = dbCampaign.SubMaps.ToList();
            foreach(var item in submaps)
            {
                db.SubMaps.Attach(item);
                item.Total = await QuerySubMapTotal(dbCampaign.AreaDescription, item.Id.Value);
                db.Entry(item).State = EntityState.Modified;
            }
            await db.SaveChangesAsync();


            #endregion

            return true;
        }
        private async Task<Dictionary<int, ViewModel.ImportCRoute>> QueryCRoutesById(List<int?> allAreas)
        {
            if(allAreas == null || allAreas.Count == 0)
            {
                return new Dictionary<int, ViewModel.ImportCRoute>();
            }
            var values = new StringBuilder();
            values.AppendFormat("{0}", allAreas[0]);
            for (int i = 1; i < allAreas.Count; i++)
            {
                values.AppendFormat(", {0}", allAreas[i]);
            }

            string sql = @"
                SELECT [A].[Id],
                       [A].[GEOCODE],
                       [A].[PartCount],
                       [B].[Total]
                FROM [dbo].[premiumcroutes] AS [A]
                INNER JOIN
                  (SELECT GEOCODE,
                          COUNT(*) AS [Total]
                   FROM [dbo].[premiumcroutes]
                   GROUP BY GEOCODE) AS [B] ON [A].GEOCODE = [B].GEOCODE
                WHERE [A].[Id] IN ({0})";
            sql = string.Format(sql, values);

            var result = await db.Database.SqlQuery<ViewModel.ImportCRoute>(sql).ToListAsync();
            Dictionary<int, ViewModel.ImportCRoute> cRouteIdGeoName = new Dictionary<int, ViewModel.ImportCRoute>();
            foreach (var item in result)
            {
                cRouteIdGeoName.Add(item.Id.Value, item);
            }

            return cRouteIdGeoName;
        }
        private async Task<Dictionary<string, List<ViewModel.ImportCRoute>>> QueryCRoutesByGeoCode(List<string> allGeoCode)
        {
            var values = new StringBuilder();
            values.AppendFormat("'{0}'", allGeoCode[0]);
            for (int i = 1; i < allGeoCode.Count; i++)
            {
                values.AppendFormat(", '{0}'", allGeoCode[i]);
            }
            
            var sql = string.Format("SELECT [ID], [GEOCODE], [PartCount], [APT_COUNT], [HOME_COUNT] FROM [dbo].[premiumcroutes] WHERE [GEOCODE] IN ({0})", values);

            var result = await db.Database.SqlQuery<ViewModel.ImportCRoute>(sql).ToListAsync();
            Dictionary<string, List<ViewModel.ImportCRoute>> cRouteByGeoName = new Dictionary<string, List<ViewModel.ImportCRoute>>();
            foreach (var item in result)
            {
                if (!cRouteByGeoName.ContainsKey(item.GEOCODE))
                {
                    cRouteByGeoName.Add(item.GEOCODE, new List<ViewModel.ImportCRoute>());
                }
                cRouteByGeoName[item.GEOCODE].Add(item);
            }

            return cRouteByGeoName;
        }
        private async Task<List<ViewModel.ImportPenetration>> QueryPenetrationByCampaignId(int campaignId)
        {
            #region SQL
            var sql = @"
                SELECT [b].[GEOCODE] ,
                       [a].[Penetration] ,
                       [a].[PartPercentage] ,
                       CAST([a].[IsPartModified] as bit) AS [IsPartModified]
                FROM [dbo].[campaigncrouteimported] [a]
                INNER JOIN [dbo].[premiumcroutes] [b] ON [a].[PremiumCRouteId] = [b].[Id]
                WHERE [a].[CampaignId] = @CampaignId
                ORDER BY [b].[GEOCODE], [a].[Penetration]
                ";
            #endregion
            var result = await db.Database.SqlQuery<ViewModel.ImportPenetration>(sql, new SqlParameter("@CampaignId", campaignId)).ToListAsync();
            return result;
        }
        private bool ValidateCRouteArea(List<int> area, out List<int> addedArea, out List<Models.Location> boundary, out StringBuilder message)
        {
            boundary = new List<Models.Location>();
            addedArea = new List<int>();
            message = new StringBuilder();
            var values = new StringBuilder();
            values.AppendFormat("{0}", area[0]);
            for (int i = 1; i < area.Count; i++)
            {
                values.AppendFormat(", '{0}'", area[i]);
            }
            var sql = string.Format("SELECT * FROM [dbo].[premiumcroutecoordinates] WHERE [PreminumCRouteId] in ({0}) ORDER BY [PreminumCRouteId], [Id]", values);
            var result = db.Database.SqlQuery<Models.PremiumCRouteCoordinate>(sql);
            int? lastAddedId = null;
            List<Coordinate> crouteCoordinate = new List<Coordinate>();
            List<Polygon> croutePolygon = new List<Polygon>();
            var enumerator = result.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (lastAddedId.HasValue && lastAddedId != enumerator.Current.PreminumCRouteId)
                {
                    crouteCoordinate.Add(crouteCoordinate[0]);
                    Polygon polygon = new Polygon(new LinearRing(crouteCoordinate.ToArray()));
                    polygon.Buffer(0);
                    polygon.UserData = lastAddedId;
                    croutePolygon.Add(polygon);

                    crouteCoordinate.Clear();
                }
                crouteCoordinate.Add(new Coordinate { Y = enumerator.Current.Latitude.Value, X = enumerator.Current.Longitude.Value });
                lastAddedId = enumerator.Current.PreminumCRouteId;
            }
            //polygon must contant more than 2 points.
            if (crouteCoordinate.Count > 1)
            {
                var endLocation = crouteCoordinate[crouteCoordinate.Count - 1];
                var startLocation = crouteCoordinate[0];
                if (startLocation.X != endLocation.X || startLocation.Y != endLocation.Y)
                {
                    crouteCoordinate.Add(crouteCoordinate[0]);
                }
                try
                {

                    Polygon polygon = new Polygon(new LinearRing(crouteCoordinate.ToArray()));
                    polygon.Buffer(0);
                    polygon.UserData = lastAddedId;
                    croutePolygon.Add(polygon);
                }
                catch (Exception ex)
                {
                    message.AppendLine("load croute plygon failed.");
                    message.AppendLine(ex.ToString());
                    var debug = crouteCoordinate.Select(i => new { i.X, i.Y }).ToArray();
                    message.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(debug));
                    return false;
                }
            }

            Polygon resultPolygon = null;
            IGeometry unionPolygon = null;
            MultiPolygon cRouteCollection = null;
            try
            {
                cRouteCollection = new MultiPolygon(croutePolygon.ToArray());
                unionPolygon = cRouteCollection.Union();
            }
            catch (Exception ex)
            {
                message.AppendFormat("union failed. {0}", ex.ToString());
                message.AppendFormat("already unioned Polygon: {0}", KMLWriter.WriteGeometry(cRouteCollection, 0));
                return false;
            }
            //foreach (var polygon in croutePolygon)
            //{
            //    if (unionPolygon == null)
            //    {
            //        unionPolygon = polygon;
            //    }
            //    else
            //    {
            //        try
            //        {
            //            unionPolygon = unionPolygon.Union(polygon);
            //        }
            //        catch (Exception ex)
            //        {
                        
            //            message.AppendFormat("union failed. {0}", ex.ToString());
            //            message.AppendFormat("already unioned Polygon: {0}", NetTopologySuite.IO.KML.KMLWriter.WriteGeometry(unionPolygon, 0));
            //            message.AppendFormat("ready to union Polygon: {0}", NetTopologySuite.IO.KML.KMLWriter.WriteGeometry(polygon, 0));

            //            return false;
            //        }
            //    }
            //}


            if (unionPolygon.NumGeometries > 1)
            {
                //message = string.Format("union croute failed. this area have {0} unioned polygon", unionPolygon.NumGeometries);
                //return false;
                try
                {
                    double maxArea = 0;
                    for (var index = 0; index < unionPolygon.NumGeometries; index++)
                    {
                        var testPolygon = unionPolygon.GetGeometryN(index) as Polygon;
                        resultPolygon = testPolygon != null && testPolygon.Area > maxArea ? testPolygon : resultPolygon;
                        maxArea = resultPolygon.Area;
                    }
                }
                catch (Exception ex)
                {
                    message.AppendLine("find unioned max area polygon failed.");
                    message.AppendLine(ex.ToString());
                    return false;
                }
            }
            else if (unionPolygon.NumGeometries == 1)
            {
                resultPolygon = unionPolygon.GetGeometryN(0) as Polygon;
            }


            if (resultPolygon == null)
            {
                message.AppendFormat("union croute should be polygon, but this area is {0}", unionPolygon.GetGeometryN(0).GeometryType);
                return false;
            }
            //if(resultPolygon.Holes.Length > 0)
            //{
            //    message = "unionPolygon croute have holes";
            //    return false;
            //}
            foreach (var cRoute in croutePolygon)
            {
                if (cRoute != null && resultPolygon.Contains(cRoute))
                {
                    addedArea.Add((int)cRoute.UserData);
                }
            }
            boundary = resultPolygon.ExteriorRing.Coordinates.Select(i => new Models.Location
            {
                Latitude = i.Y,
                Longitude = i.X
            }).ToList();
            return true;

        }
        private static int CalcTotal(string areaDescription, ViewModel.ImportCRoute croute)
        {
            switch (areaDescription)
            {
                case "APT ONLY":
                    return croute.APT_COUNT ?? 0;
                case "HOME ONLY":
                    return croute.HOME_COUNT ?? 0;
                case "APT + HOME":
                    return croute.APT_COUNT ?? 0 + croute.HOME_COUNT ?? 0;
                default:
                    return croute.APT_COUNT ?? 0;
            }
        }
        private async Task<int> QuerySubMapTotal(string areaDescription, int subMapId)
        {
            string sqlAPT = "SELECT SUM([B].[APT_COUNT]) FROM [dbo].[submaprecords] [A] INNER JOIN [dbo].[premiumcroutes] [B] ON [A].AreaId = [B].Id WHERE [A].[SubMapId] = @SubMapId";
            string sqlHOME = "SELECT SUM([B].[HOME_COUNT]) FROM [dbo].[submaprecords] [A] INNER JOIN [dbo].[premiumcroutes] [B] ON [A].AreaId = [B].Id WHERE [A].[SubMapId] = @SubMapId";
            string sqlAPT_HOME = "SELECT SUM([B].[APT_COUNT] + [B].[HOME_COUNT]) FROM [dbo].[submaprecords] [A] INNER JOIN [dbo].[premiumcroutes] [B] ON [A].AreaId = [B].Id WHERE [A].[SubMapId] = @SubMapId";
            string sql = null;
            switch (areaDescription)
            {
                case "APT ONLY":
                    sql = sqlAPT;
                    break;
                case "HOME ONLY":
                    sql = sqlHOME;
                    break;
                case "APT + HOME":
                    sql = sqlAPT_HOME;
                    break;
                default:
                    sql = sqlAPT;
                    break;
            }
            var total = await db.Database.SqlQuery<int>(sql, new SqlParameter("@SubMapId", subMapId)).FirstOrDefaultAsync();
            return total;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
