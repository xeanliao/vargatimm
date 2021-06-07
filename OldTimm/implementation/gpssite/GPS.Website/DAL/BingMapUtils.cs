using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using GPS.Website.BingGeocodeService;

namespace GPS.Website.DAL
{
    public class BingMapUtils
    {
        public static Coordinate GetCoordinate(string sAddress)
        { 
            // address can be "912 Joaquin rd, 91007", or "912 Joaquin rd, arcadia, ca"
            GeocodeRequest request = new GeocodeRequest();

            // Set the credentials using a valid Bing Maps key
            request.Credentials = new Credentials();
            request.Credentials.ApplicationId = "AvKr95P5eEx2mhj5vfZbh1-0VVXPY9BzNvwGkTsIvoCiIIagK3n106lMjoOjp__u";

            // Set the full address query
            request.Query = sAddress;

            // Set the options to only return high confidence results 
            ConfidenceFilter[] filters = new ConfidenceFilter[1];
            filters[0] = new ConfidenceFilter();
            filters[0].MinimumConfidence = Confidence.High;

            // Add the filters to the options
            GeocodeOptions geocodeOptions = new GeocodeOptions();
            geocodeOptions.Filters = filters;
            request.Options = geocodeOptions;

            // Make the geocode request
            GeocodeService geo = new GeocodeService();
            GeocodeResponse response = geo.Geocode(request);

            if (response.Results.Count() == 0)
                throw new Exception("Address can not be verified, please try again.");

            double latitude = response.Results[0].Locations[0].Latitude;
            double longitude = response.Results[0].Locations[0].Longitude;
            return new Coordinate(latitude, longitude);
        }

    }
}