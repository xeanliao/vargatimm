using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class RespGtrtl : BaseRead
    {
        private String _number;
        private String _zoneId;
        private String _zoneAlert;
        private String _gpsFix;
        private String _speed;
        private String _heading;
        private String _altitude;
        private String _gpsAccuracy;
        private String _longitude;
        private String _latitude;
        private String _mcc;
        private String _mnc;
        private String _lac;
        private String _cellid;
        private String _ta;
        private String _countNum;


        /// <summary>
        /// +RESP:GTRTL Command Constructor
        /// </summary>
        public RespGtrtl()
            : base()
        {
            base.CommandName = "+RESP:GTRTL";
            _number = String.Empty;
            _zoneId = String.Empty;
            _zoneAlert = String.Empty;
            _gpsFix = String.Empty;
            _speed = String.Empty;
            _heading = String.Empty;
            _altitude = String.Empty;
            _gpsAccuracy = String.Empty;
            _longitude = String.Empty;
            _latitude = String.Empty;
            _mcc = String.Empty;
            _mnc = String.Empty;
            _lac = String.Empty;
            _cellid = String.Empty;
            _ta = String.Empty;
            _countNum = String.Empty;
        }

        /// <summary>
        /// Number
        /// </summary>
        public String Number
        {
            get { return _number; }
            set { _number = value; }
        }

        /// <summary>
        /// Zone ID
        /// </summary>
        public String ZoneId
        {
            get { return _zoneId; }
            set { _zoneId = value; }
        }

        /// <summary>
        /// Zone Alert
        /// </summary>
        public String ZoneAlert
        {
            get { return _zoneAlert; }
            set { _zoneAlert = value; }
        }

        /// <summary>
        /// Gps Fix
        /// </summary>
        public String GpsFix
        {
            get { return _gpsFix; }
            set { _gpsFix = value; }
        }
        /// <summary>
        /// Speed
        /// </summary>
        public String Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        /// <summary>
        /// Heading
        /// </summary>
        public String Heading
        {
            get { return _heading; }
            set { _heading = value; }
        }
        /// <summary>
        /// Altitude
        /// </summary>
        public String Altitude
        {
            get { return _altitude; }
            set { _altitude = value; }
        }
        /// <summary>
        /// Gps Accuracy
        /// </summary>
        public String GpsAccuracy
        {
            get { return _gpsAccuracy; }
            set { _gpsAccuracy = value; }
        }
        /// <summary>
        /// Longitude
        /// </summary>
        public String Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }
        /// <summary>
        /// Latitude
        /// </summary>
        public String Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }
        /// <summary>
        /// Mcc
        /// </summary>
        public String Mcc
        {
            get { return _mcc; }
            set { _mcc = value; }
        }
        /// <summary>
        /// Mnc
        /// </summary>
        public String Mnc
        {
            get { return _mnc; }
            set { _mnc = value; }
        }
        /// <summary>
        /// Lac
        /// </summary>
        public String Lac
        {
            get { return _lac; }
            set { _lac = value; }
        }
        /// <summary>
        /// Cellid
        /// </summary>
        public String Cellid
        {
            get { return _cellid; }
            set { _cellid = value; }
        }
        /// <summary>
        /// Ta
        /// </summary>
        public String Ta
        {
            get { return _ta; }
            set { _ta = value; }
        }
        /// <summary>
        /// Count Num
        /// </summary>
        public String CountNum
        {
            get { return _countNum; }
            set { _countNum = value; }
        }

        
    }
}
