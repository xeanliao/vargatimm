using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class RespGtpla : BaseRead
    {
        private String _countNum;

        /// <summary>
        /// +RESP:GTPLA Command Constructor
        /// </summary>
        public RespGtpla()
            : base()
        {
            base.CommandName = "+RESP:GTPLA";
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
