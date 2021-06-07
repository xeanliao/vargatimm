using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Enum;
using GPS.DataLayer;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.Area.AreaOperators;
using System.Web;

namespace GPS.DomainLayer.Area
{
    public class MapAreaManager
    {
        #region GetAreas

        public static IEnumerable<IArea> GetAreas(Classifications classification, int boxId)
        {
            IEnumerable<IArea> areas;
            areas = MapAreaFactory.CreateAreaList(classification, boxId);
            return areas;
        }

        public static IEnumerable<IArea> GetAreas(Classifications classification, int boxId, int campaignId)
        {
            IEnumerable<IArea> areas;
            areas = MapAreaFactory.CreateAreaList(classification, boxId, campaignId);
            return areas;
        }

        public static IEnumerable<IArea> GetAreasByFiveZipCode(string zipCode)
        {
            FiveZipRepository fiveZipRepository = new FiveZipRepository();
            List<FiveZipArea> fiveZips = fiveZipRepository.GetAreaByZipCode(zipCode);
            return new FiveZipAreaOperator().ConvertToAreas(fiveZips);
        }
        /// <summary>
        /// Return the Census Tracts matching the specified state code, county code, and tract code.
        /// </summary>
        /// <param name="stateCode">The state code of the Census Tract.</param>
        /// <param name="countyCode">The county code of the Census Tract.</param>
        /// <param name="tractCode">The tract code of the Census Tract.</param>
        /// <returns>A list of <see cref="IArea"/>.</returns>
        public static IEnumerable<IArea> GetTractAreas(string stateCode, string countyCode, string code)
        {
            TractRepository repository = new TractRepository();
            return new TractOperator().ConvertToAreas(repository.GetTracts(stateCode, countyCode, code));
        }
        /// <summary>
        /// Return the Census Block Groups matching the specified state code, county code, 
        /// tract code, and block group code.
        /// </summary>
        /// <param name="stateCode">The state code of the Census Block Group.</param>
        /// <param name="countyCode">The county code of the Census Block Group.</param>
        /// <param name="tractCode">The tract code of the Census Block Group.</param>
        /// <param name="bgCode">The block group code the Census Block Group.</param>
        /// <returns>An enumerable list of <see cref="BlockGroup"/>.</returns>
        public static IEnumerable<IArea> GetBlockGroupAreas(string stateCode, string countyCode, string tractCode, string code)
        {
            BlockGroupRepository repository = new BlockGroupRepository();
            return new BlockGroupOperator().ConvertToAreas(repository.GetBlockGroups(stateCode, countyCode, tractCode, code));
        }

        #endregion

        #region Get Json

        private static string GetBaseJson(Classifications classification, int boxId)
        {
            string json = "[]";
            if (JsonAreaCache.Contains(classification, boxId))
            {
                json = JsonAreaCache.Get(classification, boxId);
            }
            else
            {
                IEnumerable<IArea> areas = GetAreas(classification, boxId);
                json = JsonConvert.SerializeObject(areas);
                JsonAreaCache.Add(classification, boxId, json);
            }
            return json;
        }

        public static string GetJsonString(Classifications classification, int boxId)
        {
            string json;
            if (JsonAreaTempCache.Contains(classification, boxId))
            {
                json = JsonAreaTempCache.Get(classification, boxId);
                if (json == null)
                {
                    IEnumerable<IArea> areas = GetAreas(classification, boxId);
                    json = JsonConvert.SerializeObject(areas);
                    JsonAreaTempCache.Add(classification, boxId, json);
                }
            }
            else
            {
                json = GetBaseJson(classification, boxId);
            }

            return json;
        }

        public static string GetJsonString(Classifications classification, int boxId, int campaignId)
        {
            string json;

            if (JsonAreaCache.Contains(classification, boxId, campaignId))
            {
                json = JsonAreaCache.Get(classification, boxId, campaignId);
                if (json == null)
                {
                    IEnumerable<IArea> areas = GetAreas(classification, boxId, campaignId);
                    json = JsonConvert.SerializeObject(areas);
                    JsonAreaCache.Add(classification, boxId, campaignId, json);
                }
            }
            else
            {
                //json = GetBaseJson(classification, boxId);
                IEnumerable<IArea> areas = GetAreas(classification, boxId, campaignId);
                json = JsonConvert.SerializeObject(areas);
                JsonAreaCache.Add(classification, boxId, campaignId, json);
            }

            return json;
        }

        #endregion

        #region CustomArea
        public static bool ExistCustomAreaName(string name)
        {
            CustomAreaRepository repository = new CustomAreaRepository();
            return repository.GetCustomArea(name) != null;
        }
        public static int AddCustomArea(string name, int total, string description, double[][] points)
        {
            CustomArea area = new CustomArea();
            area.Name = name;
            area.total = total;
            area.Description = description;
            area.IsEnabled = false;
            area.CustomAreaCoordinates = new List<CustomAreaCoordinate>();
            List<ICoordinate> coordinates = new List<ICoordinate>();
            foreach (double[] ps in points)
            {
                CustomAreaCoordinate c = new CustomAreaCoordinate();
                c.CustomArea = area;
                c.Latitude = ps[0];
                c.Longitude = ps[1];
                coordinates.Add(c);
                area.CustomAreaCoordinates.Add(c);
            }
            area.CustomAreaBoxMappings = new List<CustomAreaBoxMapping>();
            List<int> boxIds = ShapeMethods.GetShapeBoxIds(coordinates, 25, 40);
            foreach (int boxId in boxIds)
            {
                CustomAreaBoxMapping mapping = new CustomAreaBoxMapping();
                mapping.BoxId = boxId;
                mapping.CustomArea = area;
                area.CustomAreaBoxMappings.Add(mapping);
            }
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                ws.Repositories.CustomAreaRepository.InsertCustomArea(area);
                ws.Commit();
            }
            JsonAreaCache.ClearAreas();
            return area.Id;
        }
        public static void RemoveCustomArea(string name)
        {
            CustomAreaRepository repository = new CustomAreaRepository();
            CustomArea area = repository.GetCustomArea(name);
            if (area != null)
            {
                repository.DeleteCustomArea(area);
                JsonAreaCache.ClearAreas();
            }
        }
        #endregion

        #region Set Enabled

        public static void SetFiveZipEnable(string code, int total, string description, bool enabled)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                ws.Repositories.FiveZipRepository.SetFiveZipEnabled(code, total, description, enabled);
            }
            JsonAreaCache.ClearAreas();
        }

        public static void SetTractEnable(string stateCode, string countyCode, string code, int total, string description, bool enabled)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                ws.Repositories.TractRepository.SetTractEnabled(stateCode, countyCode, code, total, description, enabled);
            }
            JsonAreaCache.ClearAreas();
        }

        public static void SetBlockGroupEnable(string stateCode, string countyCode, string tractCode, string code, int total, string description, bool enabled)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                ws.Repositories.BlockGroupRepository.SetBlockGroupEnabled(stateCode, countyCode, tractCode, code, total, description, enabled);
            }
            JsonAreaCache.ClearAreas();
        }

        #endregion

        #region non-deliverable Address
        /// <summary>
        /// find an existing non-deliverable address 
        /// </summary>
        /// <param name="street">street address</param>
        /// <param name="zipCode">zip code</param>
        /// <param name="geofence">geofence feet</param>
        /// <param name="description">description of non-deliverable address </param>
        /// <returns></returns>
        private static MapNdAddress FindAddress(string street, string zipCode, int geofence, string description)
        {
            MapNdAddress address = null;
            GeocodeService finder = new GeocodeService();
            ICoordinate coordinate = finder.GeocodePoint(street, zipCode);
            if (coordinate != null)
            {
                MapCircle circle = new MapCircle(coordinate, geofence / 5280.0);
                circle.CalculateCoordinates();
                address = new MapNdAddress();
                address.Id = Guid.NewGuid().GetHashCode();
                address.Street = street;
                address.ZipCode = zipCode;
                address.Geofence = geofence;
                address.Description = description;
                address.Latitude = circle.Center.Latitude;
                address.Longitude = circle.Center.Longitude;
                address.Locations = circle.Coordinates;
            }
            return address;
        }
        /// <summary>
        /// add an non-deliverable address 
        /// </summary>
        /// <param name="street">street address</param>
        /// <param name="zipCode">zip code</param>
        /// <param name="geofence">geo-fence feet</param>
        /// <param name="description">description of non-deliverable address</param>
        /// <returns></returns>
        public static MapNdAddress AddNonDeliverableAddress(string street, string zipCode, int geofence, string description)
        {
            NdAddressRepository repository = new NdAddressRepository();
            if (repository.GetNdAddresses(street, zipCode).Count == 0)
            {
                MapNdAddress address = FindAddress(street, zipCode, geofence, description);
                if (address != null)
                {
                    NdAddress ndAddress = new NdAddress()
                    {
                        //Id = address.Id,
                        Street = address.Street,
                        ZipCode = address.ZipCode,
                        Latitude = address.Latitude,
                        Longitude = address.Longitude,
                        Geofence = address.Geofence,
                        Description = address.Description
                    };

                    List<NdAddressCoordinate> ndCoordinates = new List<NdAddressCoordinate>();
                    foreach (ICoordinate c in address.Locations)
                    {
                        NdAddressCoordinate ndCoordinate = new NdAddressCoordinate()
                        {
                            NdAddress = ndAddress,
                            Latitude = c.Latitude,
                            Longitude = c.Longitude
                        };
                        ndCoordinates.Add(ndCoordinate);
                    }
                    ndAddress.NdAddressCoordinates = ndCoordinates;
                    List<NdAddressBoxMapping> mappings = new List<NdAddressBoxMapping>();
                    List<int> boxIds = ShapeMethods.GetShapeBoxIds(address.Locations, 25, 40);
                    foreach (int boxId in boxIds)
                    {
                        NdAddressBoxMapping mapping = new NdAddressBoxMapping()
                        {
                            BoxId = boxId,
                            NdAddress = ndAddress
                        };
                        mappings.Add(mapping);
                    }
                    ndAddress.NdAddressBoxMappings = mappings;
                    repository.InsertNdAddress(ndAddress);
                    //reset the mapaddress's id,synchronize with the ndaddress
                    address.Id = ndAddress.Id;

                    if (HttpContext.Current != null)
                    {
                        JsonAreaCache.ClearAreas();
                    }
                   
                }
                return address;
            }
            else
            { return null; }
        }
        /// <summary>
        /// Remove an existing non-deliverable address
        /// </summary>
        /// <param name="addressId">non-deliverable address id</param>
        public static void RemoveNonDeliverableAddress(int addressId)
        {
            NdAddressRepository repository = new NdAddressRepository();
            NdAddress addess = repository.GetNdAddress(addressId);
            if (addess != null)
            {
                repository.DeleteNdAddress(addess);
                JsonAreaCache.ClearAreas();
            }
        }

        #endregion
    }

}
