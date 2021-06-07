using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;

namespace GTU.ModelLayer.Device.Send
{
    public class AtGttri : BaseSend
    {
        private String _password;
        private String _beginTime;
        private String _endTime;
        private String _sendInterval;
        private String _fixInterval;

        /// <summary>
        /// AT+GTTRI= Command Constructor
        /// </summary>
        public AtGttri()
            : base()
        {
            base.CommandName = "AT+GTTRI=";
            _password = "gl100";
            _beginTime = "0000";
            _endTime = "0000";
            _sendInterval = "0";
            _fixInterval = "0";
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
        /// Begin Time
        /// </summary>
        public String BeginTime
        {
            get { return _beginTime; }
            set { _beginTime = value; }
        }
        /// <summary>
        /// End Time
        /// </summary>
        public String EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }
        /// <summary>
        /// Send Interval
        /// </summary>
        public String SendInterval
        {
            get { return _sendInterval; }
            set { _sendInterval = value; }
        }
        /// <summary>
        /// Fix Interval
        /// </summary>
        public String FixInterval
        {
            get { return _fixInterval; }
            set { _fixInterval = value; }
        }

        
    }
}
