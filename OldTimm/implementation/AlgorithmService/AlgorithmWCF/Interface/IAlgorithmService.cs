using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WHYTAlgorithmService.Geo
{

    using Area = List<Coordinate>;

    [ServiceContract]
    public interface IGeofencing
    {
        [OperationContract]
        bool IsInTheArea( Coordinate oLocation, string sGUID );

        [OperationContract]
        string IsInTheDNDArea(Coordinate oLocation, List<int> ndAreaIds);
   
        /// <summary>
        /// Return the GUID if sucessed
        /// </summary>
        /// <param name="oArea"></param>
        /// <returns></returns>
        [OperationContract]
        string RegisterArea(Area oArea);

        [OperationContract]
        bool UpdateArea(string sGUID, Area oArea);

        [OperationContract]
        bool RemoveArea(string sGUID);

    }


    [DataContract]
    public class Coordinate
    {
        double dwAltitude;
        double dwLatitude;
        double dwLongitude;

        public Coordinate()
        {
        }

        public Coordinate(double dwLatitudeIn, double dwLongitudeIn)
        {
            dwLatitude = dwLatitudeIn;
            dwLongitude = dwLongitudeIn;
            dwAltitude = 0;
        }

        public Coordinate(Coordinate oCoordinate)
        {
            dwLatitude = oCoordinate.Latitude;
            dwLongitude = oCoordinate.Longitude;
            dwAltitude = oCoordinate.Altitude;
        }

        [DataMember]
        public double Latitude
        {
            get { return dwLatitude; }
            set { dwLatitude = value; }
        }

        [DataMember]
        public double Longitude
        {
            get { return dwLongitude; }
            set { dwLongitude = value; }
        }

        [DataMember]
        public double Altitude
        {
            get { return dwAltitude; }
            set { dwAltitude = value; }
        }

    }


}
