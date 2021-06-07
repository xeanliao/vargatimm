using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtcidBo : BaseReadBo
    {
        public RespGtcid RespGtcidModel { get; private set; }

        public RespGtcidBo()
        {
            RespGtcidModel = new RespGtcid();
        }

        public RespGtcidBo(RespGtcid respGtcidModel)
        {
            RespGtcidModel = respGtcidModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTCID Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtcbc Object</returns>
        public RespGtcid ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtcidModel.CommandName = dataRead[0];
            RespGtcidModel.UniqueID = dataRead[1];
            RespGtcidModel.Content = dataRead[2];
            RespGtcidModel.SendTime = dataRead[3];
            RespGtcidModel.CountNum = dataRead[4];
            return RespGtcidModel;
        }
    }
}
