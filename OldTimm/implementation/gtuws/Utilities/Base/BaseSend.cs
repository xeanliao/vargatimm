using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GTU.Utilities.Base
{
    public class BaseSend
    {
        private String _commandName;
        private String _sendTime;

        /// <summary>
        /// Constructor of the Base Class of Send DriveModel
        /// </summary>
        public BaseSend()
        {
            _commandName = String.Empty;
            _sendTime = String.Empty;
        }

        public String CommandName
        {
            get { return _commandName; }
            set { _commandName = value; }
        }

        public String SendTime
        {
            get { return _sendTime; }
            set { _sendTime = value; }
        }
    }
}