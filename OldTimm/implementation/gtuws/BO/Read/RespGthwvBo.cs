using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGthwvBo : BaseReadBo
    {
        public RespGthwv RespGthwvModel { get; private set; }

        public RespGthwvBo()
        {
            RespGthwvModel = new RespGthwv();
        }

        public RespGthwvBo(RespGthwv respGthwvModel)
        {
            RespGthwvModel = respGthwvModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTHWV Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGthwv Object</returns>
        public RespGthwv ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGthwvModel.CommandName = dataRead[0];
            RespGthwvModel.UniqueID = dataRead[1];
            RespGthwvModel.Content = dataRead[2];
            RespGthwvModel.SendTime = dataRead[3];
            RespGthwvModel.CountNum = dataRead[4];
            return RespGthwvModel;
        }
    }
}
