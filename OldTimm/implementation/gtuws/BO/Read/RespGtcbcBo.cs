using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtcbcBo : BaseReadBo
    {
        public RespGtcbc RespGtcbcModel { get; private set; }

        public RespGtcbcBo()
        {
            RespGtcbcModel = new RespGtcbc();
        }

        public RespGtcbcBo(RespGtcbc respGtcbcModel)
        {
            RespGtcbcModel = respGtcbcModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTCBC Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtcbc Object</returns>
        public RespGtcbc ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtcbcModel.CommandName = dataRead[0];
            RespGtcbcModel.UniqueID = dataRead[1];
            RespGtcbcModel.Content = dataRead[2];
            RespGtcbcModel.SendTime = dataRead[3];
            RespGtcbcModel.CountNum = dataRead[4];
            return RespGtcbcModel;
        }
    }
}
