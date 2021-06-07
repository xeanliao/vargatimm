using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.Utilities.Base
{
    public class BaseRead
    {
        private String _commandName;
        private String _uniqueId;
        private String _sendTime;

        /// <summary>
        /// Constructor of the Base Class of Read DriveModel
        /// </summary>
        public BaseRead() 
        {
            _commandName = String.Empty;
            _uniqueId = String.Empty;
            _sendTime = String.Empty;
        }

        ///// <summary>
        ///// Constructor BaseSend(String commandName, String uniqueId, String sendTime, String ackTime, String countNum)Class
        ///// </summary>
        ///// <param name="commandName">Command Name</param>
        ///// <param name="uniqueId">Unique ID</param>
        ///// <param name="sendTime">Send Time </param>
        //public BaseRead(String commandName, String uniqueId, String sendTime, String ackTime, String countNum)
        //{
        //    this._commandName = commandName;
        //    this._uniqueId = uniqueId;
        //    this._sendTime = sendTime;
        //}

        /// <summary>
        /// Command Name
        /// </summary>
        public String CommandName
        {
            get { return _commandName; }
            set { _commandName = value; }
        }

        /// <summary>
        /// Unique ID
        /// </summary>
        public String UniqueID
        {
            get { return _uniqueId; }
            set { _uniqueId = value; }
        }

        /// <summary>
        /// Send Time
        /// </summary>
        public String SendTime
        {
            get { return _sendTime; }
            set { _sendTime = value; }
        }
        
    }
}
