using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtsosBo : BaseReadBo
    {
        public RespGtsos RespGtsosModel { get; private set; }

        public RespGtsosBo()
        {
            RespGtsosModel = new RespGtsos();
        }

        public RespGtsosBo(RespGtsos respGtsosModel)
        {
            RespGtsosModel = respGtsosModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTSOS Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtsos Object</returns>
        public RespGtsos ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtsosModel.CommandName = dataRead[0];
            RespGtsosModel.UniqueID = dataRead[1];
            RespGtsosModel.Number = dataRead[2];
            RespGtsosModel.ZoneId = dataRead[3];
            RespGtsosModel.ZoneAlert = dataRead[4];
            RespGtsosModel.GpsFix = dataRead[5];
            RespGtsosModel.Speed = dataRead[6];
            RespGtsosModel.Heading = dataRead[7];
            RespGtsosModel.Altitude = dataRead[8];
            RespGtsosModel.GpsAccuracy = dataRead[9];
            RespGtsosModel.Longitude = dataRead[10];
            RespGtsosModel.Latitude = dataRead[11];
            RespGtsosModel.SendTime = dataRead[12];
            RespGtsosModel.Mcc = dataRead[13];
            RespGtsosModel.Mnc = dataRead[14];
            RespGtsosModel.Lac = dataRead[15];
            RespGtsosModel.Cellid = dataRead[16];
            RespGtsosModel.Ta = dataRead[17];
            RespGtsosModel.CountNum = dataRead[18];
            return RespGtsosModel;
        }

    }
}
