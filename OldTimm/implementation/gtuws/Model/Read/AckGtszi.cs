using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class AckGtszi : BaseRead
    {
        private String _zoneId;
        private String _ackTime;
        private String _countNum;

        /// <summary>
        /// +ACK:GTSZI Command Constructor
        /// </summary>
        public AckGtszi()
            : base()
        {
            base.CommandName = "+ACK:GTSZI";
            _zoneId = String.Empty;
            _ackTime = String.Empty;
            _countNum = String.Empty;
        }

        //public AckGtszi(String commandName, String uniqueId, String sendTime, String ackTime, String countNum)
        //    : base(commandName, uniqueId, sendTime, ackTime, countNum)
        //{
        //    base.CommandName = "+ACK:GTSZI";
        //    _zoneId = String.Empty;
        //    this._ackTime = ackTime;
        //    this._countNum = countNum;
        //}

        /// <summary>
        /// Zone ID
        /// </summary>
        public String ZoneId
        {
            get { return _zoneId; }
            set { _zoneId = value; }
        }

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

        
    }
}
