using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.Interfaces;
using System.Data.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.Area.AreaOperators
{
    public class TractOperator : AreaOperator<Tract>
    {
        #region Get
        protected override IEnumerable<Tract> GetBoxItems(int boxId)
        {
            TractRepository repository = new TractRepository();
            return repository.GetBoxItems(boxId);
        }
        public override Tract GetItem(int id)
        {
            TractRepository repository = new TractRepository();
            return repository.GetItem(id);
        }
        #endregion

        #region Convert

        protected override MapArea ConvertToArea(Tract item)
        {
            MapArea area = new MapArea()
            {
                Classification = Classifications.TRK,
                Id = item.Id,
                Name = item.Name,
                IsEnabled = item.IsEnabled,
                Description = item.Description,
                State = item.StateCode,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Relations = GetRelations(item.BlockGroupSelectMappings)
            };

            var coordinates = item.TractCoordinates.OrderBy(t => t.Id);
            foreach (var coordinate in coordinates)
            {
                area.Locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude, coordinate.ShapeId));
            }

            area.Attributes.Add("OTotal", item.OTotal.ToString());
            area.Attributes.Add("State", item.StateCode);
            area.Attributes.Add("County", item.CountyCode);
            area.Attributes.Add("Tract", item.Code);
            return area;
        }

        protected override MapArea ConvertToArea(Campaign campaign, Tract item)
        {
            MapArea area = ConvertToArea(item);
            var data = campaign.CampaignTractImporteds.Where(t => t.Tract.Id == item.Id).SingleOrDefault();
            if (data != null)
            {
                var per = data.Total > 0 ? (double)data.Penetration / (double)data.Total : 0;
                area.Attributes.Add("Total", data.Total.ToString());
                area.Attributes.Add("Count", data.Penetration.ToString());
                area.Attributes.Add("Penetration", per.ToString());
            }
            return area;
        }

        #endregion

        #region Relations

        public Dictionary<int, Dictionary<int, bool>> GetRelations(IList<BlockGroupSelectMapping> mappings)
        {
            Dictionary<int, Dictionary<int, bool>> relations = new Dictionary<int, Dictionary<int, bool>>();
            foreach (BlockGroupSelectMapping mapping in mappings)
            {
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

        #endregion


    }
}
