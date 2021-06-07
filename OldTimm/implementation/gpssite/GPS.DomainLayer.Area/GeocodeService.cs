using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Area.MapPointService;
using GPS.DomainLayer.Interfaces;
using log4net;
using System.Configuration;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace GPS.DomainLayer.Area {
    public class GeocodeService {
        //private MapPointService.FindServiceSoap _findService;

        public GeocodeService() {
            //_findService = new FindServiceSoap();
            //_findService.Credentials = new System.Net.NetworkCredential("148494", "wer@sA2134");
            //_findService.PreAuthenticate = true;

            //
            
        }

        public void test()
        {
            //3050 Metro Dr, 51501
            //44055 N Sierra Highway, 93534
            //28401 Los Alisos Blvd, 92692
            //2 Sout Pointe Dr, 92630
            List<string> data = new List<string>() 
            {
                "3050 Metro Dr, 51501",
                "44055 N Sierra Highway, 93534",
                "28401 Los Alisos Blvd, 92692",
                "2 Sout Pointe Dr, 92630"
            };
            StringBuilder result = new StringBuilder();
            foreach (var item in data)
            {
                result.AppendLine(item);
                var info = item.Split(',');
                var resultGoogle = GeocodePointGoogle(info[0], info[1]);
                if (resultGoogle != null)
                {
                    result.AppendFormat("google:{0}:{1}\r\n", resultGoogle.Latitude, resultGoogle.Longitude);
                }
                else
                {
                    result.Append("google:\r\n");
                }
                var resultBing = GeocodePointBing(info[0], info[1]);
                if (resultBing != null)
                {
                    result.AppendFormat("Bing:{0}:{1}\r\n", resultBing.Latitude, resultBing.Longitude);
                }
                else
                {
                    result.Append("Bing:\r\n");
                }

                result.AppendLine();
            }

            var content = result.ToString();
        }

        public ICoordinate GeocodePoint(string streetAddress, string postalCode)
        {
            try
            {
                var result = GeocodePointGoogle(streetAddress, postalCode);
                if (result == null)
                {
                    result = GeocodePointBing(streetAddress, postalCode);
                }
                return result;
            }
            catch (Exception ex)
            {
                ILog logger = LogManager.GetLogger(GetType());
                logger.Info(string.Format("This address is not find on geo service: {0}, {1}", streetAddress, postalCode), ex);
            }
            return null;
            //try
            //{
            //    string geoService = ConfigurationManager.AppSettings["GeoService"];
            //    if (string.Compare(geoService, "Bing", true) == 0)
            //    {
            //        return GeocodePointBing(streetAddress, postalCode);
            //    }
            //    else
            //    {
            //        return GeocodePointGoogle(streetAddress, postalCode);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ILog logger = LogManager.GetLogger(GetType());
            //    logger.Info(string.Format("This address is not find on geo service: {0}, {1}", streetAddress, postalCode), ex);
            //}
            //return null;

        }


        public ICoordinate GeocodePointBing(string streetAddress, string postalCode)
        {
            var service = new BingGeocodeService.GeocodeService();
            BingGeocodeService.GeocodeRequest request = new BingGeocodeService.GeocodeRequest();

            var credentials = new BingGeocodeService.Credentials();
            credentials.ApplicationId = ConfigurationManager.AppSettings["BinMapKey"];
            request.Credentials = credentials;

            request.Address = new BingGeocodeService.Address();
            request.Address.AddressLine = streetAddress;
            request.Address.PostalCode = postalCode;

            request.Options = new BingGeocodeService.GeocodeOptions();

            BingGeocodeService.ConfidenceFilter[] filters = new BingGeocodeService.ConfidenceFilter[1];
            filters[0] = new BingGeocodeService.ConfidenceFilter();
            filters[0].MinimumConfidence = BingGeocodeService.Confidence.Medium;

            request.Options.Filters = filters;

            var response = service.Geocode(request);

            if (response != null && response.Results != null && response.Results.Length > 0)
            {

                var latitude = response.Results[0].Locations[0].Latitude;
                var longitude = response.Results[0].Locations[0].Longitude;
                return new Coordinate(latitude, longitude);
            }
            
            return null;
        }

        public void TestGeocodePointBing()
        {
            GeocodePointGoogle("maple street", "90720");
        }

        public ICoordinate GeocodePointGoogle(string streetAddress, string postalCode)
        {
            string address = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", 
                HttpUtility.UrlEncode(string.Format("{0},{1}", streetAddress, postalCode)));
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(address);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response != null)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responsseStream = response.GetResponseStream())
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(responsseStream);
                        var statusNode = xmlDoc.SelectSingleNode("/GeocodeResponse/status");
                        if (statusNode.InnerText == "OK")
                        {
                            var resultNodes = xmlDoc.SelectNodes("//result/address_component/type[text() = 'postal_code']/../long_name");
                            foreach (XmlNode item in resultNodes)
                            {
                                if (item.InnerText == postalCode)
                                {
                                    var result = item.ParentNode.ParentNode.SelectSingleNode("//geometry/location");
                                    var lat = result.ChildNodes[0].InnerText;
                                    var lng = result.ChildNodes[1].InnerText;
                                    double latitude, longitude;
                                    if (double.TryParse(lat, out latitude) && double.TryParse(lng, out longitude))
                                    {
                                        return new Coordinate(latitude, longitude);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return null;
        }

        //public ICoordinate GeocodePoint(string streetAddress, string postalCode) {
        //    //string latlong = "Unavailable";

        //    //Define the address specification for MWS
        //    MapPointService.Address myAddress = new MapPointService.Address();
        //    myAddress.AddressLine = streetAddress;
        //    myAddress.PostalCode = postalCode;

        //    //Set the search options
        //    MapPointService.FindOptions myFindOptions = new MapPointService.FindOptions();

        //    MapPointService.FindAddressSpecification findAddressSpec = 
        //     new MapPointService.FindAddressSpecification();
        //    findAddressSpec.InputAddress = myAddress;
        //    findAddressSpec.Options = myFindOptions;
        //    findAddressSpec.DataSourceName = "MapPoint.NA";

        //    //Set the ResultMask to include the rooftop geocoding values
        //    findAddressSpec.Options.ResultMask =
        //        MapPointService.FindResultMask.RooftopFlag |
        //        MapPointService.FindResultMask.LatLongFlag |
        //        MapPointService.FindResultMask.EntityFlag |
        //        MapPointService.FindResultMask.MatchDetailsFlag |
        //        MapPointService.FindResultMask.AddressFlag;
        //    try {
        //        //Call the MWS FindAddress method
        //        MapPointService.FindResults myFindResults = _findService.FindAddress(findAddressSpec);

        //        ////Parse the Results
        //        //MapPointService.FindResult bestResult = myFindResults.Results[0];
        //        //int matchcode = 0;

        //        //MapPointService.EntityPropertyValue[] props =
        //        //  bestResult.BestViewableLocation.Entity.Properties;
        //        ////Find MatchedMethod property which tells you whether rooftop was used 
        //        //for (int i = 0; i < props.Length; i++)
        //        //{
        //        //    if (props[i].Name == "MatchedMethod")
        //        //    {
        //        //        matchcode = System.Convert.ToInt32(props[i].Value);
        //        //    }
        //        //}
        //        ////Prepare result 
        //        //if (matchcode == 7)
        //        //{
        //        //    latlong = "new VELatLong(" + bestResult.BestViewableLocation.LatLong.Latitude
        //        //        + ", " + bestResult.BestViewableLocation.LatLong.Longitude + ")";
        //        //}

        //        //return latlong;

        //        MapPointService.FindResult bestResult = myFindResults.Results[0];
        //        return new Coordinate(bestResult.BestViewableLocation.LatLong.Latitude, bestResult.BestViewableLocation.LatLong.Longitude);
        //    } catch(Exception ex) {
        //        ILog logger = LogManager.GetLogger(GetType());
        //        logger.Error("GeocodeService Unhandle Error", ex);
        //        return null;
        //    }
        //}
    }
}
