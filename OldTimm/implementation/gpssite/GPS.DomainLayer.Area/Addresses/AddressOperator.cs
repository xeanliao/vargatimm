using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Entities;
using GPS.DataLayer;
using GPS.DomainLayer.Enum;
using GPS.Utilities.IO;
using System.Configuration;

namespace GPS.DomainLayer.Area.Addresses
{
    public class AddressOperator
    {
        private readonly int BgBoxLat = 3;
        private readonly int BgBoxLon = 4;

        private readonly int CRouteBoxLat = 3;
        private readonly int CRouteBoxLon = 4;

        public AddressRecord[] ReadAddressFile(string color, string filePath)
        {            
            
            AddressRecord[] records = (new FileOperator()).ReadFile<AddressRecord>(filePath) as AddressRecord[];
            if (records == null) 
            {
                MyException e = new MyException("It is an invalid file!");
                         
                throw e;
            }
            return records;
        }

        public Coordinate GetCoordinate(string street, string zipcode)
        {
            return new GeocodeService().GeocodePoint(street, zipcode) as Coordinate;
            //// address can be "912 Joaquin rd, 91007", or "912 Joaquin rd, arcadia, ca"
            //BingGeocodeService.GeocodeRequest request = new BingGeocodeService.GeocodeRequest();
            
            //// Set the credentials using a valid Bing Maps key
            //request.Credentials = new BingGeocodeService.Credentials();
            //request.Credentials.ApplicationId = ConfigurationManager.AppSettings["BinMapKey"];

            //// Set the full address query
            //request.Query = street + ", " + zipcode;

            //// Set the options to only return high confidence results 
            //BingGeocodeService.ConfidenceFilter[] filters = new BingGeocodeService.ConfidenceFilter[1];
            //filters[0] = new BingGeocodeService.ConfidenceFilter();
            //filters[0].MinimumConfidence = BingGeocodeService.Confidence.Low;

            //// Add the filters to the options
            //BingGeocodeService.GeocodeOptions geocodeOptions = new BingGeocodeService.GeocodeOptions();
            //geocodeOptions.Filters = filters;
            //request.Options = geocodeOptions;

            //// Make the geocode request
            //BingGeocodeService.GeocodeService geo = new BingGeocodeService.GeocodeService();
            //BingGeocodeService.GeocodeResponse response = geo.Geocode(request);

            //if (response != null && response.Results != null && response.Results.Length > 0)
            //{
            //    var latitude = response.Results[0].Locations[0].Latitude;
            //    var longitude = response.Results[0].Locations[0].Longitude;

            //    return new Coordinate(latitude, longitude);
            //}
            //else
            //{
            //    Utilities.LogUtils.Info(string.Format("This address is not find on bing map geo service: {0}, {1}", street, zipcode)); 
            //    return null;
            //}
        }

        public MapAddress GetAddress(string street, string zipCode, string color)
        {
            MapAddress address = null;
            //GeocodeService service = new GeocodeService();
            //ICoordinate coordinate = service.GeocodePoint(street, zipCode);
            Coordinate coordinate = this.GetCoordinate(street, zipCode);
            if (coordinate != null)
            {
                address = new MapAddress()
                {
                    Id = Guid.NewGuid().GetHashCode(),
                    Street = street,
                    ZipCode = zipCode,
                    Color = color,
                    OriginalLatitude = coordinate.Latitude,
                    OriginalLongitude = coordinate.Longitude,
                    Latitude = coordinate.Latitude,
                    Longitude = coordinate.Longitude,
                    Radiuses = new List<MapAddressRadius>()
                };

                address.Radiuses.Add(new MapAddressRadius()
                {
                    Id = Guid.NewGuid().GetHashCode(),
                    Length = 1,
                    LengthMeasuresId = 1,
                    IsDisplay = false,
                    Relations = GetRadiusRelations(coordinate, 1, 1)
                    //Relations = new Dictionary<int, Dictionary<int, string>>()
                });
                address.Radiuses.Add(new MapAddressRadius()
                {
                    Id = Guid.NewGuid().GetHashCode(),
                    Length = 2,
                    LengthMeasuresId = 1,
                    IsDisplay = false,
                    Relations = GetRadiusRelations(coordinate, 2, 1)
                    //Relations = new Dictionary<int, Dictionary<int, string>>()
                });
                address.Radiuses.Add(new MapAddressRadius()
                {
                    Id = Guid.NewGuid().GetHashCode(),
                    Length = 3,
                    LengthMeasuresId = 1,
                    IsDisplay = false,
                    Relations = GetRadiusRelations(coordinate, 3, 1)
                    //Relations = new Dictionary<int, Dictionary<int, string>>()
                });
            }

            return address;
        }
        public MapAddress GetAddress(string street, string zipCode, string color,string pic)
        {
            MapAddress address = null;
            //GeocodeService service = new GeocodeService();
            //ICoordinate coordinate = service.GeocodePoint(street, zipCode);
            Coordinate coordinate = this.GetCoordinate(street, zipCode);
            if (coordinate != null)
            {
                address = new MapAddress()
                {
                    //Id = Guid.NewGuid().GetHashCode(),
                    Street = street,
                    ZipCode = zipCode,
                    Color = color,
                    OriginalLatitude = coordinate.Latitude,
                    OriginalLongitude = coordinate.Longitude,
                    Latitude = coordinate.Latitude,
                    Longitude = coordinate.Longitude,
                    Picture = pic,
                    Radiuses = new List<MapAddressRadius>()
                };

                address.Radiuses.Add(new MapAddressRadius()
                {
                    //Id = Guid.NewGuid().GetHashCode(),
                    Length = 1,
                    LengthMeasuresId = 1,
                    IsDisplay = false,
                    Relations = GetRadiusRelations(coordinate, 1, 1)
                    //Relations = new Dictionary<int,Dictionary<int,string>>()
                });
                address.Radiuses.Add(new MapAddressRadius()
                {
                    //Id = Guid.NewGuid().GetHashCode(),
                    Length = 2,
                    LengthMeasuresId = 1,
                    IsDisplay = false,
                    Relations = GetRadiusRelations(coordinate, 2, 1)
                    //Relations = new Dictionary<int, Dictionary<int, string>>()
                });
                address.Radiuses.Add(new MapAddressRadius()
                {
                    //Id = Guid.NewGuid().GetHashCode(),
                    Length = 3,
                    LengthMeasuresId = 1,
                    IsDisplay = false,
                    Relations = GetRadiusRelations(coordinate, 3, 1)
                    //Relations = new Dictionary<int, Dictionary<int, string>>()
                });
            }

            return address;
        }

        public MonitorAddresses GetMonitorAddress(string street, string zipCode, string pic)
        {
            MonitorAddresses address = null;
            //GeocodeService service = new GeocodeService();
            //ICoordinate coordinate = service.GeocodePoint(street, zipCode);
            Coordinate coordinate = this.GetCoordinate(street, zipCode);
            if (coordinate != null)
            {
                address = new MonitorAddresses()
                {
                    Address1 = street,
                    ZipCode = zipCode,
                    OriginalLatitude = coordinate.Latitude,
                    OriginalLongitude = coordinate.Longitude,
                    Latitude = coordinate.Latitude,
                    Longitude = coordinate.Longitude,
                    Picture = pic,
                    //Radiuses = new List<MapAddressRadius>()
                };

                //address.Radiuses.Add(new MapAddressRadius()
                //{
                //    Id = Guid.NewGuid().GetHashCode(),
                //    Length = 1,
                //    LengthMeasuresId = 1,
                //    IsDisplay = false,
                //    //Relations = GetRadiusRelations(coordinate, 1, 1)
                //    Relations = new Dictionary<int, Dictionary<int, string>>()
                //});
                //address.Radiuses.Add(new MapAddressRadius()
                //{
                //    Id = Guid.NewGuid().GetHashCode(),
                //    Length = 2,
                //    LengthMeasuresId = 1,
                //    IsDisplay = false,
                //    //Relations = GetRadiusRelations(coordinate, 2, 1)
                //    Relations = new Dictionary<int, Dictionary<int, string>>()
                //});
                //address.Radiuses.Add(new MapAddressRadius()
                //{
                //    Id = Guid.NewGuid().GetHashCode(),
                //    Length = 3,
                //    LengthMeasuresId = 1,
                //    IsDisplay = false,
                //    //Relations = GetRadiusRelations(coordinate, 3, 1)
                //    Relations = new Dictionary<int, Dictionary<int, string>>()
                //});
            }

            return address;
        }

        public Dictionary<int, Dictionary<int, string>> GetRadiusRelations(ICoordinate center, double length, int lengthMeasuresId)
        {
            return GetRadiusRelations(center, length, lengthMeasuresId, new Dictionary<int, Dictionary<int, string>>());
        }

        public Dictionary<int, Dictionary<int, string>> GetRadiusRelations(ICoordinate center, double length, int lengthMeasuresId, Dictionary<int, Dictionary<int, string>> baseRalations)
        {
            Dictionary<int, Dictionary<int, string>> relations = new Dictionary<int, Dictionary<int, string>>();

            if (!baseRalations.ContainsKey((int)Classifications.BG))
            {
                baseRalations.Add((int)Classifications.BG, new Dictionary<int, string>());
            }
            if (!baseRalations.ContainsKey((int)Classifications.PremiumCRoute))
            {
                baseRalations.Add((int)Classifications.PremiumCRoute, new Dictionary<int, string>());
            }
            List<BlockGroup> bgs = GetBlockGroups(center, length, lengthMeasuresId, baseRalations[(int)Classifications.BG]);
            List<PremiumCRoute> cRoutes = GetCRoutes(center, length, lengthMeasuresId, baseRalations[(int)Classifications.PremiumCRoute]);
            relations.Add((int)Classifications.BG, GetBlockGroupRelations(bgs));
            relations.Add((int)Classifications.TRK, GetTractRelations(bgs));
            relations.Add((int)Classifications.PremiumCRoute, GetCRouteRelations(cRoutes));
            relations.Add((int)Classifications.Z5, GetFiveZipRelations(cRoutes));
            return relations;
        }

        private List<BlockGroup> GetBlockGroups(ICoordinate center, double length, int lengthMeasuresId, Dictionary<int, string> baseItems)
        {
            List<BlockGroup> items = new List<BlockGroup>();
            //List<int> ids = GetBoxIds(
            ICircle circle = new MapCircle(center, lengthMeasuresId == 1 ? length : length * 0.621371192);
            circle.CalculateCoordinates();
            List<int> boxIds = ShapeMethods.GetShapeBoxIds(circle, BgBoxLat, BgBoxLon);
            BlockGroupRepository repository = new BlockGroupRepository();
            IEnumerable<BlockGroup> boxesBgs = repository.GetBoxItems(boxIds);
            foreach (BlockGroup bg in boxesBgs)
            {
                if (baseItems.ContainsKey(bg.Id))
                {
                    items.Add(bg);
                    continue;
                }
                List<ICoordinate> coordinates = new List<ICoordinate>();
                foreach (var coordinate in bg.BlockGroupCoordinates)
                {
                    coordinates.Add(coordinate);
                }
                if (ShapeMethods.ConnectedCirclePolygon(circle, coordinates))
                {
                    items.Add(bg);
                }
            }

            return items;
        }

        private List<PremiumCRoute> GetCRoutes(ICoordinate center, double length, int lengthMeasuresId, Dictionary<int, string> baseItems)
        {
            List<PremiumCRoute> items = new List<PremiumCRoute>();
            ICircle circle = new MapCircle(center, lengthMeasuresId == 1 ? length : length * 0.621371192);
            circle.CalculateCoordinates();
            List<int> boxIds = ShapeMethods.GetShapeBoxIds(circle, CRouteBoxLat, CRouteBoxLon);
            PremiumCRouteRepository repository = new PremiumCRouteRepository();
            IEnumerable<PremiumCRoute> boxesCRoutes = repository.GetBoxItems(boxIds);
            foreach (PremiumCRoute cRoute in boxesCRoutes)
            {
                if (baseItems.ContainsKey(cRoute.Id))
                {
                    items.Add(cRoute);
                    continue;
                }
                List<ICoordinate> coordinates = new List<ICoordinate>();
                foreach (var coordinate in cRoute.PremiumCRouteCoordinates)
                {
                    coordinates.Add(coordinate);
                }

                if (ShapeMethods.ConnectedCirclePolygon(circle, coordinates))
                {
                    items.Add(cRoute);
                }
            }

            return items;
        }

        private Dictionary<int, string> GetBlockGroupRelations(List<BlockGroup> bgs)
        {
            Dictionary<int, string> items = new Dictionary<int, string>();
            foreach (BlockGroup bg in bgs)
            {
                if (!items.ContainsKey(bg.Id))
                {
                    items.Add(bg.Id, bg.Id.ToString());
                }
            }
            return items;
        }

        private Dictionary<int, string> GetTractRelations(List<BlockGroup> bgs)
        {
            Dictionary<int, string> items = new Dictionary<int, string>();
            foreach (BlockGroup bg in bgs)
            {
                foreach (BlockGroupSelectMapping mapping in bg.BlockGroupSelectMappings)
                {
                    if (!items.ContainsKey(mapping.Tract.Id))
                    {
                        items.Add(mapping.Tract.Id, mapping.Tract.Id.ToString());
                    }
                }
            }
            return items;
        }

        private Dictionary<int, string> GetCRouteRelations(List<PremiumCRoute> cRoutes)
        {
            Dictionary<int, string> items = new Dictionary<int, string>();
            foreach (PremiumCRoute cRoute in cRoutes)
            {
                items.Add(cRoute.Id, cRoute.Id.ToString());
            }
            return items;
        }

        private Dictionary<int, string> GetFiveZipRelations(List<PremiumCRoute> cRoutes)
        {
            Dictionary<int, string> items = new Dictionary<int, string>();
            foreach (PremiumCRoute cRoute in cRoutes)
            {
                foreach (PremiumCRouteSelectMapping mapping in cRoute.PremiumCRouteSelectMappings)
                {
                    if (!items.ContainsKey(mapping.FiveZipAreaId))
                    {
                        items.Add(mapping.FiveZipAreaId, mapping.FiveZipAreaId.ToString());
                    }
                }
            }
            return items;
        }

    }
}

/// <summary>
/// </summary>
public class MyException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    public MyException(string message)
        : base(message)
    { }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="ex"></param>
    public MyException(string message, Exception ex)
        : base(message, ex)
    { }

}
