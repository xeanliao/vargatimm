using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;

namespace GTU.ModelLayer.Device.Send
{
    public class AtGtsri: BaseSend
    {
        private String _password;
        private Common.CommEnums.ReportMode _reportMode;
        private Common.CommEnums.ActiveSession _activeSession;
        private String _apn;
        private String _apnUserName;
        private String _apnUserPassword;
        private String _mainServerIP;
        private String _mainServerPort;
        private String _mainGateway;

        /// <summary>
        /// AT+GTSRI Command Constructor
        /// </summary>
        public AtGtsri(): base()
        {
            base.CommandName = "AT+GTSRI=";
            _password = "gl100";
            _reportMode = Common.CommEnums.ReportMode.DefaultOnGPRS;
            _activeSession = Common.CommEnums.ActiveSession.TCPReconnectMode;
            _apn = String.Empty;
            _apnUserName = String.Empty;
            _apnUserPassword = String.Empty;
            _mainServerIP = String.Empty;
            _mainServerPort = String.Empty;
            _mainGateway = String.Empty;
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
        /// Report Mode
        /// </summary>
        public Common.CommEnums.ReportMode ReportMode
        {
            get { return _reportMode; }
            set { _reportMode = value; }
        }

        /// <summary>
        /// Active Session
        /// </summary>
        public Common.CommEnums.ActiveSession ActiveSession
        {
            get { return _activeSession; }
            set { _activeSession = value; }
        }

        /// <summary>
        /// Apn
        /// </summary>
        public String Apn
        {
            get { return _apn; }
            set { _apn = value; }
        }

        /// <summary>
        /// Apn User Name
        /// </summary>
        public String ApnUserName
        {
            get { return _apnUserName; }
            set { _apnUserName = value; }
        }

        /// <summary>
        /// Apn User Password
        /// </summary>
        public String ApnUserPassword
        {
            get { return _apnUserPassword; }
            set { _apnUserPassword = value; }
        }

        /// <summary>
        /// Main Server IP
        /// </summary>
        public String MainServerIP
        {
            get { return _mainServerIP; }
            set { _mainServerIP = value; }
        }

        /// <summary>
        /// Main Server Port
        /// </summary>
        public String MainServerPort
        {
            get { return _mainServerPort; }
            set { _mainServerPort = value; }
        }

        /// <summary>
        /// Main SMS Gateway
        /// </summary>
        public String MainSMSGateway
        {
            get { return _mainGateway; }
            set { _mainGateway = value; }
        }

        
        
    }
}
