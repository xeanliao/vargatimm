using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;
using GTU.ModelLayer.Common;

namespace GTU.ModelLayer.Device.Read
{
    public class RespGtall : BaseRead
    {
        private Common.Configuration _configurationContent;
        private String _countNum;


        /// <summary>`
        /// +ACK:GTALL Command Constructor
        /// </summary>
        public RespGtall()
            : base()
        {
            base.CommandName = "+RESP:GTALL";
            _configurationContent = null;
            _countNum = String.Empty;
        }

        /// <summary>
        /// Configuration Content
        /// </summary>
        public Common.Configuration ConfigurationContent
        {
            get { return _configurationContent; }
            set { _configurationContent = value; }
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
