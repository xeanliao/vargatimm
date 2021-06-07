using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtestBo : BaseReadBo
    {
        public RespGtest RespGtestModel { get; private set; }

        public RespGtestBo()
        {
            RespGtestModel = new RespGtest();
        }

        public RespGtestBo(RespGtest respGtestModel)
        {
            RespGtestModel = respGtestModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTEST Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtest Object</returns>
        public RespGtest ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtestModel.CommandName = dataRead[0];
            RespGtestModel.UniqueID = dataRead[1];
            RespGtestModel.Number = dataRead[2];
            RespGtestModel.ZoneId = dataRead[3];
            RespGtestModel.ZoneAlert = dataRead[4];
            RespGtestModel.GpsFix = dataRead[5];
            RespGtestModel.Speed = dataRead[6];
            RespGtestModel.Heading = dataRead[7];
            RespGtestModel.Altitude = dataRead[8];
            RespGtestModel.GpsAccuracy = dataRead[9];
            RespGtestModel.Longitude = dataRead[10];
            RespGtestModel.Latitude = dataRead[11];
            RespGtestModel.SendTime = dataRead[12];
            RespGtestModel.Mcc = dataRead[13];
            RespGtestModel.Mnc = dataRead[14];
            RespGtestModel.Lac = dataRead[15];
            RespGtestModel.Cellid = dataRead[16];
            RespGtestModel.Ta = dataRead[17];
            RespGtestModel.CountNum = dataRead[18];
            return RespGtestModel;
        }
    }
}
