using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class RespGtinf : BaseRead
    {
        private String _iccid;
        private String _rssi;
        private String _ber;
        private String _batteryLevel;
        private String _chargerConnected;
        private String _countNum;
        private String _ver;


        /// <summary>
        /// +RESP:GTINF Command Constructor
        /// </summary>
        public RespGtinf()
            : base()
        {
            base.CommandName = "+RESP:GTINF";
            _iccid = String.Empty;
            _rssi = String.Empty;
            _ber = String.Empty;
            _batteryLevel = String.Empty;
            _chargerConnected = String.Empty;
            _countNum = String.Empty;
            _ver = String.Empty;
        }

        /// <summary>
        /// Iccid
        /// </summary>
        public String Iccid
        {
            get { return _iccid; }
            set { _iccid = value; }
        }

        /// <summary>
        /// Rssi
        /// </summary>
        public String Rssi
        {
            get { return _rssi; }
            set { _rssi = value; }
        }
        /// <summary>
        /// Ber
        /// </summary>
        public String Ber
        {
            get { return _ber; }
            set { _ber = value; }
        }
        /// <summary>
        /// Battery Level
        /// </summary>
        public String BatteryLevel
        {
            get { return _batteryLevel; }
            set { _batteryLevel = value; }
        }
        /// <summary>
        /// Charger Connected
        /// </summary>
        public String ChargerConnected
        {
            get { return _chargerConnected; }
            set { _chargerConnected = value; }
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
