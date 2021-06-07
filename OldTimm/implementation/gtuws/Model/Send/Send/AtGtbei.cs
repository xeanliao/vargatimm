using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;

namespace GTU.ModelLayer.Device.Send
{
    public class AtGtbei : BaseSend
    {
        private String _password;
        private String _reserved1;
        private String _reserved2;
        private Common.CommEnums.LocationByCall _locationByCall;
        private String _secondServerIp;
        private String _secondServerPort;
        private String _secondSmsGateway;

        /// <summary>
        /// AT+GTBEI Command Constructor
        /// </summary>
        public AtGtbei()
            : base()
        {
            base.CommandName = "AT+GTBEI=";
            _password = "gl100";
            _reserved1 = String.Empty;
            _reserved2 = String.Empty;
            _locationByCall = Common.CommEnums.LocationByCall.DisconnectCall;
            _secondServerIp = String.Empty;
            _secondServerPort = String.Empty;
            _secondSmsGateway = String.Empty;
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
        /// Reserved1
        /// </summary>
        public String Reserved1
        {
            get { return _reserved1; }
            set { _reserved1 = value; }
        }

        /// <summary>
        /// Reserved2
        /// </summary>
        public String Reserved2
        {
            get { return _reserved2; }
            set { _reserved2 = value; }
        }

        /// <summary>
        /// Location By Call
        /// </summary>
        public Common.CommEnums.LocationByCall LocationByCall
        {
            get { return _locationByCall; }
            set { _locationByCall = value; }
        }

        /// <summary>
        /// Second Server Ip
        /// </summary>
        public String SecondServerIp
        {
            get { return _secondServerIp; }
            set { _secondServerIp = value; }
        }

        /// <summary>
        /// Second Server Port
        /// </summary>
        public String SecondServerPort
        {
            get { return _secondServerPort; }
            set { _secondServerPort = value; }
        }

        /// <summary>
        /// Second Sms Gateway
        /// </summary>
        public String SecondSmsGateway
        {
            get { return _secondSmsGateway; }
            set { _secondSmsGateway = value; }
        }

        
    }
}
