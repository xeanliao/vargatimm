using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;

namespace GTU.ModelLayer.Device.Send
{
    public class RespGthbd : BaseSend
    {
        private String _content;
        private String _uniqueId;
        private String _countNum;

        /// <summary>
        /// AT+GTRTO Command Constructor
        /// </summary>
        public RespGthbd()
            : base()
        {
            base.CommandName = "+RESP:GTHBD";
            _content = String.Empty; ;
            _uniqueId = String.Empty;
            _countNum = String.Empty;
        }

        /// <summary>
        /// Content
        /// </summary>
        public String Content
        {
            get { return _content; }
            set { _content = value; }
        }
        /// <summary>
        /// Unique ID
        /// </summary>
        public String UniqueId
        {
            get { return _uniqueId; }
            set { _uniqueId = value; }
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
