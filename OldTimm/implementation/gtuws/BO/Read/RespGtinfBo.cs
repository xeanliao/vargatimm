using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtinfBo : BaseReadBo
    {
        public RespGtinf RespGtinfModel { get; private set; }

        public RespGtinfBo()
        {
            RespGtinfModel = new RespGtinf();
        }

        public RespGtinfBo(RespGtinf respGtinfModel)
        {
            RespGtinfModel = respGtinfModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTINF Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtinf Object</returns>
        public RespGtinf ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);
            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtinfModel.CommandName = dataRead[0];
            RespGtinfModel.UniqueID = dataRead[1];
            RespGtinfModel.Iccid = dataRead[2];
            RespGtinfModel.Rssi = dataRead[3];
            RespGtinfModel.Ber = dataRead[4];
            RespGtinfModel.BatteryLevel = dataRead[5];
            RespGtinfModel.ChargerConnected = dataRead[6];
            RespGtinfModel.SendTime = dataRead[7];
            RespGtinfModel.CountNum = dataRead[8];
            RespGtinfModel.Ver = dataRead[9];
            return RespGtinfModel;
        }
    }
}
