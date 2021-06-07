using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class AckGtgeo : BaseRead
    {
        private String _geofenceId;
        private String _ackTime;
        private String _countNum;
        private String _ver;

        /// <summary>
        /// +ACK:GTGEO Command Constructor
        /// </summary>
        public AckGtgeo()
            : base()
        {
            base.CommandName = "+ACK:GTGEO";
            _geofenceId = String.Empty;
            _ackTime = String.Empty;
            _countNum = String.Empty;
            _ver = String.Empty;
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
        /// ACK Time
        /// </summary>
        public String ACKTime
        {
            get { return _ackTime; }
            set { _ackTime = value; }
        }

        /// <summary>
        /// Count Num
        /// </summary>
        public String CountNum
        {
            get { return _countNum; }
            set { _countNum = value; }
        }

        /// <summary>
        /// Ver
        /// </summary>
        public String Ver
        {
            get { return _ver; }
            set { _ver = value; }
        }

        
    }
}
