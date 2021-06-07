using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.ModelLayer.Device.Read
{
    public class RespGtlgt : BaseRead
    {
        private String _content;
        private String _countNum;

        /// <summary>
        /// +RESP:GTLGT Command Constructor
        /// </summary>
        public RespGtlgt()
            : base()
        {
            base.CommandName = "+RESP:GTLGT";
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

        /// <summary>
        /// Resolve +RESP:GTLGT Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtlgt Object</returns>
        public RespGtlgt ResolveCommand(Byte[] byteStr)
        {
            RespGtlgt respGtlgt = new RespGtlgt();
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            respGtlgt.CommandName = dataRead[0];
            respGtlgt.UniqueID = dataRead[1];
            respGtlgt.Content = dataRead[2];
            respGtlgt.SendTime = dataRead[3];
            respGtlgt.CountNum = dataRead[4];
            return respGtlgt;
        }

    }
}
