using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using TIMM.GPS.Model;
using TIMM.GPS.RESTService.Helper;
using System.IO;
using log4net;


namespace TIMM.GPS.RESTService
{
    

    [ServiceContract]
    public class CampaignService
    {
        #region add by steve.j.yin on 2013/08/20 for fix calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
        private const string APT = "APT ONLY";
        private const string HOME = "HOME ONLY";
        private const string APT_HOME = "APT + HOME";
        #endregion

        private static ILog m_Logger = LogManager.GetLogger(typeof(CampaignService));
        private static readonly ILog m_SmtpLogger = LogManager.GetLogger("SmtpLogger");

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

        [WebInvoke(UriTemplate = "cmap/", Method = "POST")]
        public QueryResult<Campaign> GetCMapCampaigns(CampaignCriteria criteria)
        {
            using (var context = new TIMMContext())
            {
                QueryResult<Campaign> result = new QueryResult<Campaign>();
                var userId = UserService.CurrentUser.Id;
                var query = context.Status.Where(i => i.Status == 0).Select(i => i.Campaign).AsQueryable();
                if (!string.IsNullOrWhiteSpace(criteria.Name))
                {
                    //query = query.Where("AreaDescription = @0", criteria.Name);
                    //use filter to search result
                    query = query.Where(o => o.AreaDescription.Contains(criteria.Name));
                }
                if (!string.IsNullOrWhiteSpace(criteria.SortField))
                {
                    query = query.OrderBy(criteria.SortField);
                }
                else
                {
                    query = query.OrderByDescending(i => i.Id);
                }
                result.TotalRecord = query.Count();
                result.Result = query.Skip(criteria.PageIndex * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToList();

                FixLogoPath(result.Result);

                return result;
            }
        }

        [WebInvoke(UriTemplate = "dmap/", Method = "POST")]
        public QueryResult<Campaign> GetDMapCampaigns(CampaignCriteria criteria)
        {
            using (var context = new TIMMContext())
            {
                QueryResult<Campaign> result = new QueryResult<Campaign>();
                var userId = UserService.CurrentUser.Id;
                var query = context.Status.Where(i => i.Status == 1).Select(i => i.Campaign).AsQueryable();
                if (!string.IsNullOrWhiteSpace(criteria.Name))
                {
                    //query = query.Where("AreaDescription = @0", criteria.Name);
                    //use filter to search result
                    query = query.Where(o => o.AreaDescription.Contains(criteria.Name));
                }
                if (!string.IsNullOrWhiteSpace(criteria.SortField))
                {
                    query = query.OrderBy(criteria.SortField);
                }
                else
                {
                    query = query.OrderByDescending(i => i.Id);
                }
                result.TotalRecord = query.Count();
                result.Result = query.Skip(criteria.PageIndex * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToList();
                return result;
            }
        }

        [WebGet(UriTemplate = "/{id}")]
        public Campaign GetCampaignById(int id)
        {
            using (var context = new TIMMContext())
            {
                var result = context.Campaigns
                    .Include(i => i.CampaignPercentageColors)
                    .Include(i => i.SubMaps)
                    .Include("SubMaps.SubMapCoordinates")
                    //.Include("SubMaps.SubMapRecords")
                    .Include("SubMaps.DistributionMaps")
                    .Include("SubMaps.DistributionMaps.DistributionMapCoordinates")
                    .Include("SubMaps.DistributionMaps.DistributionMapRecords")
                    .Include("SubMaps.DistributionMaps.Tasks")
                    .Include("SubMaps.DistributionMaps.Tasks.TaskGtuInfoMappings")
                    .Include("SubMaps.DistributionMaps.Tasks.TaskGtuInfoMappings.GtuInfos")
                    .Include(i => i.Addresses)
                    .Include("Addresses.Radiuses")
                    .Where(i => i.Id == id)
                    .FirstOrDefault();


                if (result != null)
                {

                    var submapItems = LoadSubMapDetail(context, id);

                    result.SubMaps.ForEach(submap =>
                    {
                        

                        #region FiveZips
                        if (submapItems[submap.Id].ContainsKey(1))
                        {
                            var ids = submapItems[submap.Id][1].Select(i => i.AreaId).ToList();
                            #region add by steve.j.yin on 2013/08/20 for fix calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
                            var details = context.FiveZipAreas
                                .Include(i => i.Locations)
                                .Where(i => ids.Contains(i.Id))
                                .Select(i => new { 
                                    i.Id, 
                                    i.Name, 
                                    i.APT_COUNT, 
                                    i.HOME_COUNT, 
                                    APT_HOME_COUNT = i.APT_COUNT + i.HOME_COUNT, 
                                    i.Locations 
                                }).ToDictionary(i => i.Id);
                            #endregion
                            submapItems[submap.Id][1].ForEach(item =>
                            {
                                item.Name = details[item.AreaId].Name;
                                item.Locations = details[item.AreaId]
                                    .Locations
                                    .ConvertAll<Location>(i => new Location
                                    {
                                        Id = i.Id,
                                        Latitude = i.Latitude,
                                        Longitude = i.Longitude
                                    }).OrderBy(i=>i.Id).ToList();
                                if (!item.TotalHouseHold.HasValue)
                                {
                                    #region add by steve.j.yin on 2013/08/20 for fix calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
                                    //item.TotalHouseHold = details[item.AreaId].TOTAL_COUNT;
                                    switch (result.AreaDescription)
                                    {
                                        case APT:
                                            item.TotalHouseHold = details[item.AreaId].APT_COUNT;
                                            break;
                                        case HOME:
                                            item.TotalHouseHold = details[item.AreaId].HOME_COUNT;
                                            break;
                                        case APT_HOME:
                                            item.TotalHouseHold = details[item.AreaId].APT_HOME_COUNT;
                                            break;
                                        default:
                                            break;
                                    }
                                    #endregion
                                }
                                submap.FiveZipAreas.Add(item);
                            });
                        }
                        #endregion

                        #region PremiumCRoutes
                        if (submapItems[submap.Id].ContainsKey(15))
                        {
                            var ids = submapItems[submap.Id][15].Select(i => i.AreaId).ToList();
                            #region add by steve.j.yin on 2013/08/20 for fix calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
                            var details = context.PremiumCRoutes
                                .Include(i => i.Locations)
                                .Where(i => ids.Contains(i.Id))
                                .Select(i => new { 
                                    i.Id, 
                                    i.GEOCODE, 
                                    i.APT_COUNT, 
                                    i.HOME_COUNT, 
                                    APT_HOME_COUNT = i.APT_COUNT + i.HOME_COUNT, 
                                    i.Locations 
                                }).ToDictionary(i => i.Id);
                            #endregion
                            submapItems[submap.Id][15].ForEach(item =>
                            {
                                item.Name = details[item.AreaId].GEOCODE;
                                item.Locations = details[item.AreaId]
                                    .Locations
                                    .ConvertAll<Location>(i => new Location
                                    {
                                        Id = i.Id,
                                        Latitude = i.Latitude,
                                        Longitude = i.Longitude
                                    }).OrderBy(i => i.Id).ToList();
                                if (!item.TotalHouseHold.HasValue)
                                {
                                    #region add by steve.j.yin on 2013/08/20 for fix calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
                                    //item.TotalHouseHold = details[item.AreaId].TOTAL_COUNT;
                                    switch (result.AreaDescription)
                                    {
                                        case APT:
                                            item.TotalHouseHold = details[item.AreaId].APT_COUNT;
                                            break;
                                        case HOME:
                                            item.TotalHouseHold = details[item.AreaId].HOME_COUNT;
                                            break;
                                        case APT_HOME:
                                            item.TotalHouseHold = details[item.AreaId].APT_HOME_COUNT;
                                            break;
                                        default:
                                            
                                            break;
                                    }
                                    #endregion
                                }
                                submap.PremiumCRoutes.Add(item);
                            });
                        }
                        #endregion

                        #region Tracts
                        if (submapItems[submap.Id].ContainsKey(2))
                        {
                            var ids = submapItems[submap.Id][2].Select(i => i.AreaId).ToList();
                            var details = context.Tracts
                                .Include(i => i.Locations)
                                .Where(i => ids.Contains(i.Id))
                                .Select(i => new { i.Id, i.Name, i.OTotal, i.Locations })
                                .ToDictionary(i => i.Id);
                            submapItems[submap.Id][2].ForEach(item =>
                            {
                                item.Name = details[item.AreaId].Name;
                                item.Locations = details[item.AreaId]
                                    .Locations
                                    .ConvertAll<Location>(i => new Location
                                    {
                                        Id = i.Id,
                                        Latitude = i.Latitude,
                                        Longitude = i.Longitude
                                    }).OrderBy(i => i.Id).ToList();
                                if (!item.TotalHouseHold.HasValue)
                                {
                                    item.TotalHouseHold = details[item.AreaId].OTotal;
                                }
                                submap.Tracts.Add(item);
                            });
                        }
                        #endregion

                        #region BlockGroups
                        if (submapItems[submap.Id].ContainsKey(3))
                        {
                            var ids = submapItems[submap.Id][3].Select(i => i.AreaId).ToList();
                            var details = context.BlockGroups
                                .Include(i => i.Locations)
                                .Where(i => ids.Contains(i.Id))
                                .Select(i => new { i.Id, i.Name, i.OTotal, i.Locations })
                                .ToDictionary(i => i.Id);
                            submapItems[submap.Id][3].ForEach(item =>
                            {
                                item.Name = details[item.AreaId].Name;
                                item.Locations = details[item.AreaId]
                                    .Locations
                                    .ConvertAll<Location>(i => new Location
                                    {
                                        Id = i.Id,
                                        Latitude = i.Latitude,
                                        Longitude = i.Longitude
                                    }).OrderBy(i => i.Id).ToList();
                                if (!item.TotalHouseHold.HasValue)
                                {
                                    item.TotalHouseHold = details[item.AreaId].OTotal;
                                }
                                submap.BlockGroups.Add(item);
                            });
                        }
                        #endregion

                    });
                    FixLogoPath(result);
                }
                return result;
            }
        }

        [WebGet(UriTemplate = "/detail/{id}")]
        public Campaign GetCampaignByIdWithDMapGtuInfo(int id)
        {
            m_Logger.Debug("begain GetCampaignByIdWithDMapGtuInfo with id:" + id.ToString());
            using (var context = new TIMMContext())
            {
                var result = context.Campaigns
                    .Include(i => i.CampaignPercentageColors)
                    .Include(i => i.SubMaps)
                    .Include("SubMaps.SubMapCoordinates")
                    .Include(i => i.Addresses)
                    .Include("Addresses.Radiuses")
                    .Where(i => i.Id == id)
                    .FirstOrDefault();

                try
                {
                    if (result != null)
                    {
                        var submapItems = LoadSubMapDetail(context, id);
                        var dmap = LoadDistributionMaps(context, id);
                        var gtuInfo = LoadDMapGtuInfo(context, id);
                        var ndAddress = LoadDMapNdAddress(context, id); 
                        result.SubMaps.ForEach(submap =>
                        {
                            if (submapItems == null || !submapItems.ContainsKey(submap.Id))
                            {
                                return;
                            }
                            #region FiveZips
                            if (submapItems[submap.Id].ContainsKey(1))
                            {
                                var ids = submapItems[submap.Id][1].Select(i => i.AreaId).ToList();
                                #region add by steve.j.yin on 2013/08/20 for fix calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
                                var details = context.FiveZipAreas
                                    .Include(i => i.Locations)
                                    .Where(i => ids.Contains(i.Id))
                                    .Select(i => new { 
                                        i.Id, 
                                        i.Name, 
                                        i.APT_COUNT,
                                        i.HOME_COUNT,
                                        APT_HOME_COUNT = i.APT_COUNT + i.HOME_COUNT,
                                        i.Locations 
                                    }).ToDictionary(i => i.Id);
                                #endregion
                                submapItems[submap.Id][1].ForEach(item =>
                                {
                                    item.Name = details[item.AreaId].Name;
                                    item.Locations = details[item.AreaId]
                                        .Locations
                                        .ConvertAll<Location>(i => new Location
                                        {
                                            Id = i.Id,
                                            Latitude = i.Latitude,
                                            Longitude = i.Longitude
                                        }).OrderBy(i => i.Id).ToList();
                                    if (!item.TotalHouseHold.HasValue && details != null && details.ContainsKey(item.AreaId))
                                    {
                                        #region add by steve.j.yin on 2013/08/20 for fix calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
                                        //item.TotalHouseHold = details[item.AreaId].TOTAL_COUNT;
                                        switch (result.AreaDescription)
                                        {
                                            case APT:
                                                item.TotalHouseHold = details[item.AreaId].APT_COUNT;
                                                break;
                                            case HOME:
                                                item.TotalHouseHold = details[item.AreaId].HOME_COUNT;
                                                break;
                                            case APT_HOME:
                                                item.TotalHouseHold = details[item.AreaId].APT_HOME_COUNT;
                                                break;
                                            default:
                                                
                                                break;
                                        }
                                        #endregion
                                    }
                                    submap.FiveZipAreas.Add(item);
                                });
                            }
                            #endregion

                            #region PremiumCRoutes
                            if (submapItems[submap.Id].ContainsKey(15))
                            {
                                var ids = submapItems[submap.Id][15].Select(i => i.AreaId).ToList();
                                #region add by steve.j.yin on 2013/08/20 for fix calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
                                var details = context.PremiumCRoutes
                                    .Include(i => i.Locations)
                                    .Where(i => ids.Contains(i.Id))
                                    .Select(i => new { 
                                        i.Id, 
                                        i.GEOCODE, 
                                        i.APT_COUNT, 
                                        i.HOME_COUNT,
                                        APT_HOME_COUNT = i.APT_COUNT + i.HOME_COUNT,
                                        i.Locations 
                                    }).ToDictionary(i => i.Id);
                                #endregion
                                submapItems[submap.Id][15].ForEach(item =>
                                {
                                    item.Name = details[item.AreaId].GEOCODE;
                                    item.Locations = details[item.AreaId]
                                        .Locations
                                        .ConvertAll<Location>(i => new Location
                                        {
                                            Id = i.Id,
                                            Latitude = i.Latitude,
                                            Longitude = i.Longitude
                                        }).OrderBy(i => i.Id).ToList();
                                    if (!item.TotalHouseHold.HasValue && details != null && details.ContainsKey(item.AreaId))
                                    {
                                        #region add by steve.j.yin on 2013/08/20 for fix calculate Total need base campaign AreaDescription(APT ONLY/HOME ONLY/APT + HOME)
                                        //item.TotalHouseHold = details[item.AreaId].TOTAL_COUNT;
                                        switch (result.AreaDescription)
                                        {
                                            case APT:
                                                item.TotalHouseHold = details[item.AreaId].APT_COUNT;
                                                break;
                                            case HOME:
                                                item.TotalHouseHold = details[item.AreaId].HOME_COUNT;
                                                break;
                                            case APT_HOME:
                                                item.TotalHouseHold = details[item.AreaId].APT_HOME_COUNT;
                                                break;
                                            default:
                                                
                                                break;
                                        }
                                        #endregion
                                    }
                                    submap.PremiumCRoutes.Add(item);

                                });
                            }
                            #endregion

                            #region Tracts
                            if (submapItems[submap.Id].ContainsKey(2))
                            {
                                var ids = submapItems[submap.Id][2].Select(i => i.AreaId).ToList();
                                var details = context.Tracts
                                    .Include(i => i.Locations)
                                    .Where(i => ids.Contains(i.Id))
                                    .Select(i => new { i.Id, i.Name, i.OTotal, i.Locations })
                                    .ToDictionary(i => i.Id);
                                submapItems[submap.Id][2].ForEach(item =>
                                {
                                    item.Name = details[item.AreaId].Name;
                                    item.Locations = details[item.AreaId]
                                        .Locations
                                        .ConvertAll<Location>(i => new Location
                                        {
                                            Id = i.Id,
                                            Latitude = i.Latitude,
                                            Longitude = i.Longitude
                                        }).OrderBy(i => i.Id).ToList();
                                    if (!item.TotalHouseHold.HasValue && details != null && details.ContainsKey(item.AreaId))
                                    {
                                        item.TotalHouseHold = details[item.AreaId].OTotal;
                                    }
                                    submap.Tracts.Add(item);
                                });
                            }
                            #endregion

                            #region BlockGroups
                            if (submapItems[submap.Id].ContainsKey(3))
                            {
                                var ids = submapItems[submap.Id][3].Select(i => i.AreaId).ToList();
                                var details = context.BlockGroups
                                    .Include(i => i.Locations)
                                    .Where(i => ids.Contains(i.Id))
                                    .Select(i => new { i.Id, i.Name, i.OTotal, i.Locations })
                                    .ToDictionary(i => i.Id);
                                submapItems[submap.Id][3].ForEach(item =>
                                {
                                    item.Name = details[item.AreaId].Name;
                                    item.Locations = details[item.AreaId]
                                        .Locations
                                        .ConvertAll<Location>(i => new Location
                                        {
                                            Id = i.Id,
                                            Latitude = i.Latitude,
                                            Longitude = i.Longitude
                                        }).OrderBy(i => i.Id).ToList();
                                    if (!item.TotalHouseHold.HasValue && details != null && details.ContainsKey(item.AreaId))
                                    {
                                        item.TotalHouseHold = details[item.AreaId].OTotal;
                                    }
                                    submap.BlockGroups.Add(item);
                                });
                            }
                            #endregion

                            #region DMap
                            if (dmap != null && dmap.ContainsKey(submap.Id))
                            {
                                submap.DistributionMaps = dmap[submap.Id];
                            }
                            #endregion

                            #region GtuInfo and NDAddress
                            if (submap.DistributionMaps != null && submap.DistributionMaps.Count > 0)
                            {
                                submap.DistributionMaps.ForEach(item =>
                                {
                                    if (gtuInfo != null && gtuInfo.ContainsKey(item.Id))
                                    {
                                        //prepare locations by gtuId and use DouglasPeuckerReduction to merge same gtuId very close point
                                        Dictionary<int, List<Location>> temp = new Dictionary<int, List<Location>>();
                                        //List<Location> temp = new List<Location>();
                                        gtuInfo[item.Id].ForEach(t =>
                                        {
                                            if (!temp.ContainsKey(t.TaskgtuinfoId))
                                            {
                                                temp.Add(t.TaskgtuinfoId, new List<Location>());
                                            }
                                            temp[t.TaskgtuinfoId].Add(t as Location);

                                            //temp.Add(t);
                                        });

                                        List<Location> mergedGtu = new List<Location>();

                                        try
                                        {
                                            
                                            foreach (var tempItem in temp)
                                            {
                                                m_Logger.Debug("begin use DouglasPeuckerReduction: original data count is " + tempItem.Value.Count.ToString());
                                                var mergedLocations = DouglasPeuckerReduction(tempItem.Value);
                                                m_Logger.Debug("use DouglasPeuckerReduction end: after reduce data count is " + mergedLocations.Count.ToString());
                                                
                                                mergedGtu.AddRange(mergedLocations);                                                
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            m_Logger.Error("use DouglasPeuckerReduction failed", ex);
                                        }
                                        
                                        item.GtuInfo = new List<GtuInfo>();
                                        mergedGtu.ForEach(t =>
                                        {
                                            item.GtuInfo.Add(t as GtuInfo);
                                        });

                                        item.GtuInfo = item.GtuInfo.OrderBy(i => i.Id).ToList();
                                    }
                                    if (ndAddress != null && ndAddress.ContainsKey(item.Id))
                                    {
                                        item.NdAddress = ndAddress[item.Id];
                                    }
                                });
                            }
                            #endregion


                        });
                        FixLogoPath(result);
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.Error(ex);
                }
                return result;
            }
        }

        [WebGet(UriTemplate = "/withdmap/{id}")]
        public Campaign GetCamapignByIdWithDistributionMap(int id)
        {
            string sql = @"
                SELECT 
	                c.[Id],
                    c.[Name],                    
                    c.[ColorString],
                    c.[Total],
                    c.[TotalAdjustment],    
                    c.[Penetration],
                    c.[CountAdjustment],
                    d.[Latitude],
                    d.[Longitude]
                FROM campaigns a 
	                INNER JOIN submaps b ON a.Id = b.CampaignId
	                INNER JOIN [distributionmaps] c ON b.Id = c.SubMapId
	                INNER JOIN [distributionmapcoordinates] d ON c.Id = d.DistributionMapId
                WHERE a.Id = @CampaignId
                ORDER BY c.[Id], d.[Id]
            ";
            List<DistributionMap> result = new List<DistributionMap>();
            using (var context = new TIMMContext())
            {
                var campaign = context.Campaigns.FirstOrDefault(i => i.Id == id);
                if (campaign != null)
                {
                    var address = context.Addresses.Where(i => i.CampaignId == id).Include(i=>i.Radiuses).ToList();
                    campaign.Addresses = address;

                    if (context.Database.Connection.State != ConnectionState.Open)
                    {
                        context.Database.Connection.Open();
                    }
                    var dbCommand = context.Database.Connection.CreateCommand();
                    dbCommand.CommandText = sql;
                    dbCommand.Parameters.Add(new SqlParameter("@CampaignId", id));
                    var reader = dbCommand.ExecuteReader();
                    DistributionMap dmap = null;
                    while (reader.Read())
                    {
                        int dMapId = reader.GetInt32(0);
                        if (dmap == null || dmap.Id != dMapId)
                        {
                            dmap = new DistributionMap
                            {
                                Id = dMapId,
                                Name = reader.GetString(1),
                                ColorString = reader.GetString(2),
                                Total = reader.GetInt32(3),
                                TotalAdjustment = reader.GetInt32(4),
                                Penetration = reader.GetInt32(5),
                                CountAdjustment = reader.GetInt32(6),
                                Locations = new List<Location>()
                            };
                            result.Add(dmap);
                        }
                        dmap.Locations.Add(new Location
                        {
                            Latitude = reader.GetDouble(7),
                            Longitude = reader.GetDouble(8)
                        });
                    }
                    campaign.DistributionMap = result;
                }

                LoadDistributionNDAddress(ref campaign);

                return campaign;
            }
        }

        private void LoadDistributionNDAddress(ref Campaign campaign)
        {
            if (campaign == null || campaign.DistributionMap == null || campaign.DistributionMap.Count == 0)
            {
                //no distribution map
                return;
            }
            using (var context = new TIMMContext())
            {
                #region Load NdAddress
                var allNdAddress = context.NdAddresses.Include(i => i.NdAddressCoordinates).ToList();
                List<NetTopologySuite.Geometries.Polygon> allNdPolygon = new List<NetTopologySuite.Geometries.Polygon>();
                foreach (var item in allNdAddress)
                {
                    var coordinates = item.NdAddressCoordinates.OrderBy(i=>i.Id).Select(i => new GeoAPI.Geometries.Coordinate { X = i.Longitude, Y = i.Latitude }).ToArray();
                    var linearRing = new NetTopologySuite.Geometries.LinearRing(coordinates.ToArray());

                    if (!linearRing.IsClosed)
                    {
                        m_SmtpLogger.WarnFormat("NDAddress with id={0} is not closed.", item.Id);
                        if (!TryFixLinearRing(coordinates, out linearRing))
                        {
                            m_SmtpLogger.WarnFormat("Ignore this NDAddress with id={0} ", item.Id);
                            continue;
                        }
                    }

                    NetTopologySuite.Geometries.Polygon polygon = new NetTopologySuite.Geometries.Polygon(linearRing);
                    //item.NdAddressCoordinates = null;
                    polygon.UserData = item;
                    allNdPolygon.Add(polygon);
                }
                #endregion

                #region Load Distribuiton map
                foreach (var item in campaign.DistributionMap)
                {
                    var coordinates = item.Locations.OrderBy(i => i.Id).Select(i => new GeoAPI.Geometries.Coordinate { X = i.Longitude, Y = i.Latitude }).ToArray();
                    var linearRing = new NetTopologySuite.Geometries.LinearRing(coordinates.ToArray());

                    if (!linearRing.IsClosed)
                    {
                        m_SmtpLogger.WarnFormat("Distribution Map with id={0} is not closed.", item.Id);
                        if (!TryFixLinearRing(coordinates, out linearRing))
                        {
                            m_SmtpLogger.WarnFormat("Ignore this Distribution Map with id={0} ", item.Id);
                            continue;
                        }
                    }

                    NetTopologySuite.Geometries.Polygon polygon = new NetTopologySuite.Geometries.Polygon(linearRing);
                    item.NdAddress = new List<NdAddress>();
                    foreach (var ndPolygon in allNdPolygon)
                    {
                        if (!ndPolygon.Disjoint(polygon))
                        {
                            item.NdAddress.Add(ndPolygon.UserData as NdAddress);
                        }
                    }
                }
                #endregion
            }
        }

        private static bool TryFixLinearRing(GeoAPI.Geometries.Coordinate[] coordinates ,out NetTopologySuite.Geometries.LinearRing linearRing)
        {
            linearRing = null;
            m_SmtpLogger.Warn("try to fix");
            //a closed LinearRing must have more than 2 coordinates.
            if (coordinates != null && coordinates.Length > 2)
            {
                List<GeoAPI.Geometries.Coordinate> fixedLinearRing = new List<GeoAPI.Geometries.Coordinate>();
                fixedLinearRing.AddRange(coordinates);
                fixedLinearRing.Add(coordinates[0]);
                linearRing = new NetTopologySuite.Geometries.LinearRing(fixedLinearRing.ToArray());
                if (linearRing.IsClosed)
                {
                    m_SmtpLogger.Warn("fix successed!");
                    return true;
                }
                else
                {
                    m_SmtpLogger.Warn("fix failed!");
                    linearRing = null;
                    return false;
                }
            }
            return false;
        }

        #region Active DMap

        [WebGet(UriTemplate = "/activedmap/")]
        public List<DistributionMap> GetActiveDistributionMaps()
        {
            List<DistributionMap> listDMap = new List<DistributionMap>();

            #region DMap
            string strSQL = @"select a.id,a.name,a.ColorString,b.Latitude,b.Longitude from distributionmaps a
                INNER JOIN distributionmapcoordinates b ON a.Id = b.DistributionMapId
                where a.id in(
                select dmid from task where id in(
                select taskid from tasktime where TimeType <> 1 and  id in
                (select max(id) from tasktime group by taskid)))
                ORDER BY [a].[Id], [b].[Id]
            ";
            var context = new TIMMContext();
            if (context.Database.Connection.State != ConnectionState.Open)
            {
                context.Database.Connection.Open();
            }
            var dbCommand = context.Database.Connection.CreateCommand();
            dbCommand.CommandText = strSQL;
            var reader = dbCommand.ExecuteReader();
            DistributionMap dmap = null;
            while (reader.Read())
            {
                int dMapId = reader.GetInt32(0);
                if (dmap == null || dmap.Id != dMapId)
                {
                    dmap = new DistributionMap
                    {
                        Id = dMapId,
                        Name = reader.GetString(1),
                        ColorString = reader.GetString(2),
                        Locations = new List<Location>()
                    };
                    listDMap.Add(dmap);
                }
                dmap.Locations.Add(new Location
                {
                    Latitude = reader.GetDouble(3),
                    Longitude = reader.GetDouble(4)
                });
            }
            reader.Close();
            #endregion

            var task = LoadActiveDMapTask(context);
            var gtuInfo = LoadActiveDMapGtuInfo(context);
            var ndAddress = LoadActiveDMapNdAddress(context);
            var campaignAddress = LoadActivedCampaignAddress(context);
            #region GtuInfo and NDAddress
            if (listDMap != null && listDMap.Count > 0)
            {
                listDMap.ForEach(item =>
                {
                    if (task.ContainsKey(item.Id))
                    {
                        item.Tasks = task[item.Id];
                    }
                    if (gtuInfo.ContainsKey(item.Id))
                    {
                        item.GtuInfo = gtuInfo[item.Id];
                    }
                    if (ndAddress.ContainsKey(item.Id))
                    {
                        item.NdAddress = ndAddress[item.Id];
                    }
                    if (campaignAddress.ContainsKey(item.Id))
                    {
                        item.SubMap = new SubMap();
                        item.SubMap.Campaign = new Campaign();
                        item.SubMap.Campaign.Addresses = new List<Address>();
                        item.SubMap.Campaign.Addresses.AddRange(campaignAddress[item.Id]);
                    }
                });
            }
            #endregion

            return listDMap;
        }

        private Dictionary<int, List<Task>> LoadActiveDMapTask(TIMMContext context)
        {
            string sql = @"
                select dmid,auditorid,email,Name,Date from task where dmid in(
                select dmid from task where id in(
                select taskid from tasktime where TimeType <> 1 and  id in
                (select max(id) from tasktime group by taskid)))
            ";
            Dictionary<int, List<Task>> result = new Dictionary<int, List<Task>>();
            try
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var dbCommand = context.Database.Connection.CreateCommand();
                dbCommand.CommandText = sql;
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    int dMapId = reader.GetInt32(0);
                    if (!result.ContainsKey(dMapId))
                    {
                        result.Add(dMapId, new List<Task>());
                    }
                    result[dMapId].Add(new Task
                    {
                        DistributionMapId = reader.GetInt32(0),
                        AuditorId = reader.GetInt32(1),
                        Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        Name = reader.GetString(3),
                        Date = reader.GetDateTime(4)
                    });
                }
                reader.Close();
            }
            finally
            {
                context.Database.Connection.Close();
            }
            return result;

        }

        private Dictionary<int, List<GtuInfo>> LoadActiveDMapGtuInfo(TIMMContext context)
        {
            string sql = @"
                SELECT
	                c.Id AS DMapId,
	                e.UserColor,
	                f.dwLatitude AS Latitude,
	                f.dwLongitude AS Longitude,
                    f.nCellID AS nCellID
                FROM distributionmaps c
                    INNER JOIN task d ON d.DmId = c.Id
                    INNER JOIN taskgtuinfomapping e ON d.Id = e.TaskId
                    INNER JOIN gtuinfo f ON e.id = f.TaskgtuinfoId
                WHERE c.Id in (select dmid from task where id in(
                select taskid from tasktime where TimeType <> 1 and  id in
                (select max(id) from tasktime group by taskid)))
                    AND d.Status = 0
            ";
            Dictionary<int, List<GtuInfo>> result = new Dictionary<int, List<GtuInfo>>();
            try
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var dbCommand = context.Database.Connection.CreateCommand();
                dbCommand.CommandText = sql;
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    int dMapId = reader.GetInt32(0);
                    if (!result.ContainsKey(dMapId))
                    {
                        result.Add(dMapId, new List<GtuInfo>());
                    }
                    result[dMapId].Add(new GtuInfo
                    {
                        UserColor = reader.GetString(1),
                        Latitude = reader.GetDouble(2),
                        Longitude = reader.GetDouble(3),
                        nCellID = reader.GetInt32(4)
                    });
                }
                reader.Close();
            }
            finally
            {
                context.Database.Connection.Close();
            }
            return result;

        }

        private Dictionary<int, List<NdAddress>> LoadActiveDMapNdAddress(TIMMContext context)
        {
            string sql = @"
                SELECT
                    c.Id as DMapId,
	                d.Id,
                    d.Street,
	                d.ZipCode,
	                d.Description,
	                d.polygon.ToString()
                FROM distributionmaps c
	                INNER JOIN ndaddresses d ON d.polygon.STIntersects(c.polygon) = 1
                WHERE c.Id in (select dmid from task where id in(
                select taskid from tasktime where TimeType <> 1 and  id in
                (select max(id) from tasktime group by taskid)))
                union (               
                    SELECT
                    c.Id as DMapId,
                    d.Id,
                    d.Name As Street,
                    '00000' as ZipCode,
                    d.Description,
                    d.polygon.ToString()
                    FROM distributionmaps c
                    INNER JOIN customareas d ON d.polygon.STIntersects(c.polygon) = 1
                    WHERE c.Id in (select dmid from task where id in(
                select taskid from tasktime where TimeType <> 1 and  id in
                (select max(id) from tasktime group by taskid)))
                    )
            ";
            Dictionary<int, List<NdAddress>> result = new Dictionary<int, List<NdAddress>>();
            try
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var dbCommand = context.Database.Connection.CreateCommand();
                dbCommand.CommandText = sql;
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    int dMapId = reader.GetInt32(0);
                    if (!result.ContainsKey(dMapId))
                    {
                        result.Add(dMapId, new List<NdAddress>());
                    }
                    result[dMapId].Add(new NdAddress
                    {
                        Id = reader.GetInt32(1),
                        Street = reader.GetString(2),
                        ZipCode = reader.GetString(3),
                        Description = reader.GetString(4),
                        Locations = PolygonStrToLocations(reader.GetString(5))
                    });
                }
                reader.Close();
            }
            finally
            {
                context.Database.Connection.Close();
            }
            return result;

        }

        private Dictionary<int, List<Address>> LoadActivedCampaignAddress(TIMMContext context)
        {
            string sql = @"
                SELECT [c].Id AS [DmapId] 
	                  ,[a].[Id]
                      ,[a].[Address]
                      ,[a].[ZipCode]
                      ,[a].[OriginalLatitude]
                      ,[a].[OriginalLongitude]
                      ,[a].[Longitude]
                      ,[a].[Latitude]
                      ,[a].[Color]
                      ,[a].[CampaignId]
                      ,[a].[Picture]
                  FROM [Addresses] AS [a]
            INNER JOIN [submaps] AS [b] ON [b].[CampaignId] = [a].[CampaignId]
            INNER JOIN [distributionmaps] AS [c] ON [c].[SubMapId] = [b].[Id]
                 WHERE [c].[Id] IN
                (
	                SELECT [DmId] FROM [task] WHERE [Id] IN
	                (
		                SELECT [TaskId]
		                  FROM [tasktime] 
		                 WHERE [TimeType] <> 1 
		                  AND  [Id] IN (SELECT MAX([Id]) FROM [tasktime] GROUP BY [TaskId])
	                )
                )
            ";

            Dictionary<int, List<Address>> result = new Dictionary<int, List<Address>>();
            try
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var dbCommand = context.Database.Connection.CreateCommand();
                dbCommand.CommandText = sql;
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    int dMapId = reader.GetInt32(0);
                    if (!result.ContainsKey(dMapId))
                    {
                        result.Add(dMapId, new List<Address>());
                    }
                    result[dMapId].Add(new Address
                    {
                        Id = reader.GetInt32(1),
                        AddressName = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ZipCode = reader.IsDBNull(3) ? null : reader.GetString(3),
                        OriginalLatitude = reader.IsDBNull(4) ? 0 : reader.GetFloat(4),
                        OriginalLongitude = reader.IsDBNull(5) ? 0 : reader.GetFloat(5),
                        Longitude = reader.IsDBNull(6) ? 0 : reader.GetFloat(6),
                        Latitude = reader.IsDBNull(7) ? 0 : reader.GetFloat(7),
                        Color = reader.IsDBNull(8) ? null : reader.GetString(8),
                        CampaignId = reader.GetInt32(9),
                        Picture = reader.IsDBNull(10) ? null : reader.GetString(10)
                    });
                }
                reader.Close();
            }
            finally
            {
                context.Database.Connection.Close();
            }
            return result;
        }
        #endregion

        #region SendMail

        [WebGet(UriTemplate = "/sendmail/{did}")]
        [Obsolete]
        public void SendMailWithSMTP(int did)
        {
            //string content2 = @"silver.jing@maesinfo.com#####420421227@qq.com#####Mytest#####See Attach#####C:\1.jpg";
            //content = content2;
            string to = null, cc = null, subject = null, context = null, attach = null;
            //string[] strArr = content.Split(new string[] { "#####" }, StringSplitOptions.None);
            //to = strArr[0];
            //cc = strArr[1];
            //subject = strArr[2];
            //context = strArr[3];
            //attach = strArr[4];
            using (var tcontext = new TIMMContext())
            {
                to = tcontext.Tasks.Where(t => t.DistributionMapId == did).FirstOrDefault().Email;
            }
            cc = "wayne.wei@maesinfo.com";
            subject = "Active dmap";
            context = "See the attachments.";
            attach = @"C:/DMap/" + did.ToString() + ".png";

            System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

            //发送地址
            if (to != null && to != "")
            {
                mmsg.To.Add(to);
            }
            //抄送地址
            if (cc != null && cc != "")
            {
                mmsg.CC.Add(cc);
            }
            //发件人信息
            mmsg.From = new System.Net.Mail.MailAddress("wayne.wei@maesinfo.com", "Wayne", System.Text.Encoding.UTF8);
            //标题
            mmsg.Subject = subject;
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;
            //内容
            mmsg.Body = context;
            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            mmsg.IsBodyHtml = false;
            //附件
            if (attach != null && attach != "")
            {
                System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(attach);
                mmsg.Attachments.Add(data);
            }
            //优先级
            mmsg.Priority = System.Net.Mail.MailPriority.High;
            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();

            smtpClient.Credentials = new System.Net.NetworkCredential("wayne.wei@maesinfo.com",
                "Mi123456");
            smtpClient.EnableSsl = true;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;

            object userState = new Tuple<System.Net.Mail.MailMessage, string>(mmsg, attach);

            smtpClient.SendCompleted += (s, e) =>
                {
                    var asncTuple = (e.UserState as Tuple<System.Net.Mail.MailMessage, string>);

                    asncTuple.Item1.Dispose();

                    if (e.Error == null)
                    {
                        if (File.Exists(asncTuple.Item2))
                        {
                            File.Delete(asncTuple.Item2);
                        }
                    }
                };

            smtpClient.SendAsync(mmsg, userState);
        }

        #endregion

        private Dictionary<int, Dictionary<int, List<SubMapDetailItem>>> LoadSubMapDetail(TIMMContext context, int campaignId)
        {
            #region sql
            string sql = @"
                SELECT 
	                SubMapId,
	                AreaId,
	                Classification,
	                CASE 
		                WHEN Classification = 1 THEN Total1
		                WHEN Classification = 2 THEN Total2
		                WHEN Classification = 3 THEN Total3
		                WHEN Classification = 15 THEN Total15
	                END AS Total,
	                CASE 
		                WHEN Classification = 1 THEN Penetration1
		                WHEN Classification = 2 THEN Penetration2
		                WHEN Classification = 3 THEN Penetration3
		                WHEN Classification = 15 THEN Penetration15
	                END AS Penetration
                FROM(
	                SELECT 
		                a.SubMapId,
		                a.AreaId,
		                a.Classification, 
		                b.Total AS Total1,
		                b.Penetration AS Penetration1, 
		                c.Total AS Total2, 
		                c.Penetration AS Penetration2, 
		                d.Total AS Total3, 
		                d.Penetration AS Penetration3, 
		                case when e.IsPartModified=1 then e.Total * e.PartPercentage else e.Total END AS Total15, 
		                case when e.IsPartModified=1 then e.Penetration * e.PartPercentage else e.Penetration END AS Penetration15
	                FROM submaprecords a 
	                LEFT JOIN campaignfivezipimported b  ON b.CampaignId = @CampaignId and a.AreaId = b.FiveZipAreaId and a.Classification = 1
	                LEFT JOIN campaigntractimported c ON c.CampaignId = @CampaignId and a.AreaId = c.TractId and a.Classification = 2
	                LEFT JOIN campaignblockgroupimported d ON d.CampaignId = @CampaignId and a.AreaId = d.BlockGroupId and a.Classification = 3
	                LEFT JOIN campaigncrouteimported e ON e.CampaignId = @CampaignId and a.AreaId = e.PremiumCRouteId and a.Classification = 15
	                WHERE SubMapId IN (SELECT Id FROM submaps WHERE CampaignId = @CampaignId) 
                        AND a.Value = 1
                ) AS T
                ORDER BY SubMapId, Classification, AreaId
                ";
            #endregion

            var result = new Dictionary<int, Dictionary<int, List<SubMapDetailItem>>>();
            try
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var dbCommand = context.Database.Connection.CreateCommand();
                dbCommand.CommandText = sql;
                dbCommand.Parameters.Add(new SqlParameter("@CampaignId", campaignId));
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    var submapId = reader.GetInt32(0);
                    var areaId = reader.GetInt32(1);
                    var classification = reader.GetInt32(2);
                    if (!result.ContainsKey(submapId))
                    {
                        result.Add(submapId, new Dictionary<int, List<SubMapDetailItem>>());
                    }
                    if (!result[submapId].ContainsKey(classification))
                    {
                        result[submapId].Add(classification, new List<SubMapDetailItem>());
                    }
                    float fCTotal = reader.IsDBNull(3) ? 0 : reader.GetFloat(3);
                    double dCTotal = Math.Round((double)fCTotal, 0);
                    int iCTotal = (int)dCTotal;

                    float fTarget = reader.IsDBNull(4) ? 0 : reader.GetFloat(4);
                    double dTarget = Math.Round((double)fTarget, 0);
                    int iTarget = (int)dTarget;
                    result[submapId][classification].Add(new SubMapDetailItem
                    {
                        AreaId = areaId,
                        Classification = classification,
                        TotalHouseHold = reader.IsDBNull(3) ? null : new Nullable<int>(iCTotal),
                        TargetHouseHold = reader.IsDBNull(4) ? null : new Nullable<int>(iTarget)
                    });
                }
            }
            finally
            {
                context.Database.Connection.Close();
            }
            return result;
        }

        private Dictionary<int, List<GtuInfo>> LoadSubMapGtuInfo(TIMMContext context, int campaignId)
        {
            string sql = @"
                SELECT
	                b.Id AS SubMapId,
	                d.Id AS DMapId,
	                e.UserColor,
	                f.dwLatitude AS Latitude,
	                f.dwLongitude AS Longitude
                FROM Campaigns a 
                    INNER JOIN submaps b ON a.Id = b.CampaignId
                    INNER JOIN distributionmaps c ON c.SubMapId = b.Id 
                    INNER JOIN task d ON d.DmId = c.Id
                    INNER JOIN taskgtuinfomapping e ON d.Id = e.TaskId
                    INNER JOIN gtuinfo f ON e.GTUId = f.TaskgtuinfoId
                WHERE a.Id = @CampaignId AND d.Status = 1 
            ";

            Dictionary<int, List<GtuInfo>> result = new Dictionary<int, List<GtuInfo>>();
            try
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var dbCommand = context.Database.Connection.CreateCommand();
                dbCommand.CommandText = sql;
                dbCommand.Parameters.Add(new SqlParameter("@CampaignId", campaignId));
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    int subMapId = reader.GetInt32(0);
                    if (!result.ContainsKey(subMapId))
                    {
                        result.Add(subMapId, new List<GtuInfo>());
                    }
                    result[subMapId].Add(new GtuInfo
                    {
                        UserColor = reader.GetString(2),
                        Latitude = reader.GetDouble(3),
                        Longitude = reader.GetDouble(4)
                    });
                }
            }
            finally
            {
                context.Database.Connection.Close();
            }
            return result;

        }

        private Dictionary<int, List<DistributionMap>> LoadDistributionMaps(TIMMContext context, int campaignId)
        {
            string sql = @"
                SELECT 
                    c.[SubMapId],
	                c.[Id] As DistributionId,
                    c.[Name],                    
                    c.[ColorString],
                    c.[Total],
                    c.[Penetration],
                    c.[Percentage],
                    c.[TotalAdjustment],
                    c.[CountAdjustment],
                    d.[Latitude],
                    d.[Longitude]
                FROM campaigns a 
	                INNER JOIN submaps b ON a.Id = b.CampaignId
	                INNER JOIN [distributionmaps] c ON b.Id = c.SubMapId
	                INNER JOIN [distributionmapcoordinates] d ON c.Id = d.DistributionMapId
                WHERE a.Id = @CampaignId
                ORDER BY b.[Id], c.[Id], d.[Id]
            ";
            var result = new Dictionary<int, List<DistributionMap>>();
            try
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var dbCommand = context.Database.Connection.CreateCommand();
                dbCommand.CommandText = sql;
                dbCommand.Parameters.Add(new SqlParameter("@CampaignId", campaignId));
                var reader = dbCommand.ExecuteReader();
                Dictionary<int, DistributionMap> dMaps = new Dictionary<int, DistributionMap>();
                while (reader.Read())
                {
                    int subMapId = reader.GetInt32(0);
                    int dMapId = reader.GetInt32(1);

                    if (!dMaps.ContainsKey(dMapId))
                    {
                        var dmap = new DistributionMap
                        {
                            Id = dMapId,
                            SubMapId = subMapId,
                            Name = reader.GetString(2),
                            ColorString = reader.GetString(3),
                            Total = reader.GetInt32(4),
                            Penetration = reader.GetInt32(5),
                            Percentage = reader.GetDouble(6),
                            TotalAdjustment = reader.GetInt32(7),
                            CountAdjustment = reader.GetInt32(8),
                            Locations = new List<Location>()
                        };
                        dMaps.Add(dMapId, dmap);
                        if (!result.ContainsKey(subMapId))
                        {
                            result.Add(subMapId, new List<DistributionMap>());
                        }
                        result[subMapId].Add(dmap);
                    }
                    dMaps[dMapId].Locations.Add(new Location
                    {
                        Latitude = reader.GetDouble(9),
                        Longitude = reader.GetDouble(10)
                    });
                }
            }
            finally
            {
                context.Database.Connection.Close();
            }
            return result;

        }

        private Dictionary<int, List<GtuInfo>> LoadDMapGtuInfo(TIMMContext context, int campaignId)
        {
            //the previous team got the lat / long confused so we have been confused everysince
            string sql = @"
                SELECT
	                c.Id AS DMapId,
	                e.UserColor,
                    f.[Id],
	                f.dwLatitude AS Latitude,
	                f.dwLongitude AS Longitude,
                    f.nCellID AS nCellID
                FROM Campaigns a 
                    INNER JOIN submaps b ON a.Id = b.CampaignId
                    INNER JOIN distributionmaps c ON c.SubMapId = b.Id 
                    INNER JOIN task d ON d.DmId = c.Id
                    INNER JOIN taskgtuinfomapping e ON d.Id = e.TaskId
                    INNER JOIN gtuinfo f ON e.[Id] = f.[TaskgtuinfoId]
                WHERE a.Id = @CampaignId 
                    AND d.Status = 1 
                ORDER BY [c].[Id], e.[Id], f.[Id]
            ";
            Dictionary<int, List<GtuInfo>> result = new Dictionary<int, List<GtuInfo>>();
            try
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var dbCommand = context.Database.Connection.CreateCommand();
                dbCommand.CommandText = sql;
                dbCommand.Parameters.Add(new SqlParameter("@CampaignId", campaignId));
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    int dMapId = reader.GetInt32(0);
                    if (!result.ContainsKey(dMapId))
                    {
                        result.Add(dMapId, new List<GtuInfo>());
                    }
                    result[dMapId].Add(new GtuInfo
                    {                        
                        UserColor = reader.GetString(1),
                        Id = reader.GetInt64(2),
                        Latitude = reader.GetDouble(3),
                        Longitude = reader.GetDouble(4),
                        nCellID = reader.GetInt32(5)
                    });
                }
            }
            finally
            {
                context.Database.Connection.Close();
            }
            return result;

        }

        private Dictionary<int, List<NdAddress>> LoadDMapNdAddress(TIMMContext context, int campaignId)
        {
            #region sql
            string sql = @"
                SELECT  c.Id AS DMapId ,
                        d.Id ,
                        d.Street ,
                        d.ZipCode ,
                        d.Description ,
                        d.polygon.ToString()
                FROM    dbo.campaigns a
                        INNER JOIN submaps b ON a.Id = b.CampaignId
                        INNER JOIN distributionmaps c ON b.Id = c.SubMapId
                        INNER JOIN ndaddresses d ON d.polygon.STIntersects(c.polygon) = 1
                WHERE   a.Id = @CampaignId
                UNION
                ( SELECT    c.Id AS DMapId ,
                            d.Id ,
                            d.Name AS Street ,
                            '00000' AS ZipCode ,
                            d.Description ,
                            d.polygon.ToString()
                  FROM      dbo.campaigns a
                            INNER JOIN submaps b ON a.Id = b.CampaignId
                            INNER JOIN distributionmaps c ON b.Id = c.SubMapId
                            INNER JOIN customareas d ON d.polygon.STIntersects(c.polygon) = 1
                  WHERE     a.Id = @CampaignId
                )
            ";
            #endregion

            Dictionary<int, List<NdAddress>> result = new Dictionary<int, List<NdAddress>>();
            try
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var dbCommand = context.Database.Connection.CreateCommand();
                dbCommand.CommandText = sql;
                dbCommand.CommandTimeout = 0;//The default is 30 seconds.A value of 0 indicates no limit 
                dbCommand.Parameters.Add(new SqlParameter("@CampaignId", campaignId));
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    int dMapId = reader.GetInt32(0);
                    if (!result.ContainsKey(dMapId))
                    {
                        result.Add(dMapId, new List<NdAddress>());
                    }
                    result[dMapId].Add(new NdAddress
                    {
                        Id = reader.GetInt32(1),
                        Street = reader.GetString(2),
                        ZipCode = reader.GetString(3),
                        Description = reader.GetString(4),
                        Locations = PolygonStrToLocations(reader.GetString(5))
                    });
                }
            }
            finally
            {
                context.Database.Connection.Close();
            }
            return result;
        }

        /// <summary>
        /// !Warnning!not tested
        /// performance improvement for LoadDMapNdAddress() 
        /// </summary>
        /// <param name="dmaps"></param>
        /// <returns></returns>
        private Dictionary<int, List<NdAddress>> LoadDMapNdAddress(List<DistributionMap> dmaps)
        {
            Dictionary<int, List<NdAddress>> result = new Dictionary<int, List<NdAddress>>();

            #region prepare all ndaddress polygon
            var allNdAddress = NdAddressService.GetAllNdAddressAndCustomArea();
            List<NetTopologySuite.Geometries.Polygon> allNdPolygon = new List<NetTopologySuite.Geometries.Polygon>();
            foreach(var ndAddress in allNdAddress)
            {
                var coordinates = ndAddress.Locations.OrderBy(i=>i.Id).Select(i => new GeoAPI.Geometries.Coordinate { X = i.Longitude, Y = i.Latitude }).ToArray();
                var linearRing = new NetTopologySuite.Geometries.LinearRing(coordinates.ToArray());

                if (!linearRing.IsClosed)
                {
                    m_SmtpLogger.WarnFormat("NdAddress with id={0} is not closed.", ndAddress.Id);
                    if (!TryFixLinearRing(coordinates, out linearRing))
                    {
                        m_SmtpLogger.WarnFormat("Ignore this NdAddress with id={0} ", ndAddress.Id);
                        continue;
                    }
                }
                NetTopologySuite.Geometries.Polygon polygon = new NetTopologySuite.Geometries.Polygon(linearRing);

                ndAddress.Locations.Clear();
                polygon.UserData = ndAddress;
                allNdPolygon.Add(polygon);
            }
            #endregion

            #region prepare dmap polygon
            var allDMapPolygon = new List<NetTopologySuite.Geometries.Polygon>();
            foreach (var distributionMap in dmaps)
            {
                var coordinates = distributionMap.Locations.OrderBy(i => i.Id).Select(i => new GeoAPI.Geometries.Coordinate { X = i.Longitude, Y = i.Latitude }).ToArray();
                var linearRing = new NetTopologySuite.Geometries.LinearRing(coordinates.ToArray());

                if (!linearRing.IsClosed)
                {
                    m_SmtpLogger.WarnFormat("Distribution Map with id={0} is not closed.", distributionMap.Id);
                    if (!TryFixLinearRing(coordinates, out linearRing))
                    {
                        m_SmtpLogger.WarnFormat("Ignore this Distribution Map with id={0} ", distributionMap.Id);
                        continue;
                    }
                }
                NetTopologySuite.Geometries.Polygon polygon = new NetTopologySuite.Geometries.Polygon(
                    new NetTopologySuite.Geometries.LinearRing(coordinates.ToArray())
                );
                polygon.UserData = distributionMap.Id;
                allDMapPolygon.Add(polygon);
            }
            #endregion
            foreach (var dmapPolygon in allDMapPolygon)
            {
                foreach (var ndPolygon in allNdPolygon)
                {
                    if (ndPolygon.Intersects(ndPolygon))
                    {
                        int dmapId = (int)dmapPolygon.UserData;
                        if (!result.ContainsKey(dmapId))
                        {
                            result.Add(dmapId, new List<NdAddress>());
                        }
                        result[dmapId].Add(ndPolygon.UserData as NdAddress);
                    }
                }
            }

            return result;
        }

        [WebInvoke(UriTemplate = "modify/", Method = "POST")]
        public Campaign CreateCampaign(Campaign entity)
        {
            using (var context = new TIMMContext())
            {

                Campaign dbCampaign = null;
                if (entity.Id > 0)
                {
                    //update
                    dbCampaign = context.Campaigns.First(i => i.Id == entity.Id);

                }
                else
                {
                    dbCampaign = new Campaign();
                    //create

                    string where = "UserName = @0 AND ClientCode = @1 AND AreaDescription = @2 AND Date >= @3 AND Date < @4";
                    var max = context.Campaigns
                        .Where(where, entity.UserName, entity.ClientCode, entity.AreaDescription, entity.Date.Date, entity.Date.Date.AddDays(1))
                        .Select(i => i.Sequence)
                        .Cast<int?>()
                        .Max();
                    dbCampaign.Sequence = max ?? 0 + 1;
                    dbCampaign.Name = GenerateCampaignName(entity);
                    dbCampaign.Description = string.Empty;
                    dbCampaign.CustemerName = string.Empty;
                    dbCampaign.Status = new List<StatusInfo>();
                    dbCampaign.CreatorName = entity.UserName;
                    var status = new StatusInfo
                    {
                        Campaign = dbCampaign,
                        Status = 0,
                        UserId = UserService.CurrentUserId
                    };
                    dbCampaign.Status.Add(status);
                    context.Campaigns.Add(dbCampaign);
                }

                dbCampaign.AreaDescription = entity.AreaDescription;
                dbCampaign.ClientCode = entity.ClientCode;
                dbCampaign.ClientName = entity.ClientName;
                dbCampaign.ContactName = entity.ContactName;
                //old.CustemerName = entity.CustemerName;                
                //old.Description = entity.Description;
                dbCampaign.Date = entity.Date;
                dbCampaign.Logo = entity.Logo;
                dbCampaign.UserName = entity.UserName;

                context.SaveChanges();
                return dbCampaign;
            }
        }

        [WebGet(UriTemplate = "copy/{id}")]
        public Campaign CopyCampaign(int id)
        {
            using (var context = new TIMMContext())
            {
                var source = context.Campaigns.FirstOrDefault(i => i.Id == id);
                if (source == null)
                {
                    throw new BusinessException("source campaign void!");
                }
                var target = source.DeepClone();
                context.Campaigns.Add(target);
                context.SaveChanges();
                return target;
            }
        }

        [WebGet(UriTemplate = "delete/{id}")]
        public void DeleteCampaign(int id)
        {
            using (var context = new TIMMContext())
            {
                var source = context.Campaigns.FirstOrDefault(i => i.Id == id);
                if (source == null)
                {
                    throw new BusinessException("campaign not exist!");
                }
                var backup = source.Transfer<CampaignBackup>();
                backup.IPAddress = HttpContext.Current.Request.UserHostAddress;
                backup.OperationTime = DateTime.Now;
                backup.OperationUser = UserService.CurrentUser.UserName;
                context.Campaigns.Remove(source);
                context.CampaignBackups.Add(backup);

                context.SaveChanges();
            }
        }

        [WebGet(UriTemplate = "publish/{id}/username")]
        public void PublishCampaign(int id, string userName)
        {
        }

        private string GenerateCampaignName(Campaign entity)
        {
            return string.Format("{0}-{1}-{2}-{3}",
            entity.Date.ToString("MMddyy"),
            entity.ClientCode,
            entity.AreaDescription,
            entity.Sequence);
        }

        private List<Location> PolygonStrToLocations(string polygon)
        {
            List<Location> result = new List<Location>();


            if (!string.IsNullOrWhiteSpace(polygon) && polygon.Length > 12)
            {
                polygon = polygon.Substring(10, polygon.Length - 12);
                var points = polygon.Split(',');
                foreach (var point in points)
                {
                    var tmp = point.Trim();
                    var coordinate = tmp.Split(' ');
                    double longitude, latitude;
                    double.TryParse(coordinate[0], out longitude);
                    double.TryParse(coordinate[1], out latitude);
                    result.Add(new Location { Longitude = longitude, Latitude = latitude });
                }
            }

            return result;
        }

        #region Ramer–Douglas–Peucker Algorithm
        /// <summary>
        /// Uses the Douglas Peucker algorithm to reduce the number of points.
        /// </summary>
        /// <param name="Points">The points.</param>
        /// <param name="Tolerance">The tolerance.</param>
        /// <returns></returns>
        public static List<Location> DouglasPeuckerReduction(List<Location> Points)
        {
            var toleranceSetting = ConfigurationManager.AppSettings["GTUTolerance"];
            double tolerance;
            if (double.TryParse(toleranceSetting, out tolerance) && tolerance > 0)
            {
                tolerance = tolerance / 1000000d;
                if (Points == null || Points.Count < 3)
                    return Points;

                Int32 firstPoint = 0;
                Int32 lastPoint = Points.Count - 1;
                List<Int32> pointIndexsToKeep = new List<Int32>();

                //Add the first and last index to the keepers
                pointIndexsToKeep.Add(firstPoint);
                pointIndexsToKeep.Add(lastPoint);

                //The first and the last point cannot be the same
                while (Points[firstPoint].Equals(Points[lastPoint]))
                {
                    lastPoint--;
                }

                DouglasPeuckerReduction(Points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);

                List<Location> returnPoints = new List<Location>();
                pointIndexsToKeep.Sort();
                foreach (Int32 index in pointIndexsToKeep)
                {
                    returnPoints.Add(Points[index]);
                }

                return returnPoints;
            }
            else
            {
                return Points;
            }
            
        }

        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="pointIndexsToKeep">The point index to keep.</param>
        private static void DouglasPeuckerReduction(List<Location> points, Int32 firstPoint, Int32 lastPoint, Double tolerance, ref List<Int32> pointIndexsToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                double distance = PerpendicularDistance
                    (points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint,
                indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest,
                lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <summary>
        /// The distance of a point from a line made from point1 and point2.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static Double PerpendicularDistance(Location Point1, Location Point2, Location Point)
        {
            //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
            //Base = v((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
            //Area = .5*Base*H                                          *Solve for height
            //Height = Area/.5/Base

            double area = Math.Abs(.5 * (Point1.Longitude * Point2.Latitude + Point2.Longitude * Point.Latitude + Point.Longitude * Point1.Latitude - Point2.Longitude * Point1.Latitude - Point.Longitude * Point2.Latitude - Point1.Longitude * Point.Latitude));
            double bottom = Math.Sqrt(Math.Pow(Point1.Longitude - Point2.Longitude, 2) + Math.Pow(Point1.Latitude - Point2.Latitude, 2));
            double height = area / bottom * 2.0;

            return height;

            //Another option
            //Double A = Point.X - Point1.X;
            //Double B = Point.Y - Point1.Y;
            //Double C = Point2.X - Point1.X;
            //Double D = Point2.Y - Point1.Y;

            //Double dot = A * C + B * D;
            //Double len_sq = C * C + D * D;
            //Double param = dot / len_sq;

            //Double xx, yy;

            //if (param < 0)
            //{
            //    xx = Point1.X;
            //    yy = Point1.Y;
            //}
            //else if (param > 1)
            //{
            //    xx = Point2.X;
            //    yy = Point2.Y;
            //}
            //else
            //{
            //    xx = Point1.X + param * C;
            //    yy = Point1.Y + param * D;
            //}

            //Double d = DistanceBetweenOn2DPlane(Point, new Point(xx, yy));
        }
        #endregion
    }
}
