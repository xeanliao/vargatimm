using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtpnaBo : BaseReadBo
    {
        public RespGtpna RespGtpnaModel { get; private set; }

        public RespGtpnaBo()
        {
            RespGtpnaModel = new RespGtpna();
        }

        public RespGtpnaBo(RespGtpna respGtpnaModel)
        {
            RespGtpnaModel = respGtpnaModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTPNA Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtpfa Object</returns>
        public RespGtpna ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtpnaModel.CommandName = dataRead[0];
            RespGtpnaModel.UniqueID = dataRead[1];
            RespGtpnaModel.SendTime = dataRead[2];
            RespGtpnaModel.CountNum = dataRead[3];
            return RespGtpnaModel;
        }
    }
}
