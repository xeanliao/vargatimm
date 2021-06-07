using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Common;

namespace GTU.ModelLayer.Track
{
    public class TrackItem
    {
        private int _trackId;
        private String _uniqueId;
        private String _longitude;
        private String _latitude;
        private DateTime _sendTime;
        private bool _outofDeliverable;
        private bool _inNonDeliverable;
        private bool _stayAlarm;

        /// <summary>
        /// TrackId
        /// </summary>
        public int TrackId
        {
            get { return _trackId; }
            set { _trackId = value; }
        }

        /// <summary>
        /// UniqueId
        /// </summary>
        public String UniqueId
        {
            get { return _uniqueId; }
            set { _uniqueId = value; }
        }

        /// <summary>
        /// longitude
        /// </summary>
        public String longitude
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
        /// SendTime
        /// </summary>
        public DateTime SendTime
        {
            get { return _sendTime; }
            set { _sendTime = value; }
        }

        /// <summary>
        /// OutofDeliverable
        /// </summary>
        public bool InDeliverable
        {
            get { return _outofDeliverable; }
            set { _outofDeliverable = value; }
        }

        /// <summary>
        /// longitude
        /// </summary>
        public bool InNonDeliverable
        {
            get { return _inNonDeliverable; }
            set { _inNonDeliverable = value; }
        }

        /// <summary>
        /// longitude
        /// </summary>
        public bool StayAlarm
        {
            get { return _stayAlarm; }
            set { _stayAlarm = value; }
        }

        /// <summary>
        /// Valide Track Area
        /// </summary>
        public Coordinate[] TrackArea
        {
            get;
            set;
        }

        // <summary>
        /// Invalide Track Area
        /// </summary>
        public Coordinate[] InvalidArea
        {
            get;
            set;
        }

        // <summary>
        /// Invalide Track Address
        /// </summary>
        public Address[] InvalidAddress
        {
            get;
            set;
        }
    }
}
