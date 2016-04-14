using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vargainc.Timm.EF;
using Vargainc.Timm.Models;
using Wintellect.PowerCollections;
using Vargainc.Timm.REST.Helper;
using System.Threading.Tasks;
using Vargainc.Timm.Extentions;

namespace Vargainc.Timm.REST.Controllers
{
    /// <summary>
    /// Lat = Y Long = X
    /// </summary>
    [RoutePrefix("print")]
    public class PrintController : ApiController
    {
        private TimmContext db = new TimmContext();

        [HttpGet]
        [Route("ndaddress")]
        public IHttpActionResult GetNDAddress()
        {
            var query = from nd in db.NdAddresses
                        select new ViewModel.Location
                        {
                            Latitude = nd.Latitude,
                            Longitude = nd.Longitude
                        };

            var result = query.ToArray();
            return Json(new
            {
                nda = result
            });
        }

        #region Campaign
        /// <summary>
        /// this method only return the base info. the submaps the dmaps without locations. 
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("campaign/{campaignId:int}")]
        public async Task<IHttpActionResult> GetCampaignForPrint(int campaignId)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId);
            if(campaign == null)
            {
                return NotFound();
            }

            var submaps = campaign.SubMaps.OrderBy(i => i.OrderId)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.OrderId,
                    TotalHouseHold = s.Total ?? 0 + s.TotalAdjustment ?? 0,
                    TargetHouseHold = s.Penetration ?? 0 + s.CountAdjustment ?? 0,
                    Penetration = (s.Total ?? 0 + s.TotalAdjustment ?? 0) > 0 ? (float)(s.Penetration ?? 0 + s.CountAdjustment ?? 0) / (float)(s.Total ?? 0 + s.TotalAdjustment ?? 0) : 0,
                    s.ColorR,
                    s.ColorG,
                    s.ColorB,
                    DMaps = s.DistributionMaps.Select(d => new
                    {
                        d.Id,
                        d.Name,
                        d.ColorR,
                        d.ColorG,
                        d.ColorB,
                        d.CountAdjustment,
                        d.Penetration,
                        d.Percentage,
                        d.Total,
                        d.TotalAdjustment
                    })
                }).ToArray();
            int? totalHouseHold = null;
            int? targetHouseHold = null;
            float? penetration = null;
            if (submaps != null && submaps.Length > 0)
            {
                totalHouseHold = submaps.Sum(i => i.TotalHouseHold);
                targetHouseHold = submaps.Sum(i => i.TargetHouseHold);
                penetration = (totalHouseHold ?? 0) > 0 ? (float)(targetHouseHold ?? 0) / (float)(totalHouseHold ?? 0) : 0;
            }
            return Json(new
            {
                SubMaps = submaps,
                campaign.AreaDescription,
                campaign.ClientCode,
                campaign.ClientName,
                campaign.ContactName,
                campaign.CreatorName,
                campaign.CustemerName,
                Date = campaign.Date.Value,
                campaign.Description,
                campaign.Id,
                campaign.Latitude,
                campaign.Longitude,
                Logo = CampaignLogoFix.LogoPath(campaign.Logo),
                campaign.Name,
                campaign.Sequence,
                campaign.UserName,
                TotalHouseHold = totalHouseHold,
                TargetHouseHold = targetHouseHold,
                Penetration = penetration,
                DisplayName = string.Format("{0}-{1}-{2}-{3}-{4}",
                    campaign.Date.Value.ToString("MMddyy"),
                    campaign.ClientCode,
                    campaign.CreatorName,
                    campaign.AreaDescription,
                    campaign.Sequence)
            });
        }

        /// <summary>
        /// Length mesaures id 1 = miles 2 = km
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns>Meters</returns>
        [HttpGet]
        [Route("campaign/{campaignId:int}/address")]
        public IHttpActionResult GetCampaignAddress(int campaignId)
        {
            var campaign = db.Campaigns.FirstOrDefault(i => i.Id == campaignId);
            if(campaign == null)
            {
                return NotFound();
            }

            //1 Miles 2 Kilometers return Meters
            var result = campaign.Addresses.Select(i => new
            {
                color = i.Color.ToLower() == "green" ? "#00FF00" : "#FF0000",
                center = new ViewModel.Location(i.Latitude, i.Longitude),
                circle = i.Radiuses.Where(s => s.IsDisplay == true)
                    .Select(s => new {
                        label = string.Format("{0}{1}", (s.Length??0).ToString("#.0"), s.LengthMeasuresId == 2 ? "K": "M"),
                        radius = s.LengthMeasuresId == 2 ? s.Length * 1000 : s.Length * 1609.34
                    })
                    .ToArray()
            }).ToArray();
            return Json(new { address = result });
        }
        #endregion

        #region SubMap
        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}")]
        public async Task<IHttpActionResult> GetSubmapForPrint(int campaignId, int submapId)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId);
            if (campaign == null)
            {
                return NotFound();
            }

            var submaps = campaign.SubMaps.Where(i => i.Id == submapId)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.OrderId,
                    TotalHouseHold = s.Total ?? 0 + s.TotalAdjustment ?? 0,
                    TargetHouseHold = s.Penetration ?? 0 + s.CountAdjustment ?? 0,
                    Penetration = (s.Total ?? 0 + s.TotalAdjustment ?? 0) > 0 ? (float)(s.Penetration ?? 0 + s.CountAdjustment ?? 0) / (float)(s.Total ?? 0 + s.TotalAdjustment ?? 0) : 0,
                    s.ColorR,
                    s.ColorG,
                    s.ColorB,
                }).FirstOrDefault();
            return Json(submaps);
        }

        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}/boundary")]
        public IHttpActionResult GetBoundaryForSubMap(int campaignId, int submapId)
        {
            var result = GetSubMapBoundary(campaignId, submapId);
            var color = db.Campaigns.FirstOrDefault(i => i.Id == campaignId)
                .SubMaps.FirstOrDefault(i => i.Id == submapId);
            return Json(new
            {
                color = new
                {
                    r = color.ColorR,
                    g = color.ColorG,
                    b = color.ColorB
                },
                boundary = result
            });
        }

        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}/location")]
        public IHttpActionResult GetSubmapLocation(int campaignId, int submapId)
        {            
            var submap = db.Campaigns.FirstOrDefault(i => i.Id == campaignId).SubMaps.FirstOrDefault(i=>i.Id == submapId);
            if(submap == null)
            {
                return NotFound();
            }
            
            var result = new List<Models.Location[]>();
            var record = submap.SubMapRecords.FirstOrDefault();
            if(record == null)
            {
                return Json(new { });
            }
            IQueryable<ViewModel.Location> query = null;
            switch (record.Classification)
            {
                case 15: //PremiumCRoutes
                    {
                        query = from l in db.PremiumCRouteCoordinates
                                    join r in db.PremiumCRoutes on l.PreminumCRouteId equals r.Id
                                    join s in db.SubMapRecords on r.Id equals s.AreaId
                                    where s.SubMapId == submapId && s.Value == true
                                    orderby r.Id, l.Id
                                    select new ViewModel.Location
                                    {
                                        Id = s.Id,
                                        Longitude = l.Longitude ?? 0.0,
                                        Latitude = l.Latitude ?? 0.0
                                    };
                    }
                    break;
                case 1: //FiveZips
                    {
                        query = from l in db.FiveZipCoordinates
                                    join r in db.FiveZipAreas on l.FiveZipAreaId equals r.Id
                                    join s in db.SubMapRecords on r.Id equals s.AreaId
                                    where s.SubMapId == submapId && s.Value == true
                                    orderby r.Id, l.Id
                                    select new ViewModel.Location
                                    {
                                        Id = s.Id,
                                        Longitude = l.Longitude ?? 0.0,
                                        Latitude = l.Latitude ?? 0.0
                                    };
                    }
                    break;
                case 2: //Tracts
                    {
                        query = from l in db.TractCoordinates
                                    join r in db.Tracts on l.TractId equals r.Id
                                    join s in db.SubMapRecords on r.Id equals s.AreaId
                                    where s.SubMapId == submapId && s.Value == true
                                    orderby r.Id, l.Id
                                    select new ViewModel.Location
                                    {
                                        Id = s.Id,
                                        Longitude = l.Longitude ?? 0.0,
                                        Latitude = l.Latitude ?? 0.0
                                    };
                    }
                    break;
                case 3: //BlockGroups
                    {
                        query = from l in db.BlockGroupCoordinates
                                    join r in db.BlockGroups on l.BlockGroupId equals r.Id
                                    join s in db.SubMapRecords on r.Id equals s.AreaId
                                    where s.SubMapId == submapId && s.Value == true
                                    orderby r.Id, l.Id
                                    select new ViewModel.Location
                                    {
                                        Id = s.Id,
                                        Longitude = l.Longitude ?? 0.0,
                                        Latitude = l.Latitude ?? 0.0
                                    };
                    }
                    break;
                default:
                    break;
            }
            
            var polygon = FormatLocationGroup(query);
            var boundary = GetSubMapBoundary(campaignId, submapId);
            return Json(new {
                color = new {
                    r = submap.ColorR,
                    g = submap.ColorG,
                    b = submap.ColorB
                },
                polygon = polygon,
                boundary = boundary,

            });
        }

        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}/record")]
        public IHttpActionResult GetRecord(int campaignId, int submapId)
        {
            var campaign = db.Campaigns.FirstOrDefault(i => i.Id == campaignId);
            if (campaign == null)
            {
                return NotFound();
            }
            var submap = campaign.SubMaps.FirstOrDefault(i => i.Id == submapId);
            if (submap == null)
            {
                return NotFound();
            }

            var record = submap.SubMapRecords.FirstOrDefault(i => i.Value == true);
            if (record == null)
            {
                return Json(new { });
            }


            return Json(new
            {
                campaignId = campaignId,
                submapId = submapId,
                record = LoadRecordInSubmap(campaign, submapId)
            });
        }
        #endregion

        #region DMap
        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}/dmap/{dmapId:int}/boundary")]
        public IHttpActionResult GetBoundaryForDMap(int campaignId, int submapId, int dmapId)
        {
            var result = GetDMapBoundary(campaignId, submapId, dmapId);
            var color = db.Campaigns.FirstOrDefault(i => i.Id == campaignId)
                .SubMaps.FirstOrDefault(i => i.Id == submapId)
                .DistributionMaps.FirstOrDefault(i => i.Id == dmapId);

            return Json(new
            {
                color = new
                {
                    r = color.ColorR,
                    g = color.ColorG,
                    b = color.ColorB
                },
                boundary = result
            });
        }

        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}/dmap/{dmapId:int}/location")]
        public IHttpActionResult GetDmapLocation(int campaignId, int submapId, int dmapId)
        {
            var dmap = db.Campaigns.FirstOrDefault(i => i.Id == campaignId)
               .SubMaps.FirstOrDefault(i => i.Id == submapId)
               .DistributionMaps.FirstOrDefault(i => i.Id == dmapId);
            if (dmap == null)
            {
                return NotFound();
            }

            var result = new List<Models.Location>();
            var record = dmap.DistributionMapRecords.FirstOrDefault();
            if (record == null)
            {
                return Json(result);
            }
            
            var boundary = GetDMapBoundary(campaignId, submapId, dmapId);
            return Json(new
            {
                color = new
                {
                    r = dmap.ColorR,
                    g = dmap.ColorG,
                    b = dmap.ColorB
                },
                boundary = boundary
            });
        }

        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}/dmap/{dmapId:int}/gtu")]
        public IHttpActionResult GetDmapGTU(int campaignId, int submapId, int dmapId)
        {
            return LoadGtuWithoutTaskStatus(campaignId, submapId, dmapId, true, null);
        }

        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}/dmap/{dmapId:int}/gtu/inside")]
        public IHttpActionResult GetGtuInsideDmap(int campaignId, int submapId, int dmapId)
        {
            return LoadGtuWithoutTaskStatus(campaignId, submapId, dmapId, true, null);
        }

        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}/dmap/{dmapId:int}/gtu/inside/{date}")]
        public IHttpActionResult GetGtuInsideDmapAfterTime(int campaignId, int submapId, int dmapId, [DateTimeParameter]DateTime? date)
        {
            return LoadGtuWithoutTaskStatus(campaignId, submapId, dmapId, true, date);
        }

        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}/dmap/{dmapId:int}/gtu/all")]
        public IHttpActionResult GetGtuAll(int campaignId, int submapId, int dmapId)
        {
            return LoadGtuWithoutTaskStatus(campaignId, submapId, dmapId, false, null);
        }

        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}/dmap/{dmapId:int}/gtu/all/{date}")]
        public IHttpActionResult GetGtuAllAfterTime(int campaignId, int submapId, int dmapId, [DateTimeParameter]DateTime? date)
        {
            return LoadGtuWithoutTaskStatus(campaignId, submapId, dmapId, false, date);
        }

        private IHttpActionResult LoadGtuWithoutTaskStatus(int campaignId, int submapId, int dmapId, bool filterOutside, DateTime? lastTime)
        {
            var dmap = db.Campaigns.FirstOrDefault(i => i.Id == campaignId)
                .SubMaps.FirstOrDefault(i => i.Id == submapId)
                .DistributionMaps.FirstOrDefault(i => i.Id == dmapId);
            if (dmap == null)
            {
                return NotFound();
            }

            var query = from task in db.Tasks
                        join mapping in db.TaskGtuInfoMappings on task.Id equals mapping.TaskId
                        join gtu in db.GtuInfos on mapping.Id equals gtu.TaskgtuinfoId
                        where task.DistributionMapId == dmapId && (lastTime == null || gtu.dtReceivedTime > lastTime)
                        orderby task.Id, mapping.GTUId, gtu.Id
                        select new
                        {
                            Time = gtu.dtReceivedTime,
                            GTUId = mapping.GTUId,
                            Latitude = gtu.dwLatitude,
                            Longitude = gtu.dwLongitude
                        };

            var locationQuery = query.Select(i => new ViewModel.Location
            {
                Id = i.GTUId,
                Latitude = i.Latitude,
                Longitude = i.Longitude
            });

            var lastReceivedTimeQuery = query.Max(i => i.Time);

            var dmapPolygon = GetDMapBoundary(campaignId, submapId, dmapId);
            
            List<long?> validGtuId;
            var locations = FormatLocationGroup(locationQuery, dmapPolygon, out validGtuId, filterOutside);
            var colors = new List<string>();
            if (validGtuId.Count > 0)
            {
                var colorQuery = from task in db.Tasks
                                 join mapping in db.TaskGtuInfoMappings on task.Id equals mapping.TaskId
                                 where task.DistributionMapId == dmapId && validGtuId.Contains(mapping.GTUId)
                                 orderby task.Id, mapping.GTUId
                                 select mapping.UserColor;
                colors = colorQuery.ToList();
            }

            var lastUpdateTime = DateTime.Now;
            if (lastReceivedTimeQuery.HasValue)
            {
                lastUpdateTime = lastReceivedTimeQuery.Value;
            }else if (lastTime.HasValue)
            {
                lastUpdateTime = lastTime.Value;
            }

            return Json(new
            {
                points = locations,
                pointsColors = colors.ToList(),
                lastUpdateTime = lastUpdateTime.ToString("yyyyMMddhhmmss")
            });
        }
        #endregion

        #region Helper
        public List<ViewModel.Record> LoadRecordInSubmap(Campaign campaign, int submapId)
        {
            List<ViewModel.Record> result = new List<ViewModel.Record>();

            var record = campaign.SubMaps.FirstOrDefault(i => i.Id == submapId).SubMapRecords.FirstOrDefault(i => i.Value == true);
            IQueryable<ViewModel.Record> query = null;
            switch (record.Classification)
            {
                case 15: //PremiumCRoutes
                    {
                        query = from submapRecord in db.SubMapRecords
                                join area in db.PremiumCRoutes on submapRecord.AreaId equals area.Id
                                where submapRecord.SubMapId == submapId && submapRecord.Value == true
                                select new
                                {
                                    AreaId = area.Id,
                                    CampaignId = campaign.Id,
                                    Name = area.GEOCODE,
                                    area.APT_COUNT,
                                    area.HOME_COUNT
                                }
                                into combo
                                join  imported in db.CampaignCRouteImporteds.Where(i => i.CampaignId == campaign.Id)
                                on combo.AreaId equals imported.PremiumCRouteId into left                                
                                from r in left.DefaultIfEmpty()
                                orderby combo.AreaId
                                select new ViewModel.Record
                                {
                                    Id = combo.AreaId,
                                    Name = combo.Name,
                                    APT = r._IsPartModified == 1 ? r.PartPercentage * combo.APT_COUNT : combo.APT_COUNT,
                                    Home = r._IsPartModified == 1 ? r.PartPercentage * combo.HOME_COUNT : combo.HOME_COUNT,
                                    TotalHouseHold = 0,
                                    TargetHouseHold = r._IsPartModified == 1 ? r.PartPercentage * r.Penetration : r.Penetration
                                };
                    }
                    break;
                default:
                    break;
            }
            if (query == null)
            {
                return result;
            }
            //fix total should base campaign area description
            var areaDesc = campaign.AreaDescription;
            const string APT = "APT ONLY";
            const string HOME = "HOME ONLY";
            const string APT_HOME = "APT + HOME";
            var debug = query.ToString();
            foreach (var item in query)
            {
                if (record.Classification == 15)
                {
                    switch (areaDesc)
                    {
                        case APT:
                            item.TotalHouseHold = item.APT;
                            break;
                        case HOME:
                            item.TotalHouseHold = item.Home;
                            break;
                        case APT_HOME:
                            item.TotalHouseHold = item.APT + item.Home;
                            break;
                        default:
                            break;
                    }
                    item.TotalHouseHold = Math.Round(item.TotalHouseHold ?? 0, 0);
                }
                result.Add(item);
            }
            return result;
        }

        private List<ViewModel.Location> GetSubMapBoundary(int campaignId, int submapId)
        {
            var coordinates = db.Campaigns.FirstOrDefault(i => i.Id == campaignId)
                .SubMaps.FirstOrDefault(i => i.Id == submapId)
                .SubMapCoordinates
                .OrderBy(i => i.Id)
                .Select(i => new ViewModel.Location
                {
                    Latitude = i.Latitude,
                    Longitude = i.Longitude
                }).ToList();
            
            var polygon = BuildPolygon(coordinates);
            if(polygon == null)
            {
                return new List<ViewModel.Location>();
            }
            return polygon.Boundary.Coordinates.Select(i => new ViewModel.Location
            {
                Latitude = i.Y,
                Longitude = i.X
            }).ToList();
        }
        private List<ViewModel.Location> GetDMapBoundary(int campaignId, int submapId, int dmapId)
        {
            var polygon = GetDMapPolygon(dmapId);
            return polygon.Boundary.Coordinates.Select(i => new ViewModel.Location {
                Latitude = i.Y,
                Longitude = i.X
            }).ToList();
        }

        internal IGeometry GetDMapPolygon(int dmapId)
        {
            var coordinates = db.DistributionMaps
                .FirstOrDefault(i => i.Id == dmapId)
                .DistributionMapCoordinates
                .OrderBy(i => i.Id)
                .Select(i => new ViewModel.Location
                {
                    Latitude = i.Latitude,
                    Longitude = i.Longitude
                }).ToList();

            return BuildPolygon(coordinates);
        }

        /// <summary>
        /// base the location id to return location group
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private List<ViewModel.Location[]> FormatLocationGroup(IQueryable<ViewModel.Location> coordinates)
        {
            List<long?> groupKey;
            return FormatLocationGroup(coordinates, null, out groupKey);
        }

        /// <summary>
        /// base the location id to return location group
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private List<ViewModel.Location[]> FormatLocationGroup(IQueryable<ViewModel.Location> coordinates, List<ViewModel.Location> filterPolygon, out List<long?> groupKeys, bool filterOuterSide = true)
        {
            groupKeys = new List<long?>();

            var polygon = filterPolygon != null ? BuildPolygon(filterPolygon) : null;
            List<ViewModel.Location[]> result = new List<ViewModel.Location[]>();
            long? lastId = null;
            List<ViewModel.Location> area = new List<ViewModel.Location>();
            HashSet<string> ducplicateLocation = new HashSet<string>();
            foreach (var item in coordinates)
            {
                float lat = (int)(item.Latitude * 100000) / 100000f;
                float lng = (int)(item.Longitude * 100000) / 100000f;
                var key = string.Format("{0}:{1}", lat, lng);
                if (ducplicateLocation.Contains(key))
                {
                    continue;
                }
                else
                {
                    ducplicateLocation.Add(key);
                }

                var point = new Point(lng, lat);
                if (filterOuterSide && (polygon == null || !polygon.Contains(point)))
                {
                    continue;
                }
                item.OutOfBoundary = !polygon.Contains(point);
                if (!lastId.HasValue || lastId.Value == item.Id)
                {
                    area.Add(item);
                }
                else
                {
                    groupKeys.Add(lastId);
                    result.Add(area.ToArray());
                    area.Clear();
                    area.Add(item);
                    ducplicateLocation.Clear();
                }
                lastId = item.Id;
            }
            if (lastId.HasValue)
            {
                groupKeys.Add(lastId);
            }
            if(area.Count > 0)
            {
                result.Add(area.ToArray());
            }
            
            return result;
        }

        private IGeometry BuildPolygon(List<ViewModel.Location> locations)
        {
            if(locations == null || locations.Count == 0)
            {
                return null;
            }
            var coordinate = locations.Select(i => new Coordinate
            {
                Y = i.Latitude ?? 0,
                X = i.Longitude ?? 0
            }).ToList();
            coordinate.Add(new Coordinate {
                Y = locations[0].Latitude ?? 0,
                X = locations[0].Longitude ?? 0
            });

            var polygon = new Polygon(new LinearRing(coordinate.ToArray()));
            return polygon.Buffer(0);
        }
        #endregion
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
