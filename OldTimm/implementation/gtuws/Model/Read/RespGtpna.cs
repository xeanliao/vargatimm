using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class RespGtpna : BaseRead
    {
        private String _countNum;

        /// <summary>
        /// +RESP:GTPNA Command Constructor
        /// </summary>
        public RespGtpna()
            : base()
        {
            base.CommandName = "+RESP:GTPNA";
            _countNum = String.Empty;
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
