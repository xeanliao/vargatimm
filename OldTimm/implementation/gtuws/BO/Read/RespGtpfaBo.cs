using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtpfaBo : BaseReadBo
    {
        public RespGtpfa RespGtpfaModel { get; private set; }

        public RespGtpfaBo()
        {
            RespGtpfaModel = new RespGtpfa();
        }

        public RespGtpfaBo(RespGtpfa respGtpfaModel)
        {
            RespGtpfaModel = respGtpfaModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTPFA Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtpfa Object</returns>
        public RespGtpfa ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtpfaModel.CommandName = dataRead[0];
            RespGtpfaModel.UniqueID = dataRead[1];
            RespGtpfaModel.SendTime = dataRead[2];
            RespGtpfaModel.CountNum = dataRead[3];
            return RespGtpfaModel;
        }
    }
}
