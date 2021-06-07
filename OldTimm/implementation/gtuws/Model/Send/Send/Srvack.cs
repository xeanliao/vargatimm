using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.ModelLayer.Device.Send
{
    public class Srvack
    {
        private String _commandName;
        private String _countNum;

        /// <summary>
        /// +SRVACK Command Constructor
        /// </summary>
        public Srvack()
        {
            _commandName = "+SRVACK:";
            _countNum = String.Empty;
        }

        /// <summary>
        /// Command Name
        /// </summary>
        public String CommandName
        {
            get { return _commandName; }
            set { _commandName = value; }
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
