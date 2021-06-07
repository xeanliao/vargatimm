using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtswvBo : BaseReadBo
    {
        public RespGtswv RespGtswvModel { get; private set; }

        public RespGtswvBo()
        {
            RespGtswvModel = new RespGtswv();
        }

        public RespGtswvBo(RespGtswv respGtswvModel)
        {
            RespGtswvModel = respGtswvModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTSWV Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtswv Object</returns>
        public RespGtswv ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtswvModel.CommandName = dataRead[0];
            RespGtswvModel.UniqueID = dataRead[1];
            RespGtswvModel.Content1 = dataRead[2];
            RespGtswvModel.Content2 = dataRead[3];
            RespGtswvModel.SendTime = dataRead[4];
            RespGtswvModel.CountNum = dataRead[5];
            return RespGtswvModel;
        }
    }
}
