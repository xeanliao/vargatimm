using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class AckGttri : BaseRead
    {
        private String _ackTime;
        private String _countNum;


        /// <summary>
        /// +ACK:GTTRI Command Constructor
        /// </summary>
        public AckGttri()
            : base()
        {
            base.CommandName = "+ACK:GTTRI";
            _ackTime = String.Empty;
            _countNum = String.Empty;
        }

        //public AckGttri(String commandName, String uniqueId, String sendTime, String ackTime, String countNum)
        //    : base(commandName, uniqueId, sendTime, ackTime, countNum)
        //{
        //    base.CommandName = "+ACK:GTTRI";
        //    this._ackTime = ackTime;
        //    this._countNum = countNum;
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

        
    }
}
