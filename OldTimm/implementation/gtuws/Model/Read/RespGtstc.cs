using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class RespGtstc : BaseRead
    {
        private String _countNum;

        /// <summary>
        /// +RESP:GTSTC Command Constructor
        /// </summary>
        public RespGtstc()
            : base()
        {
            base.CommandName = "+RESP:GTSTC";
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
