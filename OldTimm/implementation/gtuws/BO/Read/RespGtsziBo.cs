using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtsziBo : BaseReadBo
    {
        public RespGtszi RespGtsziModel { get; private set; }

        public RespGtsziBo()
        {
            RespGtsziModel = new RespGtszi();
        }

        public RespGtsziBo(RespGtszi respGtsziModel)
        {
            RespGtsziModel = respGtsziModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTSZI Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtszi Object</returns>
        public RespGtszi ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtsziModel.CommandName = dataRead[0];
            RespGtsziModel.UniqueID = dataRead[1];
            RespGtsziModel.Number = dataRead[2];
            RespGtsziModel.ZoneId = dataRead[3];
            RespGtsziModel.ZoneAlert = dataRead[4];
            RespGtsziModel.GpsFix = dataRead[5];
            RespGtsziModel.Speed = dataRead[6];
            RespGtsziModel.Heading = dataRead[7];
            RespGtsziModel.Altitude = dataRead[8];
            RespGtsziModel.GpsAccuracy = dataRead[9];
            RespGtsziModel.Longitude = dataRead[10];
            RespGtsziModel.Latitude = dataRead[11];
            RespGtsziModel.SendTime = dataRead[12];
            RespGtsziModel.Mcc = dataRead[13];
            RespGtsziModel.Mnc = dataRead[14];
            RespGtsziModel.Lac = dataRead[15];
            RespGtsziModel.Cellid = dataRead[16];
            RespGtsziModel.Ta = dataRead[17];
            RespGtsziModel.CountNum = dataRead[18];
            return RespGtsziModel;
        }
    }
}
