using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class AckGtbei : BaseRead
    {
        private String _ackTime;
        private String _countNum;


        /// <summary>
        /// +ACK:GTBEI Command Constructor
        /// </summary>
        public AckGtbei()
            : base()
        {
            base.CommandName = "+ACK:GTBEI";
            _ackTime = String.Empty;
            _countNum = String.Empty;
        }

        //public AckGtbei(String commandName, String uniqueId, String sendTime)
        //    : base(commandName, uniqueId, sendTime)
        //{
        //    base.CommandName = "+ACK:GTBEI";
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
