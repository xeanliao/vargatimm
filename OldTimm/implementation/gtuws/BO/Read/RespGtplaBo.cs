using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtplaBo : BaseReadBo
    {
        public RespGtpla RespGtplaModel { get; private set; }

        public RespGtplaBo()
        {
            RespGtplaModel = new RespGtpla();
        }

        public RespGtplaBo(RespGtpla respGtplaModel)
        {
            RespGtplaModel = respGtplaModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTPLA Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtpla Object</returns>
        public RespGtpla ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtplaModel.CommandName = dataRead[0];
            RespGtplaModel.UniqueID = dataRead[1];
            RespGtplaModel.SendTime = dataRead[2];
            RespGtplaModel.CountNum = dataRead[3];
            return RespGtplaModel;
        }
    }
}
