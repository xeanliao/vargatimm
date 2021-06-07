using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.Area.AreaOperators
{
    public class PremiumCRouteOperator : AreaOperator<PremiumCRoute>
    {
        protected override IEnumerable<PremiumCRoute> GetBoxItems(int boxId)
        {
            PremiumCRouteRepository repository = new PremiumCRouteRepository();
            return repository.GetBoxItems(boxId);
        }

        public IEnumerable<MapArea> Gets(string code)
        {
            List<MapArea> areas = new List<MapArea>();
            PremiumCRouteRepository repository = new PremiumCRouteRepository();
            IEnumerable<PremiumCRoute> items = repository.GetItemsByCode(code);
            foreach (PremiumCRoute item in items)
            {
                areas.Add(ConvertToArea(item));
            }
            return areas;
        }

        public IEnumerable<MapArea> Gets(int campaignId, string code)
        {
            List<MapArea> areas = new List<MapArea>();
            PremiumCRouteRepository repository = new PremiumCRouteRepository();
            IEnumerable<PremiumCRoute> items = repository.GetItemsByCode(code);
            ICampaignRepository crepository = WorkSpaceManager.Instance.NewWorkSpace().Repositories.CampaignRepository;
            Campaign campaign = crepository.GetEntity(campaignId);
            if (campaign != null)
            {
                foreach (PremiumCRoute item in items)
                {
                    areas.Add(ConvertToArea(campaign, item));
                }
            }
            else
            {
                foreach (PremiumCRoute item in items)
                {
                    areas.Add(ConvertToArea(item));
                }
            }

            return areas;
        }

        protected override MapArea ConvertToArea(PremiumCRoute item)
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
                Relations = GetRelations(item.PremiumCRouteSelectMappings)
            };

            var coordinates = item.PremiumCRouteCoordinates.OrderBy(t => t.Id);
            foreach (var coordinate in coordinates)
            {
                area.Locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude, coordinate.ShapeId));
            }

            area.Attributes.Add("OTotal", item.OTotal.ToString());
            area.Attributes.Add("PartCount", item.PartCount.ToString());
            area.Attributes.Add("Zip", item.ZIP);

            area.Attributes.Add("Apt", item.APT_COUNT.ToString());
            area.Attributes.Add("Business", item.BUSINESS_COUNT.ToString());
            area.Attributes.Add("Home", item.HOME_COUNT.ToString());
            area.Attributes.Add("Sum", item.TOTAL_COUNT.ToString());
            area.Attributes.Add("All", (item.APT_COUNT + item.HOME_COUNT).ToString());
            area.Attributes.Add("IsInnerRing", item.IsInnerRing.ToString());
            return area;
        }

        protected override MapArea ConvertToArea(Campaign campaign, PremiumCRoute item)
        {
            MapArea area = ConvertToArea(item);
            var data = campaign.CampaignCRouteImporteds.Where(t => t.PremiumCRoute.Id == item.Id).SingleOrDefault();
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
            

            return area;
        }


        public Dictionary<int, Dictionary<int, bool>> GetRelations(IList<PremiumCRouteSelectMapping> mappings)
        {
            Dictionary<int, Dictionary<int, bool>> relations = new Dictionary<int, Dictionary<int, bool>>();
            foreach (PremiumCRouteSelectMapping mapping in mappings)
            {
                // add Z5 relations
                if (!relations.ContainsKey((int)Classifications.Z5))
                {
                    relations.Add((int)Classifications.Z5, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rz5s = relations[(int)Classifications.Z5];
                if (!rz5s.ContainsKey(mapping.FiveZipAreaId))
                {
                    rz5s.Add(mapping.FiveZipAreaId, true);
                }

                // add Z3 relations
                if (!relations.ContainsKey((int)Classifications.Z3))
                {
                    relations.Add((int)Classifications.Z3, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rz3s = relations[(int)Classifications.Z3];
                if (!rz3s.ContainsKey(mapping.ThreeZipAreaId))
                {
                    rz3s.Add(mapping.ThreeZipAreaId, true);
                }
            }
            return relations;
        }


        public override PremiumCRoute GetItem(int id)
        {
            PremiumCRouteRepository repository = new PremiumCRouteRepository();
            return repository.GetItem(id);
        }


    }
}
