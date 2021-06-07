using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;

namespace GTU.ModelLayer.Device.Send
{
    public class AtGtack : BaseSend
    {
        private String _password;
        private Common.CommEnums.AckType _ackType;
        private String _ackValue;

        /// <summary>
        /// AT+GTACK Command Constructor
        /// </summary>
        public AtGtack()
            : base()
        {
            base.CommandName = "AT+GTACK=";
            _password = "gl100";
            _ackType = Common.CommEnums.AckType.TCPPacket;
            _ackValue = "1";
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
        /// Ack Type 
        /// </summary>
        public Common.CommEnums.AckType AckType
        {
            get { return _ackType; }
            set { _ackType = value; }
        }
        /// <summary>
        /// Ack Value
        /// </summary>
        public String AckValue
        {
            get { return _ackValue; }
            set { _ackValue = value; }
        }

        
    }
}
