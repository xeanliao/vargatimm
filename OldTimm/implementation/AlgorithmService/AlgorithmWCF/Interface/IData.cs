using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace GTUService.TIMM.Data
{
    [DataContract]
    public class Coordinate
    {
        double dwAltitude;
        double dwLatitude;
        double dwLongitude;

        public Coordinate()
        {
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

    [DataContract]
    public enum PowerInfo
    {
        [EnumMember]
        ON = 0,
        [EnumMember]
        OFF = 1,
        [EnumMember]
        Low = 2,
        [EnumMember]
        UnKnown = 3,
    }

    [DataContract]
    public class GTU
    {

        private string sID;
        private string code = "";
        private Coordinate oCurrentCoordinate = new Coordinate();
        private double dwSpeed = 0;// 0.0 ~ 999.9 KM/h
        private int nHeading = 0; //Azimuth in degrees 
        private DateTime dtSendTime = DateTime.Now;
        private DateTime dtReceivedTime = DateTime.Now;
        private string sIPAddress = "";
        private int nAreaCode = 0; //000 ~ 999
        private int nNetworkCode = 0; // 000 ~ 999
        private int nCellID = 0;
        private PowerInfo ePowerInfo = PowerInfo.UnKnown;
        private long nGPSFix = 0;
        private int nAccuracy = 0;
        private int nCount = 0;
        private int nLocationID = 0;
        private string sVersion = "";

        public GTU()
        {

        }


        [DataMember]
        public String ID
        {
            get { return sID; }
            set { sID = value; }
        }

        [DataMember]
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        [DataMember]
        public int Accuracy
        {
            get { return nAccuracy; }
            set { nAccuracy = value; }
        }


        [DataMember]
        public Coordinate CurrentCoordinate
        {
            get { return oCurrentCoordinate; }
            set { oCurrentCoordinate = value; }
        }

        [DataMember]
        public double Speed
        {
            get { return dwSpeed; }
            set { dwSpeed = value; }
        }

        [DataMember]
        public int Heading
        {
            get { return nHeading; }
            set { nHeading = value; }
        }

        [DataMember]
        public DateTime SendTime
        {
            get { return dtSendTime; }
            set { dtSendTime = value; }
        }

        [DataMember]
        public DateTime ReceivedTime
        {
            get { return dtReceivedTime; }
            set { dtReceivedTime = value; }
        }

        [DataMember]
        public string IPAddress
        {
            get { return sIPAddress; }
            set { sIPAddress = value; }
        }

        [DataMember]
        public int AreaCode
        {
            get { return nAreaCode; }
            set { nAreaCode = value; }
        }
        [DataMember]
        public int NetworkCode
        {
            get { return nNetworkCode; }
            set { nNetworkCode = value; }
        }
        [DataMember]
        public int CellID
        {
            get { return nCellID; }
            set { nCellID = value; }
        }
        [DataMember]
        public long GPSFix
        {
            get { return nGPSFix; }
            set { nGPSFix = value; }
        }

        [DataMember]
        public int Count
        {
            get { return nCount; }
            set { nCount = value; }
        }

        [DataMember]
        public int LocationID
        {
            get { return nLocationID; }
            set { nLocationID = value; }
        }

        [DataMember]
        public string Version
        {
            get { return sVersion; }
            set { sVersion = value; }
        }

        [DataMember]
        public PowerInfo PowerInfo
        {
            get { return ePowerInfo; }
            set { ePowerInfo = value; }
        }

    }

}
