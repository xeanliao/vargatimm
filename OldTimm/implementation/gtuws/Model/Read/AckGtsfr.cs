using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class AckGtsfr : BaseRead
    {
        private String _ackTime;
        private String _countNum;
        private String _ver;

        /// <summary>
        /// +ACK:GTSFR Command Constructor
        /// </summary>
        public AckGtsfr()
            : base()
        {
            base.CommandName = "+ACK:GTSFR";
            _ackTime = String.Empty;
            _countNum = String.Empty;
            _ver = String.Empty;
        }

        //public AckGtsfr(String commandName, String uniqueId, String sendTime, String ackTime, String countNum)
        //    : base(commandName, uniqueId, sendTime, ackTime, countNum)
        //{
        //    base.CommandName = "+ACK:GTSFR";
        //    this._ackTime = ackTime;
        //}

        /// <summary>
        /// ACK Time
        /// </summary>
        public String ACKTime
        {
            get { return _ackTime; }
            set { _ackTime = value; }
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
