﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class RespGtcbc : BaseRead
    {
        private String _content;
        private String _countNum;

        /// <summary>
        /// +RESP:GTCBC Command Constructor
        /// </summary>
        public RespGtcbc()
            : base()
        {
            base.CommandName = "+RESP:GTCBC";
            _content = String.Empty;
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
        /// Count Num
        /// </summary>
        public String CountNum
        {
            get { return _countNum; }
            set { _countNum = value; }
        }

        
    }
}
