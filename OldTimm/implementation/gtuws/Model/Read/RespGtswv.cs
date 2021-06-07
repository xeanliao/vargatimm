using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class RespGtswv : BaseRead
    {
        private String _content1;
        private String _content2;
        private String _countNum;

        /// <summary>
        /// +ACK:GTACK Command Constructor
        /// </summary>
        public RespGtswv()
            : base()
        {
            base.CommandName = "+RESP:GTSWV";
            _content1 = String.Empty;
            _content2 = String.Empty;
            _countNum = String.Empty;
        }

        /// <summary>
        /// Content1
        /// </summary>
        public String Content1
        {
            get { return _content1; }
            set { _content1 = value; }
        }
        /// <summary>
        /// Content2
        /// </summary>
        public String Content2
        {
            get { return _content2; }
            set { _content2 = value; }
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
