using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class AckGtrto : BaseRead
    {
        private String _option;
        private String _ackTime;
        private String _countNum;


        /// <summary>
        /// +ACK:GTRTO Command Constructor
        /// </summary>
        public AckGtrto()
            : base()
        {
            base.CommandName = "+ACK:GTRTO";
            _option = String.Empty;
            _ackTime = String.Empty;
            _countNum = String.Empty;
        }

        //public AckGtrto(String commandName, String uniqueId, String sendTime, String ackTime, String countNum)
        //    : base(commandName, uniqueId, sendTime, ackTime, countNum)
        //{
        //    base.CommandName = "+ACK:GTRTO";
        //    _option = String.Empty;
        //    _ackTime = ackTime;
        //    _countNum = countNum;
        //}

        public String Option
        {
            get { return _option; }
            set { _option = value; }
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
