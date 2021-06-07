using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;

namespace GTU.ModelLayer.Device.Send
{
    public class AtGtrto : BaseSend
    {
        private String _password;
        private Common.CommEnums.CommandOption _commandOption;
        private Common.CommEnums.SpeedWarnEnable _speedWarmEnable;
        private String _speedWarmValue;

        /// <summary>
        /// AT+GTRTO Command Constructor
        /// </summary>
        public AtGtrto()
            : base()
        {
            base.CommandName = "AT+GTRTO=";
            _password = "gl100";
            _commandOption = Common.CommEnums.CommandOption.GetLatestTimeofGPSFixingSuccessfully;
            _speedWarmEnable = Common.CommEnums.SpeedWarnEnable.Disable;
            _speedWarmValue = String.Empty;
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
        /// Command Option
        /// </summary>
        public Common.CommEnums.CommandOption CommandOption
        {
            get { return _commandOption; }
            set { _commandOption = value; }
        }

        /// <summary>
        /// Speed Warm Enable
        /// </summary>
        public Common.CommEnums.SpeedWarnEnable SpeedWarmEnable
        {
            get { return _speedWarmEnable; }
            set { _speedWarmEnable = value; }
        }

        /// <summary>
        /// Speed Warm Value
        /// </summary>
        public String SpeedWarmValue
        {
            get { return _speedWarmValue; }
            set { _speedWarmValue = value; }
        }

        
    }
}
