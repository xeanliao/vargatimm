using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.ModelLayer.Common;

namespace GTU.ModelLayer.Device.Send
{
    public class AtGtgeo : BaseSend
    {
        private String _password;
        private String _geofenceId;
        private String _longitude;
        private String _latitude;
        private String _radius;
        private String _checkInterval;
        private Common.CommEnums.GeoFenceType _geofenceType;

        /// <summary>
        /// AT+GTGEO Command Constructor
        /// </summary>
        public AtGtgeo()
            : base()
        {
            base.CommandName = "AT+GTGEO=";
            _password = "gl100";
            _geofenceId = String.Empty;
            _longitude = String.Empty;
            _latitude = String.Empty;
            _radius = String.Empty;
            _checkInterval = String.Empty;
            _geofenceType = Common.CommEnums.GeoFenceType.ReportWhenEntersGeoFence;
        }

        /// <summary>
        /// Password
        /// </summary>
        public String Password
        {
            get { return _password; }
            set { _password = value; }
        }
        /// <summary>
        /// Geofence ID
        /// </summary>
        public String GeofenceId
        {
            get { return _geofenceId; }
            set { _geofenceId = value; }
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
        /// Radius
        /// </summary>
        public String Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        /// <summary>
        /// Check Interval
        /// </summary>
        public String CheckInterval
        {
            get { return _checkInterval; }
            set { _checkInterval = value; }
        }
        /// <summary>
        /// Geofence Type
        /// </summary>
        public Common.CommEnums.GeoFenceType GeofenceType
        {
            get { return _geofenceType; }
            set { _geofenceType = value; }
        }

        
    }
}
