using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Enum;
using System.Data.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.Area.AreaOperators
{
    public class BlockGroupOperator : AreaOperator<BlockGroup>
    {
        protected override IEnumerable<BlockGroup> GetBoxItems(int boxId)
        {
            BlockGroupRepository repository = new BlockGroupRepository();
            return repository.GetBoxItems(boxId);
        }

        public IEnumerable<MapArea> Gets(string code)
        {
            List<MapArea> areas = new List<MapArea>();
            BlockGroupRepository bgRepository = new BlockGroupRepository();
            List<BlockGroup> bgs = bgRepository.GetAreaByBGArbitraryUniqueCode(code);
            foreach (BlockGroup bg in bgs)
            {
                areas.Add(ConvertToArea(bg));
            }
            return areas;
        }

        protected override MapArea ConvertToArea(BlockGroup item)
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
                Relations = GetRelations(item.BlockGroupSelectMappings)
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

        protected override MapArea ConvertToArea(Campaign campaign, BlockGroup item)
        {
            MapArea area = ConvertToArea(item);
            var data = campaign.CampaignBlockGroupImporteds.Where(t => t.BlockGroup.Id == item.Id).SingleOrDefault();
            if (data != null)
            {
                var per = data.Total > 0 ? (double)data.Penetration / (double)data.Total : 0;
                area.Attributes.Add("Total", data.Total.ToString());
                area.Attributes.Add("Count", data.Penetration.ToString());
                area.Attributes.Add("Penetration", per.ToString());
            }
            return area;
        }

        /// <summary>
        /// Convert to relations from mappings
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public Dictionary<int, Dictionary<int, bool>> GetRelations(IList<BlockGroupSelectMapping> mappings)
        {
            Dictionary<int, Dictionary<int, bool>> relations = new Dictionary<int, Dictionary<int, bool>>();
            foreach (BlockGroupSelectMapping mapping in mappings)
            {
                // add trk relations
                if (!relations.ContainsKey((int)Classifications.TRK))
                {
                    relations.Add((int)Classifications.TRK, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rtrks = relations[(int)Classifications.TRK];
                if (!rtrks.ContainsKey(mapping.Tract.Id))
                {
                    rtrks.Add(mapping.Tract.Id, true);
                }

                // add Z5 relations
                if (!relations.ContainsKey((int)Classifications.Z5))
                {
                    relations.Add((int)Classifications.Z5, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rz5s = relations[(int)Classifications.Z5];
                if (!rz5s.ContainsKey(mapping.FiveZipArea.Id))
                {
                    rz5s.Add(mapping.FiveZipArea.Id, true);
                }

                // add Z3 relations
                if (!relations.ContainsKey((int)Classifications.Z3))
                {
                    relations.Add((int)Classifications.Z3, new Dictionary<int, bool>());
                }
                Dictionary<int, bool> rz3s = relations[(int)Classifications.Z3];
                if (!rz3s.ContainsKey(mapping.ThreeZipArea.Id))
                {
                    rz3s.Add(mapping.ThreeZipArea.Id, true);
                }
            }
            return relations;
        }

        public override BlockGroup GetItem(int id)
        {
            BlockGroupRepository repository = new BlockGroupRepository();
            return repository.GetItem(id);
        }

        
    }
}
