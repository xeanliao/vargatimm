using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;

namespace GTU.ModelLayer.Device.Send
{
    public class AtGtszi: BaseSend
    {
        private String _password;
        private String _zoneId;
        private String _longitude;
        private String _latitude;
        private String _radius;
        private String _checkInterval;

        /// <summary>
        /// AT+GTSZI Command Constructor
        /// </summary>
        public AtGtszi()
            : base()
        {
            base.CommandName = "AT+GTSZI=";
            _password = "gl100";
            _zoneId = String.Empty;
            _longitude = String.Empty;
            _latitude = String.Empty;
            _radius = String.Empty;
            _checkInterval = String.Empty;
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
        /// Zone ID
        /// </summary>
        public String ZoneId
        {
            get { return _zoneId; }
            set { _zoneId = value; }
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

        
    }
}
