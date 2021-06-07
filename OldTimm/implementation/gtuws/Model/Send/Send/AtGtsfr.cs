using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;

namespace GTU.ModelLayer.Device.Send
{
    public class AtGtsfr : BaseSend
    {
        private String _password;
        private Common.CommEnums.PowerKeyEnable _powerKeyEnable;
        private Common.CommEnums.FunctionKeyEnable _functionKeyEnable;
        private Common.CommEnums.FunctionKeyMode _functionKeyMode;
        private String _reserved;
        private Common.CommEnums.MovementDetectMode _movementDetectMode;
        private String _movementSpeed;
        private String _movementDistance;
        private String _movementSendNumber;
        private Common.CommEnums.FullChargeBoot _fullChargeBoot;

        /// <summary>
        /// AT+GTSFR Command Constructor
        /// </summary>
        public AtGtsfr()
            : base()
        {
            base.CommandName = "AT+GTSFR=";
            _password = "gl100";
            _powerKeyEnable = Common.CommEnums.PowerKeyEnable.Enable;
            _functionKeyEnable = Common.CommEnums.FunctionKeyEnable.Enable;
            _functionKeyMode = Common.CommEnums.FunctionKeyMode.GeoFenceMode;
            _reserved = String.Empty;
            _movementDetectMode = Common.CommEnums.MovementDetectMode.Disable;
            _movementSpeed = String.Empty;
            _movementDistance = String.Empty;
            _movementSendNumber = "5";
            _fullChargeBoot = Common.CommEnums.FullChargeBoot.FullyPowerOn;
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
        /// Power Key Enable
        /// </summary>
        public Common.CommEnums.PowerKeyEnable PowerKeyEnable
        {
            get { return _powerKeyEnable; }
            set { _powerKeyEnable = value; }
        }

        /// <summary>
        /// Function Key Enable
        /// </summary>
        public Common.CommEnums.FunctionKeyEnable FunctionKeyEnable
        {
            get { return _functionKeyEnable; }
            set { _functionKeyEnable = value; }
        }

        /// <summary>
        /// Function Key Mode
        /// </summary>
        public Common.CommEnums.FunctionKeyMode FunctionKeyMode
        {
            get { return _functionKeyMode; }
            set { _functionKeyMode = value; }
        }

        /// <summary>
        /// Reserved
        /// </summary>
        public String Reserved
        {
            get { return _reserved; }
            set { _reserved = value; }
        }

        /// <summary>
        /// Movement Detect Mode
        /// </summary>
        public Common.CommEnums.MovementDetectMode MovementDetectMode
        {
            get { return _movementDetectMode; }
            set { _movementDetectMode = value; }
        }

        /// <summary>
        /// Movement Speed
        /// </summary>
        public String MovementSpeed
        {
            get { return _movementSpeed; }
            set { _movementSpeed = value; }
        }

        /// <summary>
        /// Movement Distance
        /// </summary>
        public String MovementDistance
        {
            get { return _movementDistance; }
            set { _movementDistance = value; }
        }

        /// <summary>
        /// Movement Send Number
        /// </summary>
        public String MovementSendNumber
        {
            get { return _movementSendNumber; }
            set { _movementSendNumber = value; }
        }

        /// <summary>
        /// Full Charge Boot
        /// </summary>
        public Common.CommEnums.FullChargeBoot FullChargeBoot
        {
            get { return _fullChargeBoot; }
            set { _fullChargeBoot = value; }
        }

        
    }
}
