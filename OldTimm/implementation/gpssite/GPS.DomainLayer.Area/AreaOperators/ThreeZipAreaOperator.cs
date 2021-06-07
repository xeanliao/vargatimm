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
    /// <summary>
    /// Three Zip Area Operator
    /// </summary>
    public class ThreeZipAreaOperator : AreaOperator<ThreeZipArea>
    {
        #region Get

        protected override IEnumerable<ThreeZipArea> GetBoxItems(int boxId)
        {
            ThreeZipRepository repository = new ThreeZipRepository();
            return repository.GetBoxItems(boxId);
        }

        public override ThreeZipArea GetItem(int id)
        {
            ThreeZipRepository repository = new ThreeZipRepository();
            return repository.GetItem(id);
        }

        #endregion

        #region Convert

        /// <summary>
        /// Convert ThreeZipArea item to IArea
        /// </summary>
        /// <param name="item">ThreeZipArea</param>
        /// <returns>an IArea instance</returns>
        protected override MapArea ConvertToArea(ThreeZipArea item)
        {
            MapArea area = new MapArea()
            {
                Classification = Classifications.Z3,
                Id = item.Id,
                Name = item.Name,
                IsEnabled = true,
                Description = "",
                State = item.StateCode,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
            };

            var coordinates = item.ThreeZipAreaCoordinates.OrderBy(t => t.Id);
            foreach (var coordinate in coordinates)
            {
                area.Locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude, coordinate.ShapeId));
            }

            area.Attributes.Add("Apt", item.APT_COUNT.ToString());
            area.Attributes.Add("Business", item.BUSINESS_COUNT.ToString());
            area.Attributes.Add("Home", item.HOME_COUNT.ToString());
            area.Attributes.Add("Sum", item.TOTAL_COUNT.ToString());
            area.Attributes.Add("All", (item.APT_COUNT + item.HOME_COUNT).ToString());

            return area;
        }

        protected override MapArea ConvertToArea(Campaign campaign, ThreeZipArea item)
        {
            return ConvertToArea(item);
        }

        #endregion
    }
}
