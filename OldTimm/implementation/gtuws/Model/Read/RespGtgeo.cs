using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class RespGtgeo : BaseRead
    {
        private String _active;
        private String _countNum;

        /// <summary>
        /// +RESP:GTGEO Command Constructor
        /// </summary>
        public RespGtgeo()
            : base()
        {
            base.CommandName = "+RESP:GTGEO";
            _active = String.Empty;
            _countNum = String.Empty;
        }

        /// <summary>
        /// Active
        /// </summary>
        public String Active
        {
            get { return _active; }
            set { _active = value; }
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
